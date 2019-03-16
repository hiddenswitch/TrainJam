using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam.Multiplayer
{
    public sealed class InSceneEntityPrefabs : UIBehaviour
    {
        public static InSceneEntityPrefabs instance { get; private set; }
        [SerializeField] private EntityBehaviour[] m_EntityBehaviours;
        private Dictionary<string, EntityBehaviour> m_Dictionary = new Dictionary<string, EntityBehaviour>();

        public static IReadOnlyDictionary<string, EntityBehaviour> prefabs => instance.m_Dictionary;

        protected override void Awake()
        {
            instance = this;
            m_Dictionary = m_EntityBehaviours.ToDictionary(eb => eb.gameObject.name, eb => eb);
        }
    }
}