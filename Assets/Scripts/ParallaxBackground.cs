// csharp
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Tooltip("Transform de la caméra à suivre (ou le joueur).")]
    public Transform cameraTransform;

    [Tooltip("0 = background fixe, 1 = background bouge à la même vitesse que la caméra.")]
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    [Tooltip("Adoucissement du mouvement (0 = pas de lissage).")]
    public float smoothing = 5f;

    public SpriteRenderer foreground;
    public SpriteRenderer background;
    
    private Vector3 previousCameraPosition;
    private Vector3 targetPosition;

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        previousCameraPosition = cameraTransform != null ? cameraTransform.position : Vector3.zero;
        targetPosition = transform.position;
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;
        
        // Sécuriser l'accès aux SpriteRenderers
        if (foreground != null && background != null)
            background.enabled = foreground.isVisible;
        
        Vector3 cameraDelta = cameraTransform.position - previousCameraPosition;

        // Déplacer seulement sur X pour un jeu 2D
        Vector3 parallaxMovement = new Vector3(cameraDelta.x * parallaxFactor, 0f, 0f);

        targetPosition += parallaxMovement;

        if (smoothing > 0f)
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
        else
            transform.position = targetPosition;

        previousCameraPosition = cameraTransform.position;
    }
}