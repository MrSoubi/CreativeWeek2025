using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapManager : MonoBehaviour
{
    [SerializeField] private RSO_CurrentLocation rso_CurrentLocation;
    [SerializeField] private GameObject mapUI;
    [SerializeField] private InputActionReference mapToggleAction;
    
    private InputAction mapToggle;
    
    private void OnEnable()
    {
        mapToggle = mapToggleAction.action;
        mapToggle.performed += ToggleMap;
        mapToggle.Enable();
    }
    
    private void OnDisable()
    {
        mapToggle.performed -= ToggleMap;
        mapToggle.Disable();
    }

    void ToggleMap(InputAction.CallbackContext context)
    {
        mapUI.SetActive(!mapUI.activeSelf);
    }
}
