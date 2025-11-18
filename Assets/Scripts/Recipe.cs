using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
public class Recipe : ScriptableObject
{
    public string recipeName;
    public Ingredient[] ingredients;
    public Sprite recipeSprite;
}
