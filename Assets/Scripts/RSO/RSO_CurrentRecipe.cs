using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RSO_CurrentRecipe", menuName = "Scriptable Objects/RSO_CurrentRecipe")]
public class RSO_CurrentRecipe : ScriptableObject
{
    public UnityEvent<Recipe> OnValueChanged;
    private Recipe m_Value;
    
    public Recipe Value
    {
        get { return m_Value; }
        set
        {
            if (m_Value != value)
            {
                OnValueChanged?.Invoke(value);
            }
            m_Value = value;
        }
    }
}
