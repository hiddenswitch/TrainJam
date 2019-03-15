using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CuttingBoard : MonoBehaviour
{
    private bool cutting = false;

    public GameObject knife;

    Sequence sequence;
    [SerializeField] private Vector3 m_EndValue;
    [SerializeField] private Image progressBar;

    private void Start()
    {

        sequence = DOTween.Sequence();

        var tween = knife.transform.DOLocalRotate(m_EndValue, 0.1f, RotateMode.Fast);
        sequence.Insert(0f, tween);
        sequence.SetLoops(-1, LoopType.Yoyo);

        //knife.gameObject.SetActive(false);
    }

    public void StartCutting(float progress)
    {
        progressBar.fillAmount = progress;

        if (!cutting)
        {
            //knife.gameObject.SetActive(true);
            sequence.Play();
            cutting = true;
        }
    }

    public void StopCutting()
    {
        progressBar.fillAmount = 0;
        if (cutting)
        {
            //knife.gameObject.SetActive(false);
            cutting = false;
            sequence.Pause();
        }
    }
}