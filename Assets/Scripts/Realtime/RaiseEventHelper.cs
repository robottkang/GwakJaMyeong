using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RaiseEventHelper
{
    public static bool RaiseDuelEvent<T>(this RaiseEventData<T> content, DuelEventCode eventCode)
    {
        return PhotonNetwork.RaiseEvent((byte)eventCode,
            JsonUtility.ToJson(content),
            RaiseEventOptions.Default,
            SendOptions.SendReliable);
    }

    public static RaiseEventData<T> ConvertEventData<T>(this EventData eventData)
    {
        return JsonUtility.FromJson<RaiseEventData<T>>((string)eventData.CustomData);
    }

    public static bool AreEventCodesEqual(this EventData eventData, DuelEventCode eventCode)
    {
        return eventData.Code == (byte)eventCode;
    }
}

[Serializable]
public class RaiseEventData<T>
{
    private RaiseEventData() { }
    public RaiseEventData(UserType targetUser, T content)
    {
        this.targetUser = targetUser;
        if (typeof(T).IsSerializable)
            this.content = content;
        else
            throw new ArgumentException($"{content} is not serializable");
    }

    /// <summary>
    /// 송신자 입장의 targetUser
    /// </summary>
    [SerializeField]
    private UserType targetUser;
    /// <summary>
    /// 수신자 입장의 targetUser
    /// </summary>
    public UserType TargetUser => targetUser.GetOpponentUserType();
    public T content;
}
