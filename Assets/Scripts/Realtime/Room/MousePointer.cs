using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePointer : MonoBehaviour
{
    [SerializeField]
    private GameObject pointer;
    
    private void Start()
    {
        pointer = Instantiate(pointer);
    }

    private void Update()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity, LayerMask.GetMask("Board"));
        pointer.transform.position = hit.point;
    }
}
