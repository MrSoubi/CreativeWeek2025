using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float m_Speed = 6f; // max speed
    [SerializeField] private float m_Acceleration = 40f; // how fast we reach target speed
    [SerializeField] [Range(0f, 1f)] private float m_AirControlMultiplier = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float m_CoastMultiplier = 0.2f; // how much we coast when no input (0 = no coast, 1 = instant stop)

    [Header("Jump")]
    [SerializeField] private float m_JumpForce = 12f;

    [Header("Input")]
    [SerializeField] private InputActionReference m_JumpActionReference;
    [SerializeField] private InputActionReference m_MoveActionReference;

    [Header("Ground Check")]
    [SerializeField] private float m_GroundCheckDistance = 0.1f;
    [SerializeField] private float m_GroundCheckRadius = 0.08f;
    [SerializeField] private LayerMask m_GroundLayer = ~0;

    [Header("Slide")]
    [SerializeField] private LayerMask m_SlideLayer; // assign SlideBars layer in inspector
    [SerializeField] private float m_SlideSpeed = 4f; // constant slide speed along the bar
    [SerializeField] private float m_SlideDetachCooldown = 0.25f; // seconds during which reattachment is blocked after detach
    [SerializeField] private float m_SlideAcceleration = 8f; // how fast speed increases when going downhill
    [SerializeField] private float m_SlideDeceleration = 0f; // how fast speed decreases when going uphill (0 = no decay)
    [SerializeField] private float m_MinIncomingSpeedForUphill = 1.5f; // minimum incoming speed along bar to allow uphill

    [Header("Components")]
    [SerializeField] private Rigidbody2D m_Rigidbody2D;
    [SerializeField] private Collider2D m_Collider2D;

    private InputAction m_JumpAction;
    private InputAction m_MoveAction;

    // runtime state
    private Vector2 m_MoveInput = Vector2.zero;

    // Slide state
    private bool m_IsSliding = false;
    private Collider2D m_CurrentSlideCollider = null;
    private Vector2 m_SlideDirection = Vector2.right;
    private float m_PrevGravityScale = 1f;
    private float m_LastSlideDetachTime = -Mathf.Infinity;
    private float m_CurrentSlideSpeed = 0f;

    private void OnEnable()
    {
        if (m_JumpActionReference != null)
        {
            m_JumpAction = m_JumpActionReference.action;
            if (m_JumpAction != null)
            {
                m_JumpAction.performed += Jump;
                m_JumpAction.Enable();
            }
        }

        if (m_MoveActionReference != null)
        {
            m_MoveAction = m_MoveActionReference.action;
            if (m_MoveAction != null)
            {
                m_MoveAction.performed += MovePerformed;
                m_MoveAction.canceled += MoveCanceled;
                m_MoveAction.Enable();
            }
        }

        if (m_Rigidbody2D == null)
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            if (m_Rigidbody2D == null)
                Debug.LogWarning("PlayerController: Rigidbody2D not assigned and not found on the GameObject.");
        }

        if (m_Collider2D == null)
        {
            m_Collider2D = GetComponent<Collider2D>();
            if (m_Collider2D == null)
                Debug.LogWarning("PlayerController: Collider2D not assigned and not found on the GameObject. Ground check may be unreliable.");
        }
    }

    private void OnDisable()
    {
        if (m_JumpAction != null)
        {
            m_JumpAction.performed -= Jump;
            m_JumpAction.Disable();
        }

        if (m_MoveAction != null)
        {
            m_MoveAction.performed -= MovePerformed;
            m_MoveAction.canceled -= MoveCanceled;
            m_MoveAction.Disable();
        }
    }

    void Jump(InputAction.CallbackContext ctx)
    {
        if (m_Rigidbody2D == null) return;

        if (m_IsSliding)
        {
            // Detach from slide and perform jump
            DetachFromSlide();

            // Apply jump impulse (use same as normal jump)
            m_Rigidbody2D.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
            return;
        }

        if (!IsGrounded()) return;

        // Reset vertical velocity then apply impulse for consistent jump height
        Vector2 v = m_Rigidbody2D.linearVelocity;
        v.y = 0f;
        m_Rigidbody2D.linearVelocity = v;
        m_Rigidbody2D.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
    }

    void MovePerformed(InputAction.CallbackContext ctx)
    {
        m_MoveInput = ctx.ReadValue<Vector2>();
    }

    void MoveCanceled(InputAction.CallbackContext ctx)
    {
        m_MoveInput = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (m_Rigidbody2D == null) return;

        // If currently sliding, override movement and stick to the bar
        if (m_IsSliding)
        {
            // progressive slide speed: accelerate if downhill, do not increase if uphill
            bool isDownhill = Vector2.Dot(m_SlideDirection, Vector2.down) > 0f;
            if (isDownhill)
            {
                // accelerate toward max slide speed
                m_CurrentSlideSpeed = Mathf.MoveTowards(m_CurrentSlideSpeed, m_SlideSpeed, m_SlideAcceleration * Time.fixedDeltaTime);
            }
            else
            {
                // uphill: do not increase; optionally decelerate if configured
                if (m_SlideDeceleration > 0f)
                    m_CurrentSlideSpeed = Mathf.MoveTowards(m_CurrentSlideSpeed, 0f, m_SlideDeceleration * Time.fixedDeltaTime);
                // otherwise keep current speed
            }

            // apply the current slide speed along the slide direction
            m_Rigidbody2D.linearVelocity = m_SlideDirection * m_CurrentSlideSpeed;

            // keep checking if the player left the slide collider (for example fell off)
            if (m_CurrentSlideCollider == null || !IsOverlappingCollider(m_CurrentSlideCollider))
            {
                DetachFromSlide();
            }

            return;
        }

        // Not sliding: normal movement
        // But first check whether we can attach to a slide bar this frame
        TryAttachToSlideIfAvailable();

        float target = m_MoveInput.x * m_Speed;

        float accel = m_Acceleration * Time.fixedDeltaTime;
        if (!IsGrounded()) accel *= m_AirControlMultiplier;

        // If there's no horizontal input, reduce the accel so the player coasts (roller-skate feel)
        if (Mathf.Approximately(m_MoveInput.x, 0f))
        {
            accel *= m_CoastMultiplier;
        }

        float currentX = m_Rigidbody2D.linearVelocity.x;
        float newX = Mathf.MoveTowards(currentX, target, accel);

        Vector2 lv = m_Rigidbody2D.linearVelocity;
        lv.x = newX;
        m_Rigidbody2D.linearVelocity = lv;
    }

    bool IsGrounded()
    {
        // Prefer collider bottom as the check origin when available
        Vector2 circleCenter;

        if (m_Collider2D != null)
        {
            Bounds b = m_Collider2D.bounds;
            circleCenter = new Vector2(b.center.x, b.min.y) + Vector2.down * (m_GroundCheckDistance * 0.5f);
        }
        else
        {
            circleCenter = (Vector2)transform.position + Vector2.down * (m_GroundCheckDistance);
        }

        Collider2D hit = Physics2D.OverlapCircle(circleCenter, m_GroundCheckRadius, m_GroundLayer);
        return hit != null;
    }

    // Try to attach to a slide bar if the player is overlapping a slide layer collider
    private void TryAttachToSlideIfAvailable()
    {
        if (m_IsSliding) return;
        // don't reattach immediately after detaching (e.g., jump) to avoid ping-pong
        if (Time.time - m_LastSlideDetachTime < m_SlideDetachCooldown) return;
        if (m_SlideLayer == 0) return; // not configured

        // compute circle center same as ground check
        Vector2 circleCenter;
        if (m_Collider2D != null)
        {
            Bounds b = m_Collider2D.bounds;
            circleCenter = new Vector2(b.center.x, b.min.y) + Vector2.down * (m_GroundCheckDistance * 0.5f);
        }
        else
        {
            circleCenter = (Vector2)transform.position + Vector2.down * (m_GroundCheckDistance);
        }

        // find any slide collider under the player (will return one overlap if present)
        Collider2D hit = Physics2D.OverlapCircle(circleCenter, m_GroundCheckRadius, m_SlideLayer);
        if (hit != null)
        {
            // Only attach if the player is approaching the slide from above (fell onto it)
            if (IsApproachingFromTop(hit, circleCenter))
            {
                AttachToSlide(hit);
            }
        }
    }

    // Return true if the player is above the slide collider and is approaching it from the top side
    private bool IsApproachingFromTop(Collider2D slideCollider, Vector2 checkPoint)
    {
        if (slideCollider == null) return false;

        Vector2 closest = slideCollider.ClosestPoint(checkPoint);

        // Vector from collider nearest point to player center
        Vector2 toPlayer = (Vector2)transform.position - closest;

        // Use the slide's local up as the 'top' normal direction
        Vector2 barNormal = slideCollider.transform.TransformDirection(Vector3.up);

        // If the player is not on the top side (dot <= small threshold), don't attach
        float dotPos = Vector2.Dot(toPlayer, barNormal);
        const float kTopThreshold = 0.02f;
        if (dotPos <= kTopThreshold) return false;

        // Ensure the player is moving toward the bar (relative to the bar normal) or at least not moving away
        Vector2 incoming = m_Rigidbody2D != null ? m_Rigidbody2D.linearVelocity : Vector2.zero;
        float dotVel = Vector2.Dot(incoming, -barNormal);

        // Allow attach if player is moving toward the bar (positive) or not strongly moving away
        if (dotVel > -0.1f) return true;

        return false;
    }

    private void AttachToSlide(Collider2D slideCollider)
    {
        if (slideCollider == null) return;

        m_CurrentSlideCollider = slideCollider;
        m_IsSliding = true;

        // store previous gravity and disable gravity while sliding to keep exact trajectory
        m_PrevGravityScale = m_Rigidbody2D.gravityScale;
        m_Rigidbody2D.gravityScale = 0f;

        // Determine slide direction from the collider's transform right vector (assumes bars are oriented to match their tilt)
        Vector3 worldRight = slideCollider.transform.TransformDirection(Vector3.right);
        Vector2 dir = new Vector2(worldRight.x, worldRight.y).normalized;

        // Choose slide direction based on player's incoming movement so uphill is possible when the player comes in with momentum.
        Vector2 chosen;
        Vector2 incoming = Vector2.zero;
        if (m_Rigidbody2D != null)
        {
            incoming = m_Rigidbody2D.linearVelocity;
        }

        // If we have almost no velocity, use the current horizontal input as a hint
        if (incoming.sqrMagnitude < 0.01f)
        {
            incoming = new Vector2(m_MoveInput.x, 0f);
        }

        if (incoming.sqrMagnitude > 0.0001f)
        {
            Vector2 incomingDir = incoming.normalized;
            float dotA = Vector2.Dot(incomingDir, dir);
            float dotB = Vector2.Dot(incomingDir, -dir);
            // choose the direction that aligns more with the incoming vector
            chosen = dotA >= dotB ? dir : -dir;
        }
        else
        {
            // Fallback: prefer the direction with a downward component (previous behavior)
            Vector2 altDir = -dir;
            chosen = (Vector2.Dot(dir, Vector2.down) < Vector2.Dot(altDir, Vector2.down)) ? altDir : dir;
        }

        // If the chosen direction is uphill, require a minimum incoming speed along that direction to allow uphill.
        // Otherwise, force the downhill direction.
        bool chosenIsUphill = Vector2.Dot(chosen, Vector2.down) < 0f;
        if (chosenIsUphill)
        {
            float incomingProjected = 0f;
            if (m_Rigidbody2D != null)
                incomingProjected = Vector2.Dot(m_Rigidbody2D.linearVelocity, chosen);
            // if incoming projection is below threshold, switch to downhill (opposite)
            if (incomingProjected < m_MinIncomingSpeedForUphill)
            {
                chosen = -chosen; // prefer downhill
            }
        }

        m_SlideDirection = chosen.normalized;

        // initialize current slide speed from player's incoming velocity projected along the slide direction
        float projected = 0f;
        if (m_Rigidbody2D != null)
            projected = Vector2.Dot(m_Rigidbody2D.linearVelocity, m_SlideDirection);
        projected = Mathf.Max(0f, projected);

        bool isDownhillInit = Vector2.Dot(m_SlideDirection, Vector2.down) > 0f;
        if (isDownhillInit)
        {
            // start from the projected incoming speed (may accelerate up to m_SlideSpeed)
            m_CurrentSlideSpeed = Mathf.Min(projected, m_SlideSpeed);
        }
        else
        {
            // uphill: speed cannot increase beyond incoming projection; clamp to max slide speed
            m_CurrentSlideSpeed = Mathf.Min(projected, m_SlideSpeed);
        }

        // Immediately set velocity to slide speed
        m_Rigidbody2D.linearVelocity = m_SlideDirection * m_CurrentSlideSpeed;
    }

    private void DetachFromSlide()
    {
        if (!m_IsSliding) return;

        m_IsSliding = false;
        m_CurrentSlideCollider = null;

        // restore gravity
        m_Rigidbody2D.gravityScale = m_PrevGravityScale;
        m_LastSlideDetachTime = Time.time;
    }

    private bool IsOverlappingCollider(Collider2D c)
    {
        if (c == null) return false;

        Vector2 circleCenter;
        if (m_Collider2D != null)
        {
            Bounds b = m_Collider2D.bounds;
            circleCenter = new Vector2(b.center.x, b.min.y) + Vector2.down * (m_GroundCheckDistance * 0.5f);
        }
        else
        {
            circleCenter = (Vector2)transform.position + Vector2.down * (m_GroundCheckDistance);
        }

        Collider2D hit = Physics2D.OverlapCircle(circleCenter, m_GroundCheckRadius, 1 << c.gameObject.layer);
        return hit == c;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector2 circleCenter;
        if (m_Collider2D != null)
        {
            Bounds b = m_Collider2D.bounds;
            circleCenter = new Vector2(b.center.x, b.min.y) + Vector2.down * (m_GroundCheckDistance * 0.5f);
        }
        else
        {
            circleCenter = (Vector2)transform.position + Vector2.down * (m_GroundCheckDistance);
        }

        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * m_GroundCheckDistance);
        Gizmos.DrawWireSphere(circleCenter, m_GroundCheckRadius);

        // draw slide check in blue
        Gizmos.color = Color.cyan;
        if (m_Collider2D != null)
        {
            Bounds b = m_Collider2D.bounds;
            circleCenter = new Vector2(b.center.x, b.min.y) + Vector2.down * (m_GroundCheckDistance * 0.5f);
        }
        else
        {
            circleCenter = (Vector2)transform.position + Vector2.down * (m_GroundCheckDistance);
        }

        Gizmos.DrawWireSphere(circleCenter, m_GroundCheckRadius);
    }
}
