using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField]
    private bool enabledDebug = false;
    public bool EnabledDebug => enabledDebug;

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
            EditorApplication.isPlaying = false;
#else
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
#endif
    }
}
