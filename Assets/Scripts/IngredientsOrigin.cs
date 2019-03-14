using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

public class IngredientsOrigin : Interactable, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler
{
    public GameObject meatPrefab;
    public Canvas parentCanvas;

    public void OnBeginDrag(PointerEventData eventData)
    {
       // print("DRAGGIN " + Time.time);
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //print("END DRAGGING");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var obj = Instantiate(meatPrefab, parentCanvas.transform);
        obj.transform.position = eventData.position;
    }

    // Start is called before the first frame update
    void Start()
    {

        this.OnDragAsObservable().Subscribe(_ => print("DRAGGING")).AddTo(this);
        /*Observable.
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButton(0))
            .Subscribe(_ => print("CLICKED"));*/
    }
}
