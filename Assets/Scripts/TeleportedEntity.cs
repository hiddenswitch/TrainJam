using System;
using UnityEngine;

namespace TrainJam.Multiplayer
{
    public sealed class TeleportedEntity : EntityBehaviour
    {
        public string ingredientName => entity?.texts[0] ?? null;

        protected override void OnAdded(EntityDocument entity, int localPlayerId)
        {
            if (ingredientName == null)
            {
                Debug.LogError($"TeleportedEntity: {nameof(ingredientName)} was null.");
                return;
            }

            var parsed = Enum.TryParse(ingredientName, out IngredientType ingredientType);
            if (!parsed)
            {
                Debug.LogError($"TeleportedEntity: Failed to parse {ingredientName}.");
                return;
            }

            FindObjectOfType<SpawnManager>().SpawnIngredientAtSpawnPosition(ingredientType);
        }
    }
}