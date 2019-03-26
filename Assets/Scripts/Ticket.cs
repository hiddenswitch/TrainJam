using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ticket : MonoBehaviour
{
    [SerializeField] private Image barImage;
    [SerializeField] private int fullWidth = 310;
    [SerializeField] private Image ingredientImage;
    [SerializeField] private Image effectImage;
    [SerializeField] private TextMeshProUGUI orderText;

    public int timeInSeconds = 30;
    public float fadeTime = 1f;

    private float timer;
    private float timePercentage = 1;

    private IngredientType askedIngredient;
    private Action[] askedActions;

    public Color successColor;
    public Color failedColor;

    private void Start()
    {
        SetNewRandomOrder();
    }

    public void SetTimer(int newTime){
        timer = newTime;
    }

    public bool DeliverIngredient(Ingredient ingredient){
        bool wrongDelivery = false;
        Action[] deliveredActions = ingredient.GetActions();
        if (askedIngredient == ingredient.ingredientType && askedActions.Length == deliveredActions.Length){
            for (int i = 0; i < askedActions.Length; i++){
                if (askedActions[i].actionType == deliveredActions[i].actionType && askedActions[i].amount == deliveredActions[i].amount){

                }else{
                    wrongDelivery = true;
                }
            }

            if (!wrongDelivery)
            {
                SetNewRandomOrder();
                SuccessEffect();
                return true;
            }
        }

        FailedEffect();
        return false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)){
            print(GetRandomBakeTemperature());
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            print(GetRandomGrillLevel());
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SuccessEffect();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetNewRandomOrder();
        }

        timer -= Time.deltaTime;
        if (timer < 0){
            timer = 0;
        }
        timePercentage = (float)(timer / timeInSeconds);
        barImage.rectTransform.sizeDelta = new Vector2(fullWidth * timePercentage, barImage.rectTransform.sizeDelta.y);
    }

    private int GetRandomBakeTemperature(){
        return (int)(Random.Range(75, 200) / 5) * 5;
    }

    private int GetRandomGrillLevel(){
        return Random.Range(1, 4);
    }

    private IngredientType GetRandomIngredientType(){
        int randomIngredientID = Random.Range(1, 6);

        switch(randomIngredientID){
            case 1:
                return IngredientType.Fish;
            case 2:
                return IngredientType.Bread;
            case 3:
                return IngredientType.Pineapple;
            case 4:
                return IngredientType.Potato;
            case 5:
                return IngredientType.Tomato;
        }

        return IngredientType.Bread;
    }

    private Action[] GetRandomActions(){
        HashSet<int> actionSet = new HashSet<int>();
        List<Action> actions = new List<Action>();

        for (int i = 0; i < 2; i++){
            int actionId = Random.Range(1, 4);
            actionSet.Add(actionId);
        }

        foreach(var actionID in actionSet){
            Action action;
            if (actionID == 1)
            {
                action.actionType = ActionType.Cut;
                action.amount = 0;
            }
            else if (actionID == 2)
            {
                action.actionType = ActionType.Bake;
                action.amount = GetRandomBakeTemperature();
            }
            else
            {
                action.actionType = ActionType.Grill;
                action.amount = GetRandomGrillLevel();
            }
            actions.Add(action);
        }

        return actions.ToArray();
    }

    public void SetNewRandomOrder(){
        askedIngredient = GetRandomIngredientType();
        askedActions = GetRandomActions();
        UpdateTicketDisplay();
        SetTimer(timeInSeconds);
    }

    private void UpdateTicketDisplay(){
        ingredientImage.sprite = Misc.Instance.GetSpriteForIngredientType(askedIngredient);
        orderText.text = "";
        for (int i = 0; i < askedActions.Length; i++){
            if (askedActions[i].amount > 0){
                orderText.text += $"{i + 1}: {askedActions[i].actionType.ToString()} @ {askedActions[i].amount}";
            }else{
                orderText.text += $"{i + 1}: {askedActions[i].actionType.ToString()}";
            }

            if (i < askedActions.Length - 1){
                orderText.text += "\n";
            }
        }
    }

    public void SuccessEffect()
    {
        effectImage.DOPause();
        effectImage.color = successColor;
        effectImage.DOFade(0, fadeTime);
    }

    public void FailedEffect()
    {
        effectImage.DOPause();
        effectImage.color = failedColor;
        effectImage.DOFade(0, fadeTime);
    }
}
