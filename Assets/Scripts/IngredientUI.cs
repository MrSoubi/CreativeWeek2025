using System;
using UnityEngine;

public class IngredientUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI ingredientNameText;
    [SerializeField] private UnityEngine.UI.Image ingredientImage;
    [SerializeField] private UnityEngine.UI.Image crossedImage;
    [SerializeField] private RSE_OnIngredientPickedUp OnIngredientPickedUp;
    private Ingredient currentIngredient;
    public RSO_CurrentRecipe currentRecipe;
    
    private void OnEnable()
    {
        OnIngredientPickedUp.onPickup.AddListener(HandleIngredientPickedUp);
        currentRecipe.OnValueChanged.AddListener(OnRecipeChanged);
    }
    
    private void OnDisable()
    {
        OnIngredientPickedUp.onPickup.RemoveListener(HandleIngredientPickedUp);
        currentRecipe.OnValueChanged.RemoveListener(OnRecipeChanged);
    }
    
    private void OnRecipeChanged(Recipe recipe)
    {
        Destroy(gameObject);
    }
    
    public void SetIngredient(Ingredient ingredient)
    {
        currentIngredient = ingredient;
        ingredientNameText.text = ingredient.ingredientName;
        ingredientImage.sprite = ingredient.ingredientSprite;
        crossedImage.enabled = false;
    }
    
    public void HandleIngredientPickedUp(Ingredient ingredient)
    {
        if (ingredient.ingredientName != currentIngredient.ingredientName) return;
        Debug.Log("Ingredient picked up: " + ingredient.ingredientName);
        crossedImage.enabled = true;
    }
}
