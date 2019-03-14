using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Cutting : MonoBehaviour
{
    private bool cutting = false;

    public GameObject knife;

    Sequence sequence;

    private void Start()
    {
        sequence = DOTween.Sequence();

        var tween = knife.transform.DOLocalRotate(new Vector3(25, 180, 0), 0.1f, RotateMode.Fast);
        sequence.Insert(0f, tween);
        sequence.SetLoops(-1, LoopType.Yoyo);

        //knife.gameObject.SetActive(false);

    }

    public void StartCutting()
    {
        if (!cutting)
        {
            //knife.gameObject.SetActive(true);
            sequence.Play();
            cutting = true;
        }
    }

    public void StopCutting()
    {
        if (cutting)
        {
            //knife.gameObject.SetActive(false);
            cutting = false;
            sequence.Pause();
        }
    }
}
