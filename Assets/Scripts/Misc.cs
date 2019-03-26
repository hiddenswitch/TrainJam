using UnityEngine;
using UnityEngine.UI;

public enum PlayerType
{
    Blue,
    Orange,
    Green,
    Trash
}

public enum IngredientType
{
    Tomato,
    Bread,
    Pineapple,
    Fish,
    Potato
}

public enum ActionType{
    Cut,
    Bake,
    Grill
}

[System.Serializable]
public struct Action{
    public ActionType actionType;
    public int amount;
}

public class Misc : Singleton<Misc>
{
    [Header("Ingredient Sprites")]
    public Sprite fishSprite;
    public Sprite breadSprite;
    public Sprite tomatoSprite;
    public Sprite potatoSprite;
    public Sprite pineappleSprite;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    public static void SetImageAlpha(Image image, float alpha){
        Color col = image.color;
        col.a = alpha;
        image.color = col;
    }

    public static string GetPastTenseString(ActionType action){
        switch(action){
            case ActionType.Cut:
                return "Cut";
            case ActionType.Bake:
                return "Baked";
            case ActionType.Grill:
                return "Grilled";
        }

        return action.ToString();
    }

    public Sprite GetSpriteForIngredientType(IngredientType type){
        switch(type){
            case IngredientType.Fish:
                return fishSprite;
            case IngredientType.Bread:
                return breadSprite;
            case IngredientType.Tomato:
                return tomatoSprite;
            case IngredientType.Potato:
                return potatoSprite;
            case IngredientType.Pineapple:
                return pineappleSprite;
        }

        return fishSprite;
    }
}