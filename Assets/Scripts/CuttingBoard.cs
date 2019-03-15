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
    [SerializeField] private GameObject cantObject;

    public GameObject successEffect;

    public Transform effectPosition;

    private void Start()
    {
        cantObject.SetActive(false);

        sequence = DOTween.Sequence();

        var tween = knife.transform.DOLocalRotate(m_EndValue, 0.1f, RotateMode.Fast);
        sequence.Insert(0f, tween);
        sequence.SetLoops(-1, LoopType.Yoyo);

        //knife.gameObject.SetActive(false);
    }

    public void StartCutting(float progress, bool alreadyFinished)
    {
        if (!alreadyFinished)
        {
            progressBar.fillAmount = progress;
        }else
        {
            cantObject.SetActive(true);
        }

        if (!alreadyFinished && progress >= 1)
        {
            progressBar.fillAmount = 0;
            var obj = Instantiate(successEffect, effectPosition.position, Quaternion.identity);
            obj.SetActive(true);
            sequence.Pause();
        }

        if (!cutting && !alreadyFinished)
        {
            //knife.gameObject.SetActive(true);
            sequence.Play();
            cutting = true;
        }
    }

    public void StopCutting()
    {
        cantObject.SetActive(false);
        progressBar.fillAmount = 0;
        if (cutting)
        {
            //knife.gameObject.SetActive(false);
            cutting = false;
            sequence.Pause();
        }
    }
}