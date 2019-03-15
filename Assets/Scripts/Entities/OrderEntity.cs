using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace TrainJam.Multiplayer.Entities
{
    /// <summary>
    /// Represents an order icon. When it is hovered over, shows the order that is required
    ///
    /// The first text is the image to use for the order. The second through last texts are used as the ingredients. The
    /// first value is the amount of time until this order should do a warn shake and turn red. The second value is when
    /// the order has failed.
    ///
    /// TODO: Times
    /// </summary>
    public class OrderEntity : EntityBehaviour
    {
        [SerializeField] private Image m_MasterIcon;
        [SerializeField] private Image m_IngredientIconPrefab;
        [SerializeField] private RectTransform m_IngredientIconParent;

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

            // Parent to horizontal layout group, also marks the layout for rebuilding.
            transform.SetParent(GameController.instance.OrdersHorizontalLayoutGroup, false);
        }
    }
}