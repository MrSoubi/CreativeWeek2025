using System;
using UnityEngine;

public class LandingAttractor : MonoBehaviour
{
    [SerializeField] private float m_LandingTime;
    [SerializeField] private Transform m_LandingPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.AttractToLanding(m_LandingPosition, m_LandingTime);
            }
        }
    }
}
