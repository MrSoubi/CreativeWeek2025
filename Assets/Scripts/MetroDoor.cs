using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MetroDoor : MonoBehaviour
{
    [SerializeField] private MetroDoor m_Destination;
    [SerializeField] private InputActionReference m_InteractActionReference;
    
    private InputAction m_InteractAction;
    private GameObject m_Player;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            m_Player = other.gameObject;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            m_Player = null;
        }
    }

    private void OnEnable()
    {
        m_InteractAction = m_InteractActionReference.action;
        m_InteractAction.performed += OnInteractPerformed;
        m_InteractAction.Enable();
    }

    private void OnDisable()
    {
        m_InteractAction.performed -= OnInteractPerformed;
        m_InteractAction.Disable();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (m_Player != null)
        {
            m_Player.transform.position = m_Destination.transform.position;
        }
    }
}
