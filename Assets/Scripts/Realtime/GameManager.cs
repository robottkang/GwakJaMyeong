using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEditor;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField]
    private bool enabledDebug = false;
    [SerializeField]
    private bool isSingleMode = false;
    public bool EnabledDebug => enabledDebug;
    public bool IsSingleMode => isSingleMode;

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

public enum UserType
{
    Player,
    Opponent
}

public static class UserTypeExtension
{
    public static UserType GetOpponentUserType(this UserType userType)
    {
        return userType ^ (UserType.Player | UserType.Opponent);
    }
}

public enum DuelEventCode
{
    Ready,
    SetActionToken,
    RequestCardData,
    SendCardData,
    SendPosture,
    SetCardDepolyment,
    CompleteCardAction,
    TakeDamage,
}
