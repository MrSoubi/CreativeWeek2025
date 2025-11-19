using System;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.15f; // temps de lissage
    [SerializeField] private float maxSpeed; // vitesse maximale pour SmoothDamp
    [SerializeField] private float verticalOffset; // offset vertical de la caméra

    // Lookahead (anticipation) pour suivre des cibles rapides
    [SerializeField] private float lookaheadFactor = 0.25f; // en secondes d'anticipation (distance = vitesse * facteur)
    [SerializeField] private float maxLookahead = 3f; // distance maximale d'anticipation
    [SerializeField] private float lookaheadSmoothSpeed = 4f; // vitesse de lissage vers la valeur d'anticipation
    [SerializeField] private float lookaheadMoveThreshold = 0.05f; // seuil de vitesse pour activer l'anticipation
    
    private Vector3 currentVelocity = Vector3.zero; // utilisé par SmoothDamp
    private Vector3 currentLookahead = Vector3.zero;
    private Vector3 lastTargetPosition = Vector3.zero;
    
    [SerializeField] private RSE_AskFadeOut askFadeOut;

    private void OnEnable()
    {
        askFadeOut.Call.AddListener(ResetPosition);
    }

    private void OnDisable()
    {
        askFadeOut.Call.RemoveListener(ResetPosition);
    }

    void ResetPosition()
    {
        if (target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y + verticalOffset, transform.position.z);
            lastTargetPosition = target.position;
            currentLookahead = Vector3.zero;
            currentVelocity = Vector3.zero;
        }
    }
    
    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("Follower: target is not set.");
            return;
        }

        lastTargetPosition = target.position;
        // Position initiale de la caméra: alignée sur la cible
        transform.position = new Vector3(target.position.x, target.position.y + verticalOffset, transform.position.z);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calcul du déplacement de la cible depuis la dernière frame
        Vector3 targetDelta = (target.position - lastTargetPosition);

        // Vitesse horizontale (units/sec)
        float velocityX = targetDelta.x / Mathf.Max(Time.deltaTime, 1e-6f);

        // Détermine la lookahead désirée proportionnelle à la vitesse (distance = vitesse * facteur)
        float desiredLookaheadX = Mathf.Clamp(velocityX * lookaheadFactor, -maxLookahead, maxLookahead);

        // Si la vitesse est sous le seuil, on vise zéro lookahead
        Vector3 desiredLookahead = Mathf.Abs(velocityX) > lookaheadMoveThreshold ? Vector3.right * desiredLookaheadX : Vector3.zero;

        // Lisse la transition vers la lookahead désirée
        currentLookahead = Vector3.MoveTowards(currentLookahead, desiredLookahead, lookaheadSmoothSpeed * Time.deltaTime);

        // Position cible (avec offset vertical et lookahead)
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y + verticalOffset, transform.position.z) + currentLookahead;

        // SmoothDamp vers la position cible (adapté aux hautes vitesses)
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime, maxSpeed, Time.deltaTime);

        // Mémorise la position pour la prochaine frame
        lastTargetPosition = target.position;
    }
}
