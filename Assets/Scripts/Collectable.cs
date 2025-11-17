using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Collectable : MonoBehaviour
{
    [SerializeField] private InputActionReference m_InputActionReference;
    
    private InputAction m_InputAction;
    private bool m_IsPlayerInRange = false;

    private void OnEnable()
    {
        m_InputAction = m_InputActionReference.action;
        m_InputAction.performed += OnCollect;
        m_InputAction.Enable();
    }
    
    private void OnDisable()
    {
        m_InputAction.performed -= OnCollect;
        m_InputAction.Disable();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Entered");
        if (other.CompareTag("Player"))
            m_IsPlayerInRange = true;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Trigger Exited");
        if (other.CompareTag("Player"))
            m_IsPlayerInRange = false;
    }

    void OnCollect(InputAction.CallbackContext context)
    {
        Debug.Log("OnCollect");
        if (m_IsPlayerInRange)
        {
            // Add your collectable logic here (e.g., increase score, play sound, etc.)
            Debug.Log("Collectable collected!");
            Destroy(gameObject);
        }
    }
}
