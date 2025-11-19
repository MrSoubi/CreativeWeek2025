using UnityEngine;
using UnityEngine.Events;

public class IngredientPickup : MonoBehaviour
{
    public Ingredient ingredient;
    public UnityEvent<Ingredient> onPickup;
    [SerializeField] private SpriteRenderer ingredientSprite;
    [SerializeField] private RSE_OnIngredientPickedUp m_OnIngredientPickedUp;
    private SpawnLocation spawnLocation;
    
    public void Initialize(Ingredient ingredient, SpawnLocation location)
    {
        this.ingredient = ingredient;
        ingredientSprite.sprite = ingredient.ingredientSprite;
        this.spawnLocation = location;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            onPickup.Invoke(ingredient);
            m_OnIngredientPickedUp.onPickup.Invoke(ingredient);
            Destroy(gameObject);
        }
    }
}