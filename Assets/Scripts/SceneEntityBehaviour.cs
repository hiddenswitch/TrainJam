using UnityEngine;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("m_EntityId")] [SerializeField]
        protected string m_SceneId;

        internal override string canonicalName => m_SceneId;
        internal override bool instantiateOnAdded => false;
        internal override bool destroyOnRemoved => false;

        protected override void Awake()
        {
            m_Instances[m_SceneId] = this;
        }

        protected override void OnEnable()
        {
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();
            if (entityId != null && m_Instances.ContainsKey(entityId))
            {
                m_Instances.Remove(entityId);
            }

            entity = null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (m_Instances.ContainsKey(m_SceneId))
            {
                m_Instances.Remove(m_SceneId);
            }
        }
    }
}