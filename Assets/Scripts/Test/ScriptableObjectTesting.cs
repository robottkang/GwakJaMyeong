using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//[CreateAssetMenu(fileName = "Test", menuName = "Test")]
public class ScriptableObjectTesting : ScriptableObject
{
    [EasyButtons.Button]
    void TestButton()
    {
        Test.Invoke();
    }
    [SerializeField]
    UnityEvent Test;

    private void TestMethod(int a, int b)
    {
        Debug.Log("Hello World");
        Debug.Log(a + b);
    }

    ScriptableObjectTesting()
    {
        Test = new(); // if you don't initialize, you will get NullReferenceException
        Test.AddListener(() => TestMethod(1, 2));
    }
}
