using UnityEngine;

public class Delivery : Actionable
{
    [SerializeField] private Ticket ticket;

    public override string GetHoverActionText()
    {
        return "Deliver This";
    }

    public override void ExecuteAction(Ingredient ingredient)
    {
        if (ticket.DeliverIngredient(ingredient))
        {
            NetworkManager.Instance.CallBling();
            Destroy(ingredient.gameObject);
        }
    }
}
