using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RSE_OnFadeOutComplete", menuName = "Scriptable Objects/RSE_OnFadeOutComplete")]
public class RSE_OnFadeOutComplete : ScriptableObject
{
    public UnityEvent Call;
}
