using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;

namespace Room
{
    public class DuelManager : MonoBehaviour, IOnEventCallback
    {
        public static int StrategyPlanOrder { get; private set; }
        public static bool HasActionToken { get; private set; } = false;
        private static UnityEvent onSumStart = new();

        private void Awake()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public static void ResetStrategyPlanOrder()
        {
            if (HasActionToken)
                StrategyPlanOrder = -1;
            else
                StrategyPlanOrder = 0;
        }

        public static void SubscribeOnSumStart(UnityAction action)
        {
            onSumStart.AddListener(action);
        }

        public static void SwapActionToken()
        {
            // opponent
            PhotonNetwork.RaiseEvent((byte)DuelEventCode.SetActionToken, 
                HasActionToken, 
                new RaiseEventOptions { Receivers = ReceiverGroup.Others }, 
                SendOptions.SendReliable);
            // me
            HasActionToken = !HasActionToken;
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (byte)DuelEventCode.SetActionToken)
            {
                HasActionToken = (bool)photonEvent.CustomData;
            }
        }
    }

    public enum DuelEventCode
    {
        Ready,
        SetActionToken,
        SendCardInfos,
        SendMyPosture,
        SendOpponentPosture,
        SendCardDepolyment,
        TakeDamage,
    }
}
