using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Vector3 offset;
    private float zCoord;
    public Rigidbody rb;
    private Vector3 lastTouchPos;
    private Vector3 dir;
    private Vector3 lastPosition;
    bool dragging = false;
    public float force = 2.0f;

    private void OnMouseDown()
    {
        zCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        offset = gameObject.transform.position - GetMouseWorldPos();
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;
        Vector3 endPosition = Camera.main.ScreenToWorldPoint(mousePoint);
        endPosition.y = 10f;
        return endPosition;
    }

    private void OnMouseDrag()
    {
        dragging = true;
        lastTouchPos = Input.mousePosition;
        //lastTouchPos = GetMouseWorldPos() + offset;
        Vector3 pos = GetMouseWorldPos() + offset;
        pos.y = 0.4f;
        transform.position = pos;

        dir = (transform.position - lastPosition).normalized;
        lastPosition = transform.position;
    }

    private void OnMouseExit()
    {
        if (dragging){
           // dragging = false;

        }

    }

    private void FixedUpdate()
    {
        if (dragging)
        {
            if (!Input.GetMouseButton(0))
            {
                dragging = false;
                print(dir);
                rb.velocity = dir * force;
            }
        }
    }
}
