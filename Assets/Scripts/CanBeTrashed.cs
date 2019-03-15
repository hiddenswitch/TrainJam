using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam
{
    [RequireComponent(typeof(Collider))]
    public sealed class CanBeTrashed : UIBehaviour
    {
        protected override void Start()
        {
            base.Start();

            var used = false;
            this.OnTriggerEnterAsObservable()
                .Subscribe(col =>
                {
                    if (used)
                    {
                        return;
                    }

                    used = true;
                    // Check if we're triggering a trash
                    if (!col.gameObject.GetComponent<Trash>())
                    {
                        return;
                    }

                    // Set me to kinematic just in case
                    var rigidbody = GetComponent<Rigidbody>();
                    if (rigidbody)
                    {
                        rigidbody.isKinematic = true;
                    }

                    var sequence = DOTween.Sequence();
                    sequence.Insert(0f, transform.DOScale(0, 1f));
                    sequence.Insert(0f,
                        transform.DOMove(col.gameObject.transform.position + new Vector3(0, 0.5f, 0), 0.5f));

                    sequence.OnComplete(() =>
                    {
                        FindObjectOfType<SpawnManager>().SpawnPrefab1(new Vector3(0, 6, 0));
                        if (gameObject != null)
                        {
                            Destroy(gameObject);                            
                        }
                    });

                    sequence.Play();
                })
                .AddTo(this);

            this.OnTriggerExitAsObservable()
                .Subscribe(ignored => { used = false; })
                .AddTo(this);
        }
    }
}