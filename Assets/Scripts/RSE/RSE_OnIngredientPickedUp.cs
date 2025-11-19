using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RSE_OnIngredientPickedUp", menuName = "Scriptable Objects/RSE_OnIngredientPickedUp")]
public class RSE_OnIngredientPickedUp : ScriptableObject
{
    public UnityEvent<Ingredient> onPickup;
}
