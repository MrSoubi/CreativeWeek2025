using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class IngredientManager : MonoBehaviour
{
    private List<Ingredient> currentIngredients;
    [SerializeField] private List<Recipe> recipes;
    [SerializeField] private List<IngredientSpawn> ingredientSpawns;
    [SerializeField] private RSO_CurrentRecipe currentRecipe;
    
    // Spawns encore disponibles (non utilisés par les recettes précédentes)
    private List<IngredientSpawn> availableSpawns;
    // Spawns utilisés par toutes les recettes précédentes (à ne pas réutiliser pour la prochaine)
    private HashSet<IngredientSpawn> usedSpawnsAcrossRecipes = new HashSet<IngredientSpawn>();

    private void Start()
    {
        // Initialisation de la liste des spawns disponibles
        RebuildAvailableSpawns();
        SetRandomRecipe();
    }

    private void RebuildAvailableSpawns()
    {
        if (ingredientSpawns == null)
        {
            availableSpawns = new List<IngredientSpawn>();
            return;
        }

        availableSpawns = ingredientSpawns
            .Where(s => s != null && !usedSpawnsAcrossRecipes.Contains(s))
            .ToList();
    }

    private void SetRandomRecipe()
    {
        if (recipes == null || recipes.Count == 0)
        {
            Debug.LogWarning("No recipes set on IngredientManager.");
            return;
        }

        // Rebuild available spawns au démarrage de la recette
        RebuildAvailableSpawns();

        Recipe chosen = recipes[Random.Range(0, recipes.Count)];
        currentIngredients = new List<Ingredient>(chosen.ingredients);
        
        currentRecipe.Value = chosen;
        
        // On veut que chaque ingredient soit placé sur une Location différente
        HashSet<SpawnLocation> usedLocationsThisRecipe = new HashSet<SpawnLocation>();

        // Vérifier s'il y a assez de locations distinctes dans availableSpawns
        int needed = currentIngredients.Count;
        int distinctAvailableLocations = (availableSpawns ?? new List<IngredientSpawn>()).Select(s => s.spawnLocation).Distinct().Count();
        if (distinctAvailableLocations < needed)
        {
            // Pas assez de spawns restants distincts : on réinitialise les spawns utilisés entre recettes
            Debug.LogWarning("Not enough distinct available spawn locations for this recipe. Resetting used spawns pool so spawns can be reused.");
            usedSpawnsAcrossRecipes.Clear();
            RebuildAvailableSpawns();
        }

        foreach (Ingredient ingredient in currentIngredients)
        {
            IngredientSpawn spawn = ChooseSpawnForIngredient(usedLocationsThisRecipe);
            if (spawn == null)
            {
                Debug.LogError("Failed to find a suitable spawn for ingredient: " + ingredient.name);
                continue;
            }

            // Assigner l'ingredientManager sur le spawn au cas où l'inspector ne l'aurait pas fait
            spawn.ingredientManager = this;

            spawn.SpawnIngredient(ingredient);

            // Marquer la location utilisée dans cette recette
            usedLocationsThisRecipe.Add(spawn.spawnLocation);
            // Marquer le spawn comme utilisé pour qu'il ne soit pas réutilisé pour la prochaine recette
            usedSpawnsAcrossRecipes.Add(spawn);
            // Retirer de la liste disponible immédiatement
            availableSpawns?.Remove(spawn);
        }
    }

    private IngredientSpawn ChooseSpawnForIngredient(HashSet<SpawnLocation> usedLocationsThisRecipe)
    {
        if (availableSpawns == null || availableSpawns.Count == 0)
            RebuildAvailableSpawns();

        // 1) candidats non utilisés et avec une location différente de celles déjà prises dans la recette
        var candidates = (availableSpawns ?? new List<IngredientSpawn>())
            .Where(s => s != null && !usedLocationsThisRecipe.Contains(s.spawnLocation))
            .ToList();

        // 2) Si aucun candidat, essayer parmi tous les spawns (rester prudent)
        if (candidates.Count == 0)
        {
            candidates = (ingredientSpawns ?? new List<IngredientSpawn>())
                .Where(s => s != null && !usedLocationsThisRecipe.Contains(s.spawnLocation))
                .ToList();
        }

        // 3) Si toujours aucun (cas très rare), alors on accepte la réutilisation de location pour terminer la recette
        if (candidates.Count == 0)
        {
            candidates = (ingredientSpawns ?? new List<IngredientSpawn>()).Where(s => s != null).ToList();
        }

        if (candidates.Count == 0)
            return null;

        IngredientSpawn chosen = candidates[Random.Range(0, candidates.Count)];
        return chosen;
    }

    public void OnIngredientPickedUp(Ingredient ingredient)
    {
        currentIngredients.Remove(ingredient);

        if (currentIngredients.Count == 0)
        {
            Debug.Log("All ingredients collected! Recipe complete.");
            SetRandomRecipe();
        }
    }
}
