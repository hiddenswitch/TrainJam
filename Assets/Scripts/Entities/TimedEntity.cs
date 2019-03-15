using System;
using UniRx;

namespace TrainJam.Multiplayer.Entities
{
    public abstract class TimedEntity : SceneEntityBehaviour
    {
        private DateTime m_StartTime;
        private CompositeDisposable m_Disposable = new CompositeDisposable();
        public float durationSeconds => entity?.value ?? 0f;

        protected override void OnAdded(EntityDocument entity, int localPlayerId)
        {
            m_Disposable.Dispose();
            m_Disposable = new CompositeDisposable();
            m_StartTime = DateTime.Now;
            OnTick(TimeSpan.FromSeconds(durationSeconds));
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

                    OnTick(remaining);
                })
                .AddTo(m_Disposable);
        }

        protected override void OnRemoved()
        {
            m_Disposable.Dispose();
        }


        protected virtual void OnElapsed()
        {
        }

        protected virtual void OnTick(TimeSpan remaining)
        {
        }
    }
}