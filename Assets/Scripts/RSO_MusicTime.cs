using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RSO_MusicTime", menuName = "Scriptable Objects/RSO_MusicTime")]
public class RSO_MusicTime : ScriptableObject
{
    public UnityEvent OnValueChanged;
    private float m_Time;
    
    public float Time
    {
        get { return m_Time; }
        set
        {
            if (m_Time != value)
            {
                OnValueChanged.Invoke();
            }
            m_Time = value;
        }
    }
}
