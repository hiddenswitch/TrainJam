using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Vector3 offset;
    private float zCoord;

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
        endPosition.y = 1f;
        return endPosition;
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + offset;
    }
}
