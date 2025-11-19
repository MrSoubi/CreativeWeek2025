using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RSO_CurrentLocation", menuName = "Scriptable Objects/RSO_CurrentLocation")]
public class RSO_CurrentLocation : ScriptableObject
{
    private SpawnLocation currentLocation;
    public UnityEvent<SpawnLocation> onLocationChanged;
    public SpawnLocation CurrentLocation
    {
        get { return currentLocation; }
        set
        {
            currentLocation = value; 
            onLocationChanged?.Invoke(currentLocation);
            Debug.Log("Current Location changed to: " + currentLocation);
        }
    }
}
