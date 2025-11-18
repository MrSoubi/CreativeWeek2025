using UnityEngine;

public enum SpawnLocation
{
    Ville,
    Chinatown,
    Metro,
    Egout
}

public class IngredientSpawn : MonoBehaviour
{
    public SpawnLocation spawnLocation;
    public GameObject ingredientPrefab;
    public IngredientManager ingredientManager;
    
    public void SpawnIngredient(Ingredient ingredient)
    {
        GameObject pickup = Instantiate(ingredientPrefab, transform.position, Quaternion.identity);
        pickup.GetComponent<IngredientPickup>().Initialize(ingredient);
        pickup.GetComponent<IngredientPickup>().onPickup.AddListener(ingredientManager.OnIngredientPickedUp);
    }
}


