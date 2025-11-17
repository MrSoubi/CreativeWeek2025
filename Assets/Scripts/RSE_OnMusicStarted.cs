using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RSE_OnMusicStarted", menuName = "Scriptable Objects/RSE_OnMusicStarted")]
public class RSE_OnMusicStarted : ScriptableObject
{
    public UnityEvent onMusicStarted;
}
