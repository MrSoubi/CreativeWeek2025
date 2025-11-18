using UnityEngine;
using UnityEngine.Events;

public class IngredientPickup : MonoBehaviour
{
    public Ingredient ingredient;
    public UnityEvent<Ingredient> onPickup;
    [SerializeField] private SpriteRenderer ingredientSprite;
    [SerializeField] private RSE_OnIngredientPickedUp m_OnIngredientPickedUp;
    public void Initialize(Ingredient ingredient)
    {
        this.ingredient = ingredient;
        ingredientSprite.sprite = ingredient.ingredientSprite;
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