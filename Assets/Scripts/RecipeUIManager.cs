using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUIManager : MonoBehaviour
{
    [SerializeField] RSO_CurrentRecipe currentRecipe;
    [SerializeField] GameObject ingredientUIPrefab;
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Image recipeSprite;
    [SerializeField] private Transform ingredientsParent;
    private void OnEnable()
    {
        currentRecipe.OnValueChanged.AddListener(SetRecipe);
    }

    private void OnDisable()
    {
        currentRecipe.OnValueChanged.RemoveListener(SetRecipe);
    }

    public void SetRecipe(Recipe recipe)
    {
        foreach (Transform child in ingredientsParent)
        {
            Destroy(child.gameObject);
        }
        
        recipeNameText.text = recipe.recipeName;
        recipeSprite.sprite = recipe.recipeSprite;

        foreach (Ingredient ingredient in recipe.ingredients)
        {
            GameObject ingredientUIObj = Instantiate(ingredientUIPrefab, ingredientsParent);
            IngredientUI ingredientUI = ingredientUIObj.GetComponent<IngredientUI>();
            ingredientUI.SetIngredient(ingredient);
        }
    }
}
