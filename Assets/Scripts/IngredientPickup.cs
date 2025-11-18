using UnityEngine;
using UnityEngine.Events;

public class IngredientPickup : MonoBehaviour
{
    public Ingredient ingredient;
    public UnityEvent<Ingredient> onPickup;
    [SerializeField] private SpriteRenderer ingredientSprite;

    public void Initialize(Ingredient ingredient)
    {
        this.ingredient = ingredient;
        ingredientSprite.sprite = ingredient.ingredientSprite;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Logic to add the ingredient to the player's inventory
            Debug.Log("Picked up: " + ingredient.ingredientName);
            onPickup.Invoke(ingredient);
            Destroy(gameObject);
        }
    }
}