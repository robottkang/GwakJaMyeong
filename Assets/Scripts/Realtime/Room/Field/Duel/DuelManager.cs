using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;
using Cysharp.Threading.Tasks;
using Card;
using System;

namespace Room
{
    public class DuelManager : MonoBehaviour, IOnEventCallback
    {
        [SerializeField]
        private List<Sprite> cardSprites = new();
        private static bool receivesOpponentCardDepolyment;
        private static UserType actionToken = UserType.Player;
        public static UnityEvent<UserType> OnActionTokenChanged { get; } = new();
        public static bool IsInDuel { get; private set; } = false;
        public static int StrategyPlanOrder { get; private set; } = 0;
        public static Sprite[] CardSprites { get; private set; }
        public static UserType ActionToken
        {
            get => actionToken;
            private set
            {
                OnActionTokenChanged.Invoke(value);
                actionToken = value;
            }
        }

        private static System.Threading.CancellationTokenSource ctsOnDestory = new();

        private void Awake()
        {
            CardSprites = cardSprites.ToArray();
            ctsOnDestory = new();

            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
            ctsOnDestory.Cancel();
            ctsOnDestory.Dispose();
        }

        public static void ResetStrategyPlans()
        {
            StrategyPlanOrder = 0;
            Opponent.OpponentController.Instance.IsReadyPlanCard = false;
            PlayerController.Instance.IsReadyPlanCard = false;
        }

        public static void SetPlayerActionToken(UserType actionToken)
        {
            // opponent
            PhotonNetwork.RaiseEvent((byte)DuelEventCode.SetActionToken,
                actionToken ^ UserType.Opponent,
                new RaiseEventOptions { Receivers = ReceiverGroup.Others },
                SendOptions.SendReliable);
            // me
            ActionToken = actionToken;
        }

        public static void SwapActionToken()
        {
            // opponent
            PhotonNetwork.RaiseEvent((byte)DuelEventCode.SetActionToken, 
                ActionToken, 
                new RaiseEventOptions { Receivers = ReceiverGroup.Others }, 
                SendOptions.SendReliable);
            // me
            ActionToken ^= UserType.Opponent;
        }

        public static async UniTask StartDuel()
        {
            IsInDuel = true;
            for (int i = 0; i < 3; i++)
            {
                receivesOpponentCardDepolyment = false;

                await UniTask.WaitUntil(() => ActionToken == UserType.Player, cancellationToken: ctsOnDestory.Token);

                PlayerController.Instance.PlanCardController.SelectDeployment(PlayerController.Instance.CurrentCard);
                await UniTask.WaitUntil(() => !PlayerController.Instance.PlanCardController.IsSelecting
                && receivesOpponentCardDepolyment && !FieldController.IsChangingAnyPosture, cancellationToken: ctsOnDestory.Token);
                
                if (ActionToken == UserType.Player)
                    await CalcaulateCard(PlayerController.Instance.CurrentCard, Opponent.OpponentController.Instance.CurrentCard);
                else
                    await CalcaulateCard(Opponent.OpponentController.Instance.CurrentCard, PlayerController.Instance.CurrentCard);

                StrategyPlanOrder++;
            }
            SwapActionToken();
            IsInDuel = false;
        }

        private async static UniTask CalcaulateCard(PlanCard attacker, PlanCard defender)
        {
            Debug.Log("Start Card Calc");

            List<Action> cardActionOrderList = new()
            {
                attacker.onCardOpen, defender.onCardOpen,
                attacker.onCardSumStart, defender.onCardSumStart,
                attacker.onCardSum, defender.onCardSumEnd
            };
            foreach (var cardAction in cardActionOrderList)
            {
                cardAction?.Invoke();
                await UniTask.WaitUntil(() => !FieldController.IsChangingAnyPosture, cancellationToken: ctsOnDestory.Token);
            }
        }

        public static void EndGame(FieldController loser)
        {
            if (loser is PlayerController)
            {
                // Display lose
                Debug.Log("You Lose");
            }
            else
            {
                // Display win
                Debug.Log("You Win");
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (byte)DuelEventCode.SetActionToken)
            {
                ActionToken = (UserType)photonEvent.CustomData;
            }
            if (photonEvent.Code == (byte)DuelEventCode.SetCardDepolyment)
            {
                receivesOpponentCardDepolyment = true;
            }
        }
    }

    public enum UserType
    {
        Player,
        Opponent
    }

    public enum DuelEventCode
    {
        Ready,
        SetActionToken,
        SendCardsData,
        SendPosture,
        SetCardDepolyment,
        TakeDamage,
    }
}
