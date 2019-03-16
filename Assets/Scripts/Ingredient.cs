using UnityEngine;

namespace TrainJam
{
    public class Ingredient : MonoBehaviour
    {
        public IngredientType type;
    }

    public enum IngredientType
    {
        BlockOfBread,
        Bun,
        BlockOfCheese,
        SliceOfCheese,
        BlockOfLettuce,
        SliceOfLettuce,
        CheeseBurger,
        HamBurger,
        CookedMeat,
        UncookedMeat
    }
}