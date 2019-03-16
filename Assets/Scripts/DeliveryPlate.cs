using System.Collections;
using System.Collections.Generic;
using TrainJam.Multiplayer;
using UnityEngine;

namespace TrainJam
{
    public class DeliveryPlate : MonoBehaviour
    {
        private static List<DeliveryPlate> m_DeliveryPlates = new List<DeliveryPlate>();
        public IReadOnlyList<DeliveryPlate> deliveryPlates => m_DeliveryPlates;
        [SerializeField] private int m_DeliverForPlayer;
        [SerializeField] private string m_FinishedText = "Finished!";
        [SerializeField] private TMPro.TextMeshPro nextText;
        [SerializeField] private GameObject successEffect;
        [SerializeField] private Transform effectPosition;

        public IngredientType[] neededIngredients;
        private int ingredientIndex = 0;
        public string orderId { get; set; }

        private void Start()
        {
            m_DeliveryPlates.Add(this);
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
                if (successEffect != null)
                {
                    Instantiate(successEffect, effectPosition.position, Quaternion.identity);
                }

                Destroy(ingredient.gameObject);
                ingredientIndex++;
                if (ingredientIndex < neededIngredients.Length)
                {
                    UpdateIngredientDisplay();
                }
                else
                {
                    Deliver();
                }
            }
        }

        private void Deliver()
        {
            nextText.text = m_FinishedText;
            GameController.instance.Deliver(orderId);
        }

        private void UpdateIngredientDisplay()
        {
            nextText.text = Misc.GetNameFromIngredientType(neededIngredients[ingredientIndex]);
        }
    }
}