using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RSE_OnPlayerTeleported", menuName = "Scriptable Objects/RSE_OnPlayerTeleported")]
public class RSE_OnPlayerTeleported : ScriptableObject
{
    public UnityEvent<SpawnLocation> Triggered;
}
