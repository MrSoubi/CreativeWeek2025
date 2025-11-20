using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapManager : MonoBehaviour
{
    [SerializeField] private RSO_CurrentPickups rso_CurrentPickups;
    [SerializeField] private RSO_CurrentLocation rso_CurrentLocation;
    [SerializeField] private GameObject mapUI;
    [SerializeField] private InputActionReference mapToggleAction;

    [SerializeField] private GameObject m_ChinaTownPointer;
    [SerializeField] private GameObject m_MetroPointer;
    [SerializeField] private GameObject m_EgoutsPointer;
    [SerializeField] private GameObject m_VillePointer;
    
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

    private void Start()
    {
        mapUI.SetActive(false);
    }

    void ToggleMap(InputAction.CallbackContext context)
    {
        mapUI.SetActive(!mapUI.activeSelf);
        
        UpdateLocationPointers();
    }

    void UpdateLocationPointers()
    {
        m_ChinaTownPointer.SetActive(false);
        m_MetroPointer.SetActive(false);
        m_EgoutsPointer.SetActive(false);
        m_VillePointer.SetActive(false);
        
        foreach (IngredientPickup pickup in rso_CurrentPickups.IngredientPickups)
        {
            switch (pickup.spawnLocation)
            {
                case SpawnLocation.Chinatown:
                    m_ChinaTownPointer.SetActive(true);
                    continue;
                case SpawnLocation.Metro:
                    m_MetroPointer.SetActive(true);
                    continue;
                case SpawnLocation.Egout:
                    m_EgoutsPointer.SetActive(true);
                    continue;
                case SpawnLocation.Ville:
                    m_VillePointer.SetActive(true);
                    continue;
            }
        }
    }
}
