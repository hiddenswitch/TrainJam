using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam
{
    /// <summary>
    /// Whenever a new instance of this game object is enabled, destroys itself if its the oldest version after the
    /// count is exceeded.
    /// </summary>
    public sealed class LimitsCountOfSelf : UIBehaviour
    {
        private static readonly IDictionary<string, IList<LimitsCountOfSelf>> m_Cache =
            new Dictionary<string, IList<LimitsCountOfSelf>>();

        [SerializeField] private string m_Key;
        [SerializeField] private int m_MaxCount;
        [SerializeField] private float m_RemoveDelay;

        protected override void Awake()
        {
            if (string.IsNullOrEmpty(m_Key))
            {
                Debug.LogError($"Awake: key is null on ${gameObject.name}");
                return;
            }

            if (m_MaxCount <= 0)
            {
                Debug.LogError($"Awake: max count <= 0 on ${gameObject.name}");
                return;
            }

            if (!m_Cache.ContainsKey(m_Key))
            {
                m_Cache[m_Key] = new List<LimitsCountOfSelf>();
            }

            var list = m_Cache[m_Key];
            list.Add(this);
            if (list.Count > m_MaxCount)
            {
                var toRemove = list[0].gameObject;
                list.RemoveAt(0);
                Observable.Timer(TimeSpan.FromSeconds(m_RemoveDelay))
                    .Where(ignored => toRemove != null)
                    .Subscribe(ignored => { Destroy(toRemove); })
                    .AddTo(this);
            }

            // In case something else removes this, remove it from the cache
            this.OnDestroyAsObservable()
                .Subscribe(ignored => { list.Remove(this); })
                .AddTo(this);
        }
    }
}