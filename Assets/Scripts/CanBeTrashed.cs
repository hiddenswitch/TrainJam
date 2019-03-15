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
        private ProjectedDragObject dragObject;

        protected override void Start()
        {
            dragObject = GetComponent<ProjectedDragObject>();

            base.Start();

            var used = false;
            this.OnTriggerStayAsObservable()
                .Subscribe(col =>
                {
                    if (used)
                    {
                        return;
                    }

                    if (dragObject && dragObject.dragging)
                    {
                        return;
                    }

                    if (!col.gameObject.GetComponent<Trash>())
                    {
                        return;
                    }

                    used = true;

                    Destroy(dragObject);
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
                        Ingredient ingredient = GetComponent<Ingredient>();
                        if (ingredient)
                        {
                            FindObjectOfType<SpawnManager>().SpawnIngredient(ingredient.type, new Vector3(0.15f, 6, 2.66f));
                        }
                        
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