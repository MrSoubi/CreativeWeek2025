using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RSO_IngredientInLevel", menuName = "Scriptable Objects/RSO_IngredientInLevel")]
public class RSO_IngredientInLevel : ScriptableObject
{
    private Transform ingredientPosition;
    public UnityEvent<Transform> OnValueChanged;
    
    public Transform IngredientPosition
    {
        get { return ingredientPosition; }
        set
        {
            if (ingredientPosition != value)
            {
                ingredientPosition = value;
                OnValueChanged.Invoke(ingredientPosition);
            }
        }
    }
}
