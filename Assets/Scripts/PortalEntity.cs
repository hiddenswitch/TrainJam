using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam.Multiplayer
{
    /// <summary>
    /// values [0] whom I'm going to send to
    /// </summary>
    public sealed class Portal : UIBehaviour
    {
        [SerializeField] private int m_ToPlayerId;

        public void Teleport(IngredientType ingredientType)
        {
            GameController.instance.Instantiate(m_ToPlayerId, ingredientType.ToString());
        }
    }
}