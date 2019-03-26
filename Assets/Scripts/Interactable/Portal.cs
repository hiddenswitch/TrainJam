using DG.Tweening;

public class Portal : Actionable
{
    public PlayerType targetPlayer;

    public override string GetHoverActionText()
    {
        return $"Send to {targetPlayer.ToString()}"; 
    }

    public void SendIngredient(Ingredient ingredient)
    {
        NetworkManager.Instance.CallSendIngredient(ingredient, targetPlayer);
    }

    public override void ExecuteAction(Ingredient ingredient)
    {
        ingredient.transform.DOMove(this.transform.position, 0.5f);
        ingredient.transform.DOScale(0, 1f).OnComplete(()=> {
            Destroy(ingredient.gameObject);
        });

        if (targetPlayer != PlayerType.Trash)
            this.SendIngredient(ingredient);
    }
}
