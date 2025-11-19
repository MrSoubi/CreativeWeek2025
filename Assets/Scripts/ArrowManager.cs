using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    public RSO_CurrentPickups currentPickups;
    public RSO_CurrentLocation currentLocation;
    public GameObject arrowIndicator;

    private Transform target;

    private void Start()
    {
        currentLocation.CurrentLocation = SpawnLocation.Ville;
        StartCoroutine(PostInit());
    }
    
    IEnumerator PostInit()
    {
        yield return new WaitForEndOfFrame();
        UpdateArrowVisibility(null);
    }

    private void SetArrowVisibility(bool isVisible)
    {
        arrowIndicator.SetActive(isVisible);
    }
    
    private void OnEnable()
    {
        currentPickups.OnPickupsChanged.AddListener(UpdateArrowVisibility);
        currentLocation.onLocationChanged.AddListener(UpdateArrowVisibility);
    }
    
    private void OnDisable()
    {
        currentPickups.OnPickupsChanged.RemoveListener(UpdateArrowVisibility);
        currentLocation.onLocationChanged.RemoveListener(UpdateArrowVisibility);
    }
    
    private void UpdateArrowVisibility(SpawnLocation _)
    {
        foreach (IngredientPickup pickup in currentPickups.IngredientPickups)
        {
            if (pickup.spawnLocation == currentLocation.CurrentLocation)
            {
                target = pickup.transform;
                SetArrowVisibility(true);
                return;
            }
        }
        
        target = null;
        SetArrowVisibility(false);
    }
    
    private void UpdateArrowVisibility(List<IngredientPickup> _)
    {
        foreach (IngredientPickup pickup in currentPickups.IngredientPickups)
        {
            if (pickup == null)
            {
                Debug.Log("Null pickup found in currentPickups. Skipping.");
                continue;
            }
            if (pickup.spawnLocation == currentLocation.CurrentLocation)
            {
                target = pickup.transform;
                SetArrowVisibility(true);
                return;
            }
        }
        
        target = null;
        SetArrowVisibility(false);
    }

    
    private void Update()
    {
        UpdateOrientation();
    }
    
    void UpdateOrientation()
    {
        if (target == null || arrowIndicator == null)
            return;

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("ArrowManager: No main camera found for orientation calculation.");
            return;
        }

        // Convert world positions to screen points for a reliable 2D angle on UI canvases
        Vector2 screenTarget = RectTransformUtility.WorldToScreenPoint(cam, target.position);
        Vector2 screenArrow = RectTransformUtility.WorldToScreenPoint(cam, arrowIndicator.transform.position);
        Vector2 dir = screenTarget - screenArrow;

        if (dir.sqrMagnitude < 0.000001f)
            return;

        // Calculate the angle in degrees and rotate only around the Z axis so the UI arrow doesn't tilt in 3D
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float desiredZ = angle - 90f; // adjust if your arrow sprite points 'up'

        Transform t = arrowIndicator.transform;
        float currentZ = t.localEulerAngles.z;
        float newZ = Mathf.LerpAngle(currentZ, desiredZ, Time.deltaTime * 5f);
        t.localRotation = Quaternion.Euler(0f, 0f, newZ);
    }

}
