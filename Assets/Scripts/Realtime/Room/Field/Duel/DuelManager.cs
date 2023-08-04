using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;
using Cysharp.Threading.Tasks;
using Card;

namespace Room
{
    public class DuelManager : MonoBehaviour, IOnEventCallback
    {
        private static bool receivesCardDepolyment;
        public static bool IsInDuel { get; private set; } = false;
        public static int StrategyPlanOrder { get; private set; } = 0;
        public static bool HasActionToken { get; private set; } = false;

        private static System.Threading.CancellationTokenSource ctsOnDestory = new();

        private void Awake()
        {
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

        public static void SetActionToken(bool myActionToken)
        {
            // opponent
            PhotonNetwork.RaiseEvent((byte)DuelEventCode.SetActionToken,
                !myActionToken,
                new RaiseEventOptions { Receivers = ReceiverGroup.Others },
                SendOptions.SendReliable);
            // me
            HasActionToken = myActionToken;
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

        public static void StartDuel() => UniTask.Void(async (token) =>
        {
            IsInDuel = true;
            for (int i = 0; i < 3; i++)
            {
                receivesCardDepolyment = false;

                await UniTask.WaitUntil(() => HasActionToken, cancellationToken: token);

                PlayerController.Instance.PlanCardController.SelectDeployment(PlayerController.Instance.CurrentCard);
                await UniTask.WaitUntil(() => !PlayerController.Instance.PlanCardController.IsSelecting
                && receivesCardDepolyment && !FieldController.IsChangingAnyPosture, cancellationToken: token);
                
                if (HasActionToken) await CalcaulateCard(PlayerController.Instance.CurrentCard, Opponent.OpponentController.Instance.CurrentCard);
                else await CalcaulateCard(Opponent.OpponentController.Instance.CurrentCard, PlayerController.Instance.CurrentCard);
                SwapActionToken();
            }
            IsInDuel = false;
        }, ctsOnDestory.Token);

        private async static UniTask CalcaulateCard(PlanCard attacker, PlanCard defender)
        {
            UniTask waitChangingAnyPosture = UniTask.WaitUntil(() => !FieldController.IsChangingAnyPosture, cancellationToken: ctsOnDestory.Token);
            
            Debug.Log("Start Card Calc");
            attacker.onCardOpen.Invoke();
            await waitChangingAnyPosture;
            defender.onCardOpen.Invoke();
            await waitChangingAnyPosture;
            attacker.onCardSumStart.Invoke();
            await waitChangingAnyPosture;
            defender.onCardSumStart.Invoke();
            await waitChangingAnyPosture;
            attacker.onCardSum.Invoke();
            await waitChangingAnyPosture;
            defender.onCardSum.Invoke();
            await waitChangingAnyPosture;

            StrategyPlanOrder++;
        }

        public static void EndGame(FieldController loser)
        {
            if (loser is PlayerController)
            {
                // Display lose
            }
            else
            {
                // Display win
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (byte)DuelEventCode.SetActionToken)
            {
                HasActionToken = (bool)photonEvent.CustomData;
            }
            if (photonEvent.Code == (byte)DuelEventCode.SendCardDepolyment)
            {
                receivesCardDepolyment = true;
            }
        }
    }

    public enum DuelEventCode
    {
        Ready,
        SetActionToken,
        SendCardsData,
        SendMyPosture,
        SendOpponentPosture,
        SendCardDepolyment,
        TakeDamage,
    }
}
