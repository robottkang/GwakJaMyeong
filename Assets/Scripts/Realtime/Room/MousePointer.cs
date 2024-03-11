using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePointer : MonoBehaviour
{
    private void Update()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity, LayerMask.GetMask("Board"));
        gameObject.transform.position = hit.point;
    }
}
