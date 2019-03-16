using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainJam
{
    public class Misc : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            Application.targetFrameRate = 60;
        }

        public static string GetNameFromIngredientType(IngredientType type)
        {
            switch(type)
            {
                case IngredientType.BlockOfBread:
                    return "Loaf of Bread";
                case IngredientType.BlockOfCheese:
                    return "Brick of Cheese";
                case IngredientType.BlockOfLettuce:
                    return "Block of Lettuce";
                case IngredientType.Bun:
                    return "Slice of Bread";
                case IngredientType.Burger:
                    return "Burger";
                case IngredientType.SliceOfCheese:
                    return "Slice Of Cheese";
                case IngredientType.SliceOfLettuce:
                    return "Slice Of Lettuce";
                default:
                    return "No Name";
            }
        }
    }
}