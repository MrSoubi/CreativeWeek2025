using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class IngredientPickup : MonoBehaviour
{
    public Ingredient ingredient;
    public UnityEvent<Ingredient> onPickup;
    [SerializeField] private SpriteRenderer ingredientSprite;
    [SerializeField] private RSE_OnIngredientPickedUp m_OnIngredientPickedUp;
    public SpawnLocation spawnLocation;
    [SerializeField] private RSO_CurrentPickups m_CurrentPickups;
    public void Initialize(Ingredient ingredient, SpawnLocation location)
    {
        this.ingredient = ingredient;
        ingredientSprite.sprite = ingredient.ingredientSprite;
        this.spawnLocation = location;
        m_CurrentPickups.AddPickup(this);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            onPickup?.Invoke(ingredient);
            m_OnIngredientPickedUp.onPickup?.Invoke(ingredient);
            m_CurrentPickups.RemovePickup(this);
            Destroy(gameObject);
        }
    }
}