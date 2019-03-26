using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectsManager : Singleton<EffectsManager>
{
    [SerializeField] private Image overlayImage;
    [SerializeField] private Color greenColor;
    [SerializeField] private TextMeshProUGUI ratingText;
    [SerializeField] private float fadeTime = 1f;

    public void Bling(){
        overlayImage.DOPause();
        overlayImage.color = greenColor;
        ratingText.DOPause();
        ratingText.color = Color.white;
        overlayImage.DOFade(0, fadeTime);
        ratingText.DOFade(0, fadeTime);
    }
}
