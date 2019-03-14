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
    private float magnitude;
    bool dragging = false;
    public float force = 2.0f;
    Cutting lastCutting;
    private float grabHeight = 0.2f;

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
        return endPosition;
    }

    private void OnMouseDrag()
    {
        dragging = true;
        lastTouchPos = Input.mousePosition;
        //lastTouchPos = GetMouseWorldPos() + offset;
        Vector3 pos = GetMouseWorldPos() + offset;
        pos.y = grabHeight;
        transform.position = pos;

        transform.eulerAngles = new Vector3(0, 180, 0);
        dir = (transform.position - lastPosition).normalized;
        magnitude = (transform.position - lastPosition).magnitude;
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
                rb.velocity = dir * magnitude * force;
            }

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                Cutting cutting = hit.collider.gameObject.GetComponent<Cutting>();
                if (cutting){
                    lastCutting = cutting;
                    lastCutting.StartCutting();
                }else
                {
                    if (lastCutting)
                    lastCutting.StopCutting();
                }
                //print(hit.collider.gameObject.name);
            }
        }else
        {
            //if (lastCutting)
            //    lastCutting.StopCutting();
        }
    }
}
