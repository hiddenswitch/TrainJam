using UnityEngine;

namespace TrainJam
{
    public class DeliveryPlate : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshPro nextText;

        public GameObject successEffect;
        public Transform effectPosition;

        public IngredientType[] neededIngredients;
        private int ingredientIndex = 0;

        public GameObject wrongItemText;

        private float heightOffset = 0.05f;

        private void Start()
        {
            UpdateIngredientDisplay();
        }

        System.Collections.IEnumerator WrongItemCoroutine(){
            wrongItemText.SetActive(true);
            yield return new WaitForSeconds(0.8f);
            wrongItemText.SetActive(false);
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
                /*Destroy(ingredient.gameObject.GetComponent<ProjectedDragObject>());
                Destroy(ingredient.gameObject.GetComponent<CanBeTrashed>());
                Destroy(ingredient.gameObject.GetComponent<CanBePlated>());
                Destroy(ingredient.gameObject.GetComponent<BoxCollider>());
                Destroy(ingredient.gameObject.GetComponent<Rigidbody>());
                Destroy(ingredient.gameObject.GetC)*/

                ingredient.transform.position = transform.position + new Vector3(0, heightOffset * (ingredientIndex+1), 0);
                ingredientIndex++;
                if (ingredientIndex < neededIngredients.Length)
                {
                    UpdateIngredientDisplay();
                }else{
                    nextText.text = "FINISH";
                }
            }else
            {
                //StopCoroutine("WrongItemCoroutine");
                //StartCoroutine(WrongItemCoroutine());
            }
        }

        private void UpdateIngredientDisplay()
        {
            nextText.text = neededIngredients[ingredientIndex].ToString();
        }
    }
}