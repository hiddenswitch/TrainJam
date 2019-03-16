using System.Collections.Generic;
using UnityEngine;

namespace TrainJam
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private Ingredient[] IngredientPrefabs;
        private Dictionary<IngredientType, GameObject> IngredientPrefabsDict = new Dictionary<IngredientType, GameObject>();

        public Transform spawnTransform;

        private void Awake()
        {
            foreach(var prefab in IngredientPrefabs)
            {
                IngredientPrefabsDict.Add(prefab.type, prefab.gameObject);
            }
        }

        public void SpawnIngredient(IngredientType type, Vector3 position)
        {
            Instantiate(IngredientPrefabsDict[type], position, Random.rotation);
        }

        public void SpawnIngredientAtSpawnPosition(IngredientType type)
        {
            SpawnIngredient(type, spawnTransform.position);
        }
    }
}