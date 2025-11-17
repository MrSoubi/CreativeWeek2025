using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_JumpForce;
    [SerializeField] private float m_Speed;
    [SerializeField] private Vector3 m_CurrentVelocity;
    [SerializeField] private InputActionReference m_JumpActionReference;
    [SerializeField] private float m_GroundCheckDistance = 0.1f;
    [SerializeField] private float m_NoGravityDelayAfterJump = 0.1f;
    [SerializeField] private RSE_OnMusicStarted m_OnMusicStarted;
    [SerializeField] private RSO_MusicTime m_MusicTime;
    
    private InputAction m_JumpAction;
    private bool m_IsJumping;
    private float m_Gravity;
    
    private void OnEnable()
    {
        m_JumpAction = m_JumpActionReference.action;
        m_JumpAction.performed += Jump;
        m_JumpAction.canceled += Fall;
        m_JumpAction.Enable();
        
        m_OnMusicStarted.onMusicStarted.AddListener(Run);
    }

    private void OnDisable()
    {
        m_JumpAction.performed -= Jump;
        m_JumpAction.canceled -= Fall;
        m_JumpAction.Disable();
        
        m_OnMusicStarted.onMusicStarted.RemoveListener(Run);
    }

    private void Start()
    {
        m_Gravity = Physics.gravity.y;
    }

    void Run()
    {
        m_CurrentVelocity.x = m_Speed;
    }
    
    void Stop()
    {
        m_CurrentVelocity = Vector3.zero;
    }
    
    void Jump(InputAction.CallbackContext ctx)
    {
        if (!IsGrounded() && !m_IsJumping) return;
        
        m_Gravity = Physics.gravity.y;
        m_CurrentVelocity.y += m_JumpForce;
        StartCoroutine(JumpTimer());
    }
    
    void Fall(InputAction.CallbackContext ctx)
    {
        m_Gravity = Physics.gravity.y * 2;
    }
    
    void Move()
    {
        transform.position += m_CurrentVelocity * Time.deltaTime;
    }

    private void Update()
    {
        if (!IsGrounded())
        {
            m_CurrentVelocity.y += m_Gravity * Time.deltaTime;
        }
        else if (!m_IsJumping)
        {
            m_CurrentVelocity.y = 0f;
        }
        
        Move();
    }
    
    bool IsGrounded()
    {
        
        Vector2 origin = new Vector2(transform.position.x, transform.position.y);
        Vector2 direction = Vector2.down;
        
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down);
        
        if (hit.collider != null && hit.distance <= 0.1f)
        {
            return true;
        }
        
        return false;
    }

    IEnumerator JumpTimer()
    {
        m_IsJumping = true;
        yield return new WaitForSeconds(m_NoGravityDelayAfterJump);
        m_IsJumping = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * m_GroundCheckDistance);
    }
    
    public void AttractToLanding(Transform landingTransform, float landingTime)
    {
        Stop();
        StopAllCoroutines();
        StartCoroutine(AttractToLandingCoroutine(landingTransform, landingTime));
    }

    IEnumerator AttractToLandingCoroutine(Transform landingTransform, float landingTime)
    {
        Vector2 movementRemaining = (Vector2)landingTransform.position - (Vector2)transform.position;
        float targetSpeed = movementRemaining.magnitude / (landingTime - m_MusicTime.Time);
        Vector2 direction = (movementRemaining / targetSpeed).normalized;
        while (m_MusicTime.Time < landingTime)
        {
            Vector2 movementThisFrame = direction * targetSpeed * Time.deltaTime;
            transform.position += (Vector3)movementThisFrame;
            yield return null;
        }
    }
}