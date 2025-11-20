using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RSO_Timer", menuName = "Scriptable Objects/RSO_Timer")]
public class RSO_Timer : ScriptableObject
{
    private float timeRemaining;
    public UnityEvent<float> onTimerChanged;
    public UnityEvent onTimerReset;
    
    public float TimeRemaining
    {
        get { return timeRemaining; }
        set
        {
            if (value != timeRemaining)
            {
                timeRemaining = value;
                onTimerChanged?.Invoke(timeRemaining);
            }
            
        }
    }
    
    public void Reset(float newTime)
    {
        timeRemaining = newTime;
        onTimerReset?.Invoke();
        onTimerChanged?.Invoke(timeRemaining);
    }
}
