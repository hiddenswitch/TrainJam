using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TrainJam.Multiplayer.Entities
{
    /// <summary>
    /// Represents the game timer.
    ///
    /// The entity's first value is the duration in seconds.
    /// </summary>
    public class TimeLeftEntity : SceneEntityBehaviour
    {
        [SerializeField] private Text m_TimerText;
        [SerializeField] private string m_TimeFormat = "mm\\:ss";
        private DateTime m_StartTime;
        private CompositeDisposable m_Disposable = new CompositeDisposable();
        public float durationSeconds => entity?.value ?? 0f;

        protected override void OnAdded(EntityDocument entity, int localPlayerId)
        {
            m_Disposable.Dispose();
            m_Disposable = new CompositeDisposable();
            m_StartTime = DateTime.Now;
            Observable.ReturnUnit().ContinueWith(Observable.Interval(TimeSpan.FromSeconds(1)))
                .Subscribe(ignored =>
                {
                    var elapsed = (DateTime.Now - m_StartTime);
                    var remaining = TimeSpan.FromSeconds(durationSeconds) - elapsed;
                    if (remaining <= TimeSpan.Zero)
                    {
                        OnElapsed();
                        remaining = TimeSpan.Zero;
                        // Cancels the timer
                        m_Disposable.Dispose();
                    }

                    m_TimerText.text = remaining.ToString(m_TimeFormat);
                })
                .AddTo(m_Disposable);
        }

        protected override void OnRemoved()
        {
            m_Disposable.Dispose();
        }

        protected virtual void OnElapsed()
        {
            // TODO: Do any transitions or anything?
        }
    }
}