using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using MaterialUI;
using Meteor;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TrainJam.Multiplayer.Entities
{
    /// <summary>
    /// Represents an order icon. When it is hovered over, shows the order that is required
    ///
    /// The first text is the image to use for the order. The second through last texts are used as the ingredients. The
    /// first value is the time the order should come into play. The second value is the amount of time until this order
    /// should do a warn shake and turn red. The third value is the amount of time until the order has failed.
    ///
    /// The first boolean indicates whether or not the order is done. The second boolean indicates the order has failed!
    ///
    /// </summary>
    public class OrderEntity : EntityBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private CanvasGroup m_CanvasGroup;
        [SerializeField] private RectTransform m_IngredientIconParent;
        [SerializeField] private Image m_MasterIcon;
        [SerializeField] private Image m_IngredientIconPrefab;
        [SerializeField] private float m_FadeOutDuration = 1f;
        [SerializeField] private RectTransform m_ActiveOrdersParent;
        [SerializeField] private RectTransform m_InactiveOrdersParent;
        private CompositeDisposable m_Timers = new CompositeDisposable();
        [SerializeField] private float m_WarnShakeDuration = 0.8f;
        [SerializeField] private Vector3 m_WarnShakeStrength = Vector2.right * 20;


        public float insertionTime => entity?.values[0] ?? -1;
        public float warningDuration => entity?.values[1] ?? 999;
        public float failureDuration => entity?.values[2] ?? 999;

        public bool finished => entity?.bools[0] ?? false;
        public bool failed => entity?.bools[1] ?? false;

        protected override void Start()
        {
            base.Start();

            // Show the ingredients when we press down on the icon
            this.OnPointerDownAsObservable()
                .Subscribe(ignored => { m_IngredientIconParent.gameObject.SetActive(true); })
                .AddTo(this);

            // Hide the ingredients when we release the icon
            this.OnPointerUpAsObservable()
                .Subscribe(ignored => { m_IngredientIconParent.gameObject.SetActive(false); })
                .AddTo(this);
        }

        protected override void OnAdded(EntityDocument entity, int localPlayerId)
        {
            m_IngredientIconPrefab.gameObject.SetActive(false);
            m_IngredientIconParent.gameObject.SetActive(false);
            if (entity.texts.Length < 1)
            {
                Debug.LogError(
                    $"OrderEntity.OnAdded: Entity with prefab value {entity.prefab} did not receive any texts from the server.");
                return;
            }

            var orderSpriteName = entity.texts[0];
            if (!Sprites.sprites.ContainsKey(orderSpriteName))
            {
                Debug.LogError($"Missing sprite {orderSpriteName}");
                return;
            }

            var orderIcon = Sprites.sprites[orderSpriteName];

            m_MasterIcon.sprite = orderIcon;
            var aspectRatioFitter = m_MasterIcon.GetComponent<AspectRatioFitter>();
            if (aspectRatioFitter != null)
            {
                // This sets it to be dirty
                aspectRatioFitter.aspectRatio = orderIcon.rect.width / orderIcon.rect.height;
            }

            foreach (var ingredientSpriteName in entity.texts.Skip(1))
            {
                var ingredientImage = Instantiate(m_IngredientIconPrefab, m_IngredientIconParent);
                ingredientImage.gameObject.SetActive(true);
                if (!Sprites.sprites.ContainsKey(ingredientSpriteName))
                {
                    Debug.LogError($"Missing sprite {ingredientSpriteName}");
                    return;
                }

                var ingredientImageSprite = Sprites.sprites[ingredientSpriteName];
                ingredientImage.sprite = ingredientImageSprite;
                var ingredientFitter = ingredientImage.GetComponent<AspectRatioFitter>();
                if (ingredientFitter != null)
                {
                    ingredientFitter.aspectRatio = ingredientImageSprite.rect.width / ingredientImageSprite.rect.height;
                }
            }

            // Move to inactive
            transform.SetParent(m_InactiveOrdersParent, false);
//            m_Timers?.Dispose();
            // For now, just schedule a timer
            Observable.Timer(TimeSpan.FromSeconds(insertionTime))
                .Subscribe(v1 =>
                {
                    // Move to active
                    transform.SetParent(m_ActiveOrdersParent, false);
                    transform.SetAsLastSibling();
                    // Start warning timer
                    Observable.Timer(TimeSpan.FromSeconds(warningDuration))
                        .Subscribe(v2 =>
                        {
                            if (!finished)
                            {
                                transform.DOShakePosition(m_WarnShakeDuration, m_WarnShakeStrength);
                            }
                        })
                        .AddTo(m_Timers)
                        .AddTo(this);

                    Observable.Timer(TimeSpan.FromSeconds(failureDuration))
                        .Subscribe(v2 =>
                        {
                            if (!finished)
                            {
                                // TODO: Fail!
                                MoveToInactive();
                            }
                        })
                        .AddTo(m_Timers)
                        .AddTo(this);
                })
                .AddTo(m_Timers)
                .AddTo(this);
        }

        protected override void OnBooleansSet(bool[] values)
        {
            var finished = values[0];
            var failed = values[1];

            if (finished || failed)
            {
                // TODO: Do something special if the user failed an order
                if (failed)
                {
                    ToastManager.Show("Order failed!", 3f);                    
                }
                
                m_Timers?.Dispose();

                // Move back to inactive
                MoveToInactive();
            }
        }

        private void MoveToInactive()
        {
            // Already located in inactive
            if (transform.parent == m_InactiveOrdersParent)
            {
                return;
            }

            m_CanvasGroup.DOFade(0f, m_FadeOutDuration)
                .OnComplete(() => { transform.SetParent(m_InactiveOrdersParent); });
        }

        /// <summary>
        /// Finishes the order in the server and propagates the side effects
        /// </summary>
        public void FinishOrder()
        {
            GameController.instance.SetBool(entityId, 0, true);
        }

        /// <summary>
        /// Fails the order on the server and propagates the side effects
        /// </summary>
        public void Fail()
        {
            GameController.instance.SetBool(entityId, 1, true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnPointerUp(PointerEventData eventData)
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            
        }
    }
}