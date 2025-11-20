using System;
using UnityEngine;

public class SandwichCounter : MonoBehaviour
{
    private int sandwichCount = 0;
    [SerializeField] private TMPro.TextMeshProUGUI sandwichCountText;
    [SerializeField] private RSO_CurrentRecipe onSandwichCompleted;
    bool hasFirstReceipeBeenCompleted = false;
    private void OnEnable()
    {
        onSandwichCompleted.OnValueChanged.AddListener(HandleSandwichCompleted);
    }

    private void OnDisable()
    {
        onSandwichCompleted.OnValueChanged.RemoveListener(HandleSandwichCompleted);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sandwichCount = 0;
        sandwichCountText.text = sandwichCount.ToString();
    }
    
    public void HandleSandwichCompleted(Recipe recipe)
    {
        if (!hasFirstReceipeBeenCompleted)
        {
            hasFirstReceipeBeenCompleted = true;
            return;
        }
        
        sandwichCount++;
        sandwichCountText.text = sandwichCount.ToString();
    }
}
