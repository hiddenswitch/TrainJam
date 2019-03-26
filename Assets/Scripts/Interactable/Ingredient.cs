using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class Ingredient : UIBehaviour, IDragHandler
{
    public IngredientType ingredientType;

    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI statusText;

    private Vector2 gripOffset;
    private List<Action> actions = new List<Action>();

    public void OnDrag(PointerEventData eventData)
    {
    }


    private void Start()
    {
        this.OnBeginDragAsObservable().Subscribe(pointer =>
        {
            canvasGroup.blocksRaycasts = false;
            gripOffset = new Vector2(transform.position.x, transform.position.y) - pointer.position;
        }).AddTo(this);

        this.OnEndDragAsObservable().Subscribe(pointer =>
        {
            var hoveringObject = pointer.pointerCurrentRaycast.gameObject;
            if (hoveringObject){
                Actionable actionable = hoveringObject.GetComponent<Actionable>();
                if (actionable)
                {
                    actionable.ExecuteAction(this);
                }
            }


            canvasGroup.blocksRaycasts = true;
            Misc.SetImageAlpha(backgroundImage, 1f);
            actionText.text = "";

        }).AddTo(this);

        this.OnDragAsObservable().Subscribe(pointer =>
        {
            transform.position = pointer.position + gripOffset;

            var hoveringObject = pointer.pointerCurrentRaycast.gameObject;
            if (hoveringObject)
            {
                Actionable action = hoveringObject.GetComponent<Actionable>();
                if (action)
                {
                    Misc.SetImageAlpha(backgroundImage, 0.75f);
                    actionText.text = action.GetHoverActionText();
                }else
                {
                    Misc.SetImageAlpha(backgroundImage, 1f);
                    actionText.text = "";
                }
            }
            else
            {
                actionText.text = "";
                Misc.SetImageAlpha(backgroundImage, 1f);
            }
        }).AddTo(this);
    }

    public void AddAction(Action action){

        foreach(var a in actions){
            if (a.actionType == action.actionType){
                //FAILED TO REPEAT ACTION AGAIN
                return;
            }
        }

        actions.Add(action);
        UpdateStatus();
    }

    public void AddActions(Action[] actionsParam){
        foreach(var action in actionsParam){
            AddAction(action);
        }
    }

    public Action[] GetActions(){
        return actions.ToArray();
    }

    private void UpdateStatus(){
        statusText.text = "";
        for (int i = 0; i < actions.Count; i++){
            if (i > 0){
                statusText.text += "\n";
            }
            statusText.text += Misc.GetPastTenseString(actions[i].actionType);
            if (actions[i].amount > 0){
                statusText.text += $" {actions[i].amount.ToString()}";
            }
        }
    }
}
