using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Room;
using TMPro;

public class PhaseDisplayer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.gameManager;
#if !(UNITY_EDITOR)
        text.gameObject.SetActive(false);
#endif
    }

    void Update()
    {
        text.text = gameManager.CurrentPhase.ToString();
    }
}
