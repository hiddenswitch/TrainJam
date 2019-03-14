using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using DG.Tweening;

public class Ingredients : Interactable, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public CanvasGroup canvasGroup;
    public TMPro.TextMeshProUGUI nameText;
    public Image progressBar;
    float progress = 0.0f;
    public float cutSpeed = 0.3f;

    bool cutDone = false;

    bool isHolding = false;
    PointerEventData lastEvenData;

    bool panDone = false;

    bool isAnimating = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    void Start()
    {
        Observable.EveryUpdate().Subscribe(_ =>
        {
            if (isHolding)
            {
                if (!cutDone)
                {
                    OnCut();
                }else{
                    OnPan();
                }
            }else
            {
                OnPan();
            }
        });


        this.OnBeginDragAsObservable().Subscribe(pointer =>
        {
            this.canvasGroup.blocksRaycasts = false;
            isHolding = true;
        }).AddTo(this);

        this.OnDragAsObservable().Subscribe(pointer =>
        {
            this.transform.SetAsLastSibling();
            this.gameObject.transform.position = pointer.position;
            isHolding = true;
            lastEvenData = pointer;


        }).AddTo(this);

        this.OnEndDragAsObservable().Subscribe(pointer => 
        {
            isHolding = false;
            var hitObject = pointer.pointerCurrentRaycast.gameObject;
            if (hitObject)
            {
                if (hitObject.GetComponent<TrashBin>() || hitObject.transform.parent.GetComponent<TrashBin>())
                {
                    Destroy(this.gameObject);
                }
            }

            this.canvasGroup.blocksRaycasts = true;
        }).AddTo(this);
    }

    void FinishCut(){
        cutDone = true;
        nameText.text = "CUT MEAT";
        //progressBar.gameObject.SetActive(false);
        progressBar.fillAmount = 0;
        progress = 0;
    }

    void OnCut()
    {
        if (!isAnimating)
        {
            transform.DOScale(new Vector3(0.9f, 0.9f, 1f), 0.3f).SetLoops(-1, LoopType.Yoyo);
            isAnimating = true;
        }
        var hitObject = lastEvenData.pointerCurrentRaycast.gameObject;
        if (hitObject)
        {
            if (hitObject.GetComponent<CuttingBoard>() || hitObject.transform.parent.GetComponent<CuttingBoard>())
            {
                progress += Time.deltaTime * cutSpeed;
                progressBar.fillAmount = progress;
                if (progress > 1)
                {
                    FinishCut();
                }
            }
        }
    }

    void OnPan()
    {
        if (!panDone && cutDone)
        {
            var hitObject = lastEvenData.pointerCurrentRaycast.gameObject;
            if (hitObject)
            {
                if (hitObject.GetComponent<Pan>() || hitObject.transform.parent.GetComponent<Pan>())
                {
                    progress += Time.deltaTime * cutSpeed * 0.2f;
                    progressBar.fillAmount = progress;
                    if (progress > 1)
                    {
                        FinishPan();
                    }
                }
            }
        }
    }

    void FinishPan(){
        panDone = true;
        nameText.text = "CRAB PEOPLE";
        progressBar.fillAmount = 0;
    }
}
