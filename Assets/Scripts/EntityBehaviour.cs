using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam.Multiplayer
{
    public abstract class EntityBehaviour : UIBehaviour
    {
        private static Dictionary<string, EntityBehaviour> m_Prefabs =
            new Dictionary<string, EntityBehaviour>();

        protected static Dictionary<string, EntityBehaviour> m_Instances = new Dictionary<string, EntityBehaviour>();

        [RuntimeInitializeOnLoadMethod]
        protected static void EntityBehaviourInitialize()
        {
            m_Prefabs = Resources.FindObjectsOfTypeAll<EntityBehaviour>()
                .ToDictionary(eb => eb.canonicalName, eb => eb);
        }

        [SerializeField] protected bool m_InstantiateOnAdded = true;
        [SerializeField] protected bool m_DestroyOnRemoved = true;

        /// <summary>
        /// Prefabs by name as specified in <see cref="canonicalName"/> (defaults to the game object's name).
        /// </summary>
        public static IReadOnlyDictionary<string, EntityBehaviour> prefabs => m_Prefabs;

        /// <summary>
        /// Instances by entity document ID
        /// </summary>
        public static IReadOnlyDictionary<string, EntityBehaviour> instances => m_Instances;

        protected override void OnEnable()
        {
            base.OnEnable();
            OnInstantiated();
        }

        internal virtual void OnInstantiated()
        {
            if (entityId != null)
            {
                m_Instances[entityId] = this;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (entityId != null && m_Instances.ContainsKey(entityId))
            {
                m_Instances.Remove(entityId);
            }
        }

        public EntityDocument entity { get; internal set; }
        public int localPlayerId { get; internal set; }
        internal virtual string canonicalName => gameObject.name;
        internal virtual bool instantiateOnAdded => m_InstantiateOnAdded;
        internal virtual bool destroyOnRemoved => m_DestroyOnRemoved;
        internal virtual string entityId => entity?._id;
        protected bool isLocalPlayer => entity?.playerIds.Contains(localPlayerId) ?? false;

        internal void OnAddedInternal(EntityDocument entity, int localPlayerId)
        {
            this.localPlayerId = localPlayerId;
            this.entity = entity;
            OnAdded(entity, localPlayerId);
            OnUpdateInternal(entity, localPlayerId);
        }

        internal void OnUpdateInternal(EntityDocument entity, int localPlayerId)
        {
            if (isLocalPlayer)
            {
                OnPlayerIdSetToLocal();
            }

            OnPlayerIdSet(isLocalPlayer ? localPlayerId : entity.playerId);
            OnPlayerIdsSet(entity.playerIds);
            OnValueSet(entity.value);
            OnValuesSet(entity.values);
            OnTextSet(entity.text);
            OnTextsSet(entity.texts);
            OnBooleanSet(entity.boolValue);
            OnBooleansSet(entity.bools);
        }

        internal void OnChangedInternal(EntityDocument newEntity, IDictionary changes, string[] changedFields)
        {
            entity = newEntity;
            OnChanged(newEntity, changes, changedFields);
            OnUpdateInternal(newEntity, localPlayerId);
        }

        protected virtual void OnChanged(EntityDocument newEntity, IDictionary changes, string[] changedFields)
        {
        }

        internal void OnRemovedInternal()
        {
            OnRemoved();
        }

        protected virtual void OnRemoved()
        {
        }

        protected virtual void OnAdded(EntityDocument entity, int localPlayerId)
        {
            
        }

        protected virtual void OnValueSet(float newValue)
        {
        }

        protected virtual void OnValuesSet(float[] newValues)
        {
        }

        protected virtual void OnTextSet(string text)
        {
        }

        protected virtual void OnTextsSet(string[] text)
        {
        }

        protected virtual void OnPlayerIdSetToLocal()
        {
        }

        protected virtual void OnPlayerIdSet(int playerId)
        {
        }

        protected virtual void OnPlayerIdsSet(int[] playerIds)
        {
        }

        protected virtual void OnBooleanSet(bool newValue)
        {
        }

        protected virtual void OnBooleansSet(bool[] newValues)
        {
        }
    }
}