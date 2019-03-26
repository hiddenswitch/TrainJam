using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tool : Actionable
{
    public Action action;

    [Header("References")]
    [SerializeField] private Image actionImage;
    [SerializeField] private TextMeshProUGUI actionText;

    [SerializeField] private Animator toolAnimation;

    public override string GetHoverActionText()
    {
        return $"{action.actionType.ToString()} This";
    }

    public override void ExecuteAction(Ingredient ingredient)
    {
        ingredient.AddAction(action);

        if (toolAnimation){
            toolAnimation.SetTrigger("Effect");
            toolAnimation.Play(0);
        }
    }
}
