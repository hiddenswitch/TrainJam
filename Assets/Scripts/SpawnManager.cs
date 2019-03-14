using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject prefab1;

    public void SpawnPrefab1(Vector3 position)
    {
        Instantiate(prefab1, position, Random.rotation);
    }
}
