using System.Collections;
using UnityEngine;

namespace TrainJam
{
    public class DeliveryPlate : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshPro nextText;

        [SerializeField] private GameObject successEffect;
        [SerializeField] private Transform effectPosition;

        public IngredientType[] neededIngredients;
        private int ingredientIndex = 0;

        private void Start()
        {
            UpdateIngredientDisplay();
        }

        public void DeliverIngredient(Ingredient ingredient)
        {
            if (ingredientIndex >= neededIngredients.Length)
            {
                return;
            }

            if (ingredient.type == neededIngredients[ingredientIndex])
            {
                Instantiate(successEffect, effectPosition.position, Quaternion.identity);

                Destroy(ingredient.gameObject);
                ingredientIndex++;
                if (ingredientIndex < neededIngredients.Length)
                {
                    UpdateIngredientDisplay();
                }else{
                    nextText.text = "FINISH";
                }
            }
        }

        private void UpdateIngredientDisplay()
        {
            nextText.text = neededIngredients[ingredientIndex].ToString();
        }
    }
}