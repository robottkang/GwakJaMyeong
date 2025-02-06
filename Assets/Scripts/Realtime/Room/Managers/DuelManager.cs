using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;
using Cysharp.Threading.Tasks;
using Card;
using Room.UI;

namespace Room
{
    public class DuelManager : MonoBehaviour, IOnEventCallback
    {
        public static DuelManager Instance { get; private set; }

        private bool hasOpponentCardActionCompleted = false;
        [SerializeField]
        private Timer uiTimer = null;
        [field:SerializeField]
        public UnityEvent OnWin { get; private set; } = new();
        [field:SerializeField]
        public UnityEvent OnLose { get; private set; } = new();
        private static UserType actionToken = UserType.Player;
        public static UnityEvent<UserType> OnActionTokenChanged { get; } = new();
        public static bool IsInDuel { get; private set; } = false;
        public static int StrategyPlanOrder { get; private set; } = 0;
        public static UserType ActionToken
        {
            get => actionToken;
            private set
            {
                OnActionTokenChanged.Invoke(value);
                actionToken = value;
            }
        }

        private System.Threading.CancellationTokenSource ctsOnDestory = new();

        private void Awake()
        {
            Instance = this;
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
                actionToken.GetOpponentUserType(),
                RaiseEventOptions.Default,
                SendOptions.SendReliable);
            // player
            ActionToken = actionToken;
        }

        public static void TurnOverActionToken()
        {
            SetPlayerActionToken(ActionToken.GetOpponentUserType());
        }

        private void SwapActionToken()
        {
            if (PhotonNetwork.IsMasterClient) TurnOverActionToken();
        }

        public async UniTask StartDuel()
        {
            IsInDuel = true;
            for (int i = 0; i < 3; i++)
            {
                await UniTask.WaitUntil(() => ActionToken == UserType.Player, cancellationToken: ctsOnDestory.Token);

                // Select deployment
                PlayerController.Instance.PlanCardController.SelectDeployment(PlayerController.Instance.CurrentCard);

                // Wait until player select deployment
                await UniTask.WaitUntil(() => !PlayerController.Instance.PlanCardController.IsSelecting &&
                !FieldController.IsChangingAnyPosture, cancellationToken: ctsOnDestory.Token);
                
                CompleteCardAction();

                // Wait until opponent card action completed
                await UniTask.WaitUntil(() => hasOpponentCardActionCompleted, cancellationToken: ctsOnDestory.Token);

                if (ActionToken == UserType.Player)
                    await CalcaulateCard(PlayerController.Instance.CurrentCard, Opponent.OpponentController.Instance.CurrentCard);
                else
                    await CalcaulateCard(Opponent.OpponentController.Instance.CurrentCard, PlayerController.Instance.CurrentCard);

                StrategyPlanOrder++;
                hasOpponentCardActionCompleted = false;
            }

            SwapActionToken();

            IsInDuel = false;
        }

        private async UniTask CalcaulateCard(PlanCard attacker, PlanCard defender)
        {
            Debug.Log($"{attacker.OnwerFieldCtrl}: {attacker.CardData.ThisCardCode}\n{defender.OnwerFieldCtrl}: {defender.CardData.ThisCardCode}");

            UniTask[] cardActionOrder = new UniTask[]
            {
                attacker.SumStart(),
                defender.SumStart(),
                attacker.Sum(),
                defender.Sum(),
                attacker.SumEnd(),
                defender.SumEnd()
            };

            foreach (var action in cardActionOrder)
            {
                await action;
                await UniTask.WaitUntil(() => !FieldController.IsChangingAnyPosture, cancellationToken: ctsOnDestory.Token);
            }
        }

        private void CompleteCardAction()
        {
            TurnOverActionToken();

            new RaiseEventData<bool>(UserType.Player, true).RaiseDuelEvent(DuelEventCode.CompleteCardAction);
            //PhotonNetwork.RaiseEvent((byte)DuelEventCode.CompleteCardAction,
            //    null,
            //    RaiseEventOptions.Default,
            //    SendOptions.SendReliable);
        }

        public void EndGame(FieldController loser)
        {
            if (loser is PlayerController)
            {
                // Display lose
                Debug.Log("You Lose");
                OnLose.Invoke();
            }
            else
            {
                // Display win
                Debug.Log("You Win");
                OnWin.Invoke();
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.AreEventCodesEqual(DuelEventCode.SetActionToken))
            {
                ActionToken = (UserType)photonEvent.CustomData;
            }
            if (photonEvent.AreEventCodesEqual(DuelEventCode.CompleteCardAction))
            {
                hasOpponentCardActionCompleted = true;
            }
        }
    }
}
