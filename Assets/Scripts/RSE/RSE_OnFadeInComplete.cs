using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RSE_OnFadeInComplete", menuName = "Scriptable Objects/RSE_OnFadeInComplete")]
public class RSE_OnFadeInComplete : ScriptableObject
{
    public UnityEvent Call;
}
