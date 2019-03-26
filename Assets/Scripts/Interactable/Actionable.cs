using UnityEngine;

public abstract class Actionable : MonoBehaviour
{
    public virtual string GetHoverActionText()
    {
        return "";
    }

    public virtual void ExecuteAction(Ingredient ingredient){

    }
}
