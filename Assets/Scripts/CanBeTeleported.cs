using DG.Tweening;
using TrainJam.Multiplayer;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam
{
    [RequireComponent(typeof(Collider))]
    public sealed class CanBeTeleported : UIBehaviour
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

                    var portalToPlayerId = col.gameObject.GetComponent<PortalToPlayerId>();
                    if (!portalToPlayerId)
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

                    var ingredient = GetComponent<Ingredient>();
                    if (ingredient == null)
                    {
                        Debug.LogError($"CanBeTeleported: Missing ingredient script on {gameObject.name}");
                        return;
                    }

                    var prefabToInstantiate = ingredient.type.ToString();
                    Debug.Log($"CanBeTeleported: {prefabToInstantiate}");

                    var sequence = DOTween.Sequence();
                    sequence.Insert(0f, transform.DOScale(0, 1f));
                    sequence.Insert(0f,
                        transform.DOMove(col.gameObject.transform.position + new Vector3(0, 0.5f, 0), 0.5f));

                    sequence.OnComplete(() =>
                    {
                        // Actually portal the object to the other player
                        GameController.instance.Instantiate(portalToPlayerId.toPlayerId, prefabToInstantiate);

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