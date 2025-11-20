using UnityEngine;

public class MapPointer : MonoBehaviour
{
    // Direction locale le long de laquelle le GameObject se déplace (utilisez (0,1,0) pour 'up' en 2D)
    public Vector3 localDirection = Vector3.up;
    // Distance totale parcourue (aller+retour = travelDistance, l'objet oscille autour de startPosition)
    public float travelDistance = 1f;
    // Vitesse du déplacement (cycles par seconde multipliée par travelDistance via PingPong)
    public float speed = 1f;

    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (travelDistance <= 0f || speed <= 0f)
            return;

        // PingPong entre 0 et travelDistance
        float t = Mathf.PingPong(Time.time * speed, travelDistance);
        // Recentre pour obtenir une oscillation symétrique autour de startPosition: [-travelDistance/2, +travelDistance/2]
        float offsetAlong = t - (travelDistance * 0.5f);

        // Convertit la direction locale en direction monde
        Vector3 worldDir = transform.TransformDirection(localDirection.normalized);

        transform.position = startPosition + worldDir * offsetAlong;
    }
}
