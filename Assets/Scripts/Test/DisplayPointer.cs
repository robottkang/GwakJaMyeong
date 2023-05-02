using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPointer : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    private GameObject pointerPrefab;
    
    private void Start()
    {
        pointerPrefab = Instantiate(pointerPrefab);
    }

    private void Update()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit);
        pointerPrefab.transform.position = hit.point;
    }
#endif
}
