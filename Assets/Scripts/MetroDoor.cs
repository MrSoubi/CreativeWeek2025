using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MetroDoor : MonoBehaviour
{
    [SerializeField] private MetroDoor m_Destination;
    [SerializeField] private InputActionReference m_InteractActionReference;
    [SerializeField] private GameObject m_UI;
    [SerializeField] private RSE_OnPlayerTeleported m_OnPlayerTeleported;
    [SerializeField] private RSE_AskFadeIn m_AskFadeIn;
    [SerializeField] private RSE_AskFadeOut m_AskFadeOut;
    [SerializeField] private SpawnLocation m_Location;
    private InputAction m_InteractAction;
    private GameObject m_Player;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            m_Player = other.gameObject;
            DisplayInteractionPrompt();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTeleporting)
        {
            m_Player = null;
            HideInteractionPrompt();
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

    private void Start()
    {
        HideInteractionPrompt();
    }

    private void DisplayInteractionPrompt()
    {
        if (m_UI != null)
        {
            m_UI.SetActive(true);
        }
    }
    
    private void HideInteractionPrompt()
    {
        if (m_UI != null)
        {
            m_UI.SetActive(false);
        }
    }
    
    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (m_Player != null)
        {
            StartCoroutine(TeleportCoroutine());
        }
    }

    private bool isTeleporting;
    IEnumerator TeleportCoroutine()
    {
        isTeleporting = true;
        m_AskFadeIn.Call.Invoke();
        m_Player.GetComponent<PlayerController>().Freeze();
        yield return new WaitForSeconds(.9f); // Attendre la fin du fondu
        
        m_Player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        m_Player.transform.position = m_Destination.transform.position;
        m_OnPlayerTeleported.Triggered.Invoke(m_Destination.m_Location);
        yield return new WaitForSeconds(.9f);
        m_Player.GetComponent<PlayerController>().UnFreeze();
        HideInteractionPrompt();
        m_Player = null;
        
        m_AskFadeOut.Call.Invoke();
    }
}
