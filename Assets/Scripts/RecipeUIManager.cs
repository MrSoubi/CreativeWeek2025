using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUIManager : MonoBehaviour
{
    [SerializeField] RSO_CurrentRecipe currentRecipe;
    
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private TextMeshProUGUI ingredient1NameText;
    [SerializeField] private TextMeshProUGUI ingredient2NameText;
    [SerializeField] private TextMeshProUGUI ingredient3NameText;
    [SerializeField] private TextMeshProUGUI ingredient4NameText;
    
    [SerializeField] private Image recipeSprite;
    [SerializeField] private Image ingredient1Sprite;
    [SerializeField] private Image ingredient2Sprite;
    [SerializeField] private Image ingredient3Sprite;
    [SerializeField] private Image ingredient4Sprite;

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
        recipeNameText.text = recipe.recipeName;
        recipeSprite.sprite = recipe.recipeSprite;
        ingredient1NameText.text = recipe.ingredients[0].ingredientName;
        ingredient1Sprite.sprite = recipe.ingredients[0].ingredientSprite;
        ingredient2NameText.text = recipe.ingredients[1].ingredientName;
        ingredient2Sprite.sprite = recipe.ingredients[1].ingredientSprite;
        ingredient3NameText.text = recipe.ingredients[2].ingredientName;
        ingredient3Sprite.sprite = recipe.ingredients[2].ingredientSprite;
        ingredient4NameText.text = recipe.ingredients[3].ingredientName;
        ingredient4Sprite.sprite = recipe.ingredients[3].ingredientSprite;
    }
}
