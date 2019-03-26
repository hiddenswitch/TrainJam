using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientButton : MonoBehaviour
{
    [SerializeField] private IngredientType spawnIngredient;
    [SerializeField] private TextMeshProUGUI buttonLabel;
    [SerializeField] private Button button;
    [SerializeField] private int cooldownTime = 10;
    private float cooldownTimer;

    private void Update()
    {
        if (cooldownTimer > 0){
            cooldownTimer -= Time.deltaTime;
            buttonLabel.text = ((int)cooldownTimer + 1).ToString();
        }else{
            buttonLabel.text = "Ready";
            button.interactable = true;
        }
    }

    public void SpawnObject()
    {
        SpawnManager.Instance.SpawnIngredient(spawnIngredient);
        cooldownTimer = cooldownTime;
        button.interactable = false;
    }
}
