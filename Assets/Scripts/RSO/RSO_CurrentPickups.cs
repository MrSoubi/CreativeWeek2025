using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RSO_CurrentPickups", menuName = "Scriptable Objects/RSO_CurrentPickups")]
public class RSO_CurrentPickups : ScriptableObject
{
    List<IngredientPickup> ingredientPickups;
    public UnityEvent<List<IngredientPickup>> OnPickupsChanged;
    
    public List<IngredientPickup> IngredientPickups
    {
        get { return ingredientPickups; }
        set
        {
            if (ingredientPickups == null)
                ingredientPickups = new List<IngredientPickup>();
                
            ingredientPickups = value; 
            Debug.Log("Current Pickups changed. Total pickups: " + ingredientPickups.Count);
            OnPickupsChanged?.Invoke(ingredientPickups);
        }
    }
    
    public void AddPickup(IngredientPickup pickup)
    {
        if (ingredientPickups == null)
            ingredientPickups = new List<IngredientPickup>();
        
        ingredientPickups.Add(pickup);
        OnPickupsChanged?.Invoke(ingredientPickups);
        Debug.Log("Current Pickups changed. Total pickups: " + ingredientPickups.Count);
        Debug.Log("Added pickup: " + pickup.ingredient.name + " at location: " + pickup.spawnLocation);
    }
    
    public void RemovePickup(IngredientPickup pickup)
    {
        if (ingredientPickups == null)
            ingredientPickups = new List<IngredientPickup>();
        
        ingredientPickups.Remove(pickup);
        OnPickupsChanged?.Invoke(ingredientPickups);
        Debug.Log("Current Pickups changed. Total pickups: " + ingredientPickups.Count);
    }
}
