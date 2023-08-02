using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Room.Opponent;

namespace Room
{
    public class PhaseManager : MonoBehaviour, IOnEventCallback
    {
#if UNITY_EDITOR
        [EasyButtons.Button] private void ChangePhaseTest(int phase) => CurrentPhase = (Phase)phase;
#endif
        private UniTask phaseChangingTask;
        private CancellationTokenSource cts = new();
        private UnityEvent onRecallDuringPhaseChanging = new();
        
        private static Phase currentPhase = Phase.BeforeStart;
        private static int turnCount = 1;

        public static Phase CurrentPhase
        {
            get => currentPhase;
            set
            {
                currentPhase = value;

#if UNITY_EDITOR
                Debug.Log($"--- Current Page: {currentPhase} ---");
#endif
                PhaseEventBus.Publish(currentPhase);
            }
        }
        public static int TurnCount => turnCount;


        private void Awake()
        {
            PhotonNetwork.AddCallbackTarget(this);

            currentPhase = Phase.BeforeStart;
            turnCount = 1;
            PhaseEventBus.Subscribe(Phase.End, () => turnCount++);
        }

        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);

            cts.Cancel();
            cts.Dispose();
        }

        public void ChangeNextPhase()
        {
            if (phaseChangingTask.Status == UniTaskStatus.Pending)
            {
                onRecallDuringPhaseChanging.Invoke();
                onRecallDuringPhaseChanging.RemoveAllListeners();
                return;
            }

            phaseChangingTask = CurrentPhase switch
            {
                Phase.BeforeStart => BeforeStartPhase(cts.Token),
                Phase.Launch => LaunchPhase(cts.Token),
                Phase.Draw => DrawPhase(cts.Token),
                Phase.StrategyPlan => StrategyPlanPhase(cts.Token),
                Phase.Duel => DuelPhase(cts.Token),
                Phase.End => EndPhase(cts.Token),
                _ => UniTask.Create(() => throw new NotImplementedException()),
            };
        }

        private void CancelPhaseChanging()
        {
            cts.Cancel();
            cts = new();
            phaseChangingTask = UniTask.CompletedTask;
        }

        #region BeforeStartPhase
        private async UniTask BeforeStartPhase(CancellationToken token)
        {
            await WaitPlayer(token);
            CurrentPhase = Phase.Launch;
        }

        private async UniTask WaitPlayer(CancellationToken token)
        {
            PhotonNetwork.RaiseEvent((byte)DuelEventCode.Ready,
                true,
                RaiseEventOptions.Default,
                SendOptions.SendReliable);
            onRecallDuringPhaseChanging.AddListener(StopWaitingPlayer);
            await UniTask.WaitUntil(() => OpponentController.Instance.IsReadyToPlay, cancellationToken: token);
        }

        private void StopWaitingPlayer()
        {
            PhotonNetwork.RaiseEvent((byte)DuelEventCode.Ready,
                false,
                RaiseEventOptions.Default,
                SendOptions.SendReliable);

            CancelPhaseChanging();
        }
        #endregion

        #region LaunchPhase
        private async UniTask LaunchPhase(CancellationToken token)
        {
            if (PhotonNetwork.IsMasterClient) ExecuteCoinToss();
            await ChooseOpeningPosture(token);
            CurrentPhase = Phase.StrategyPlan;
        }

        public void ExecuteCoinToss()
        {
            bool coin = UnityEngine.Random.Range(0, 2) > 0;

            DuelManager.SetActionToken(coin);
            Debug.Log("Coin Toss: " + (coin ? "mine" : "opponent"));
        }

        public async UniTask ChooseOpeningPosture(CancellationToken token)
        {
            OpponentController.Instance.PostureController.SelectPosture();
            await UniTask.WaitUntil(() => DuelManager.HasActionToken, cancellationToken: token);

            PlayerController.Instance.PostureController.SelectPosture();
            await UniTask.WaitUntil(() => !PlayerController.Instance.PostureController.IsPostureChanging, cancellationToken: token);
            DuelManager.SwapActionToken();
            await UniTask.WaitUntil(() => !OpponentController.Instance.PostureController.IsPostureChanging, cancellationToken: token);
        }
        #endregion

        #region DrawPhase
        private async UniTask DrawPhase(CancellationToken token)
        {
            CurrentPhase = Phase.StrategyPlan;
        }
        #endregion

        #region StrategyPlanPhase
        private async UniTask StrategyPlanPhase(CancellationToken token)
        {
            DuelManager.ResetStrategyPlanOrder();
            if (!CheckStrategyPlanReady()) return;
            await UniTask.WaitUntil(() => OpponentController.Instance.IsReadyPlanCard, cancellationToken: token);
            CurrentPhase = Phase.Duel;
        }

        public bool CheckStrategyPlanReady()
        {
            Card.CardData[] cardsData = new Card.CardData[3];

            for (int i = 0; i < 3; i++)
            {
                Card.PlanCard planCard;
                if ((planCard = PlayerController.Instance.GetCard(i)) == null)
                    return false;
                cardsData[i] = planCard.CardData;
            }

            for (int i = 0; i < 3; i++)
            {
                PhotonNetwork.RaiseEvent((byte)DuelEventCode.SendCardsData,
                    JsonUtility.ToJson(cardsData[i]),
                    RaiseEventOptions.Default,
                    SendOptions.SendReliable);
            }

            Debug.Log("Send cards data");
            return true;
        }
        #endregion

        #region DuelPhase
        private async UniTask DuelPhase(CancellationToken token)
        {
            CurrentPhase = Phase.End;
        }
        #endregion

        #region EndPhase
        private async UniTask EndPhase(CancellationToken token)
        {
            CurrentPhase = Phase.Draw;
        }
        #endregion

        public void OnEvent(EventData photonEvent)
        {

        }
    }

    public enum Phase
    {
        BeforeStart,
        Launch,
        Draw,
        StrategyPlan,
        Duel,
        End,
    }
}
