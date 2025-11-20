using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class IngredientPickup : MonoBehaviour
{
    public Animator animator;
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
    
    bool pickedUp = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp) return;
        
        if (other.CompareTag("Player"))
        {
            pickedUp = true;
            onPickup?.Invoke(ingredient);
            m_OnIngredientPickedUp.onPickup?.Invoke(ingredient);
            m_CurrentPickups.RemovePickup(this);
            animator.SetTrigger("OnPickedUp");
        }
    }

    public void OnAnimationFinished()
    {
        Destroy(gameObject);
    }
}