using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    [SerializeField] private Ingredient[] ingredients;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private Transform spawnTopLeft;
    [SerializeField] private Transform spawnBottomRight;
    private Dictionary<IngredientType, Ingredient> ingredientsDict = new Dictionary<IngredientType, Ingredient>();

    private void Awake()
    {
        FillDictionary();
    }

    private void FillDictionary()
    {
        foreach (var ingredient in ingredients)
        {
            ingredientsDict.Add(ingredient.ingredientType, ingredient);
        }
    }

    public void SpawnIngredient(IngredientType ingredientType, Action[] actions)
    {
        Ingredient spawnedIngredient = SpawnIngredient(ingredientType);
        spawnedIngredient.AddActions(actions);
    }

    public void SpawnIngredient(Ingredient ingredient){
        SpawnIngredient(ingredient.ingredientType, ingredient.GetActions());
    }

    public Ingredient SpawnIngredient(IngredientType ingredientType)
    {
        float x = Random.Range(spawnTopLeft.position.x, spawnBottomRight.position.x);
        float y = Random.Range(spawnBottomRight.position.y, spawnTopLeft.position.y);

        Ingredient spawnedIngredient = Instantiate(ingredientsDict[ingredientType], new Vector3(x, y, 0), Quaternion.identity, canvasTransform);
        return spawnedIngredient;
    }
}
