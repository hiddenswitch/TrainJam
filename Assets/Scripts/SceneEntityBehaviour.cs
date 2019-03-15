using UnityEngine;

namespace TrainJam.Multiplayer
{
    /// <summary>
    /// An entity that already exists in the scene and should not be instantiated nor destroyed.
    ///
    /// Ignores the instantiate on added and destroy on removed flags.
    ///
    /// Receives the normal callbacks from the server.
    /// </summary>
    public abstract class SceneEntityBehaviour : EntityBehaviour
    {
        [SerializeField] protected string m_EntityId;
        internal override string entityId => $"{GameController.instance.matchId ?? ""}/{m_EntityId}";
        internal override bool instantiateOnAdded => false;
        internal override bool destroyOnRemoved => false;

        protected override void OnEnable()
        {
            if (GameController.instance?.matchId == null)
            {
                return;
            }
            base.OnEnable();
        }
    }
}