using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace Room
{
    public class PhaseManager : MonoBehaviour, IOnEventCallback
    {
#if UNITY_EDITOR
        [EasyButtons.Button] private void ChangePhaseTest(int phase) => CurrentPhase = (Phase)phase;
#endif
        private bool getsActionToken = false;
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
            phaseChangingTask.Forget();
        }

        private void CancelChangingPhase()
        {
            cts.Cancel();
            cts = new();
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
            await UniTask.WaitUntil(() => Opponent.OpponentController.Instance.IsReadyToPlay, cancellationToken: token);
        }

        private void StopWaitingPlayer()
        {
            PhotonNetwork.RaiseEvent((byte)DuelEventCode.Ready,
                false,
                RaiseEventOptions.Default,
                SendOptions.SendReliable);

            CancelChangingPhase();
        }
        #endregion

        #region LaunchPhase
        private async UniTask LaunchPhase(CancellationToken token)
        {
            if (PhotonNetwork.IsMasterClient) ExecuteCoinToss();
            await UniTask.WaitUntil(() => getsActionToken, cancellationToken: token);

            await ChooseOpeningPosture(token);
            CurrentPhase = Phase.StrategyPlan;
        }

        public void ExecuteCoinToss()
        {
            bool coin = UnityEngine.Random.Range(0, 2) > 0;

            PhotonNetwork.RaiseEvent((byte)DuelEventCode.SetActionToken,
                !coin,
                new RaiseEventOptions { Receivers = ReceiverGroup.Others },
                SendOptions.SendReliable);
            PhotonNetwork.RaiseEvent((byte)DuelEventCode.SetActionToken,
                coin,
                new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
                SendOptions.SendReliable);
        }

        public async UniTask ChooseOpeningPosture(CancellationToken token)
        {
            var myPostureChooseTask = UniTask.Create(async () =>
            {
                PlayerController.Instance.PostureController.SelectPosture();
                await UniTask.WaitUntil(() => PlayerController.Instance.PostureController.IsPostureChanging, cancellationToken: token);
            });
            var opponentPostureChooseTask = UniTask.Create(async () =>
            {
                Opponent.OpponentController.Instance.PostureController.SelectPosture();
                await UniTask.WaitUntil(() => Opponent.OpponentController.Instance.PostureController.IsPostureChanging, cancellationToken: token);
            });

            if (DuelManager.HasActionToken)
            {
                await myPostureChooseTask;
                await opponentPostureChooseTask;
            }
            else
            {
                await opponentPostureChooseTask;
                await myPostureChooseTask;
            }
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
            await UniTask.WaitUntil(() => Opponent.OpponentController.Instance.IsReadyPlanCard, cancellationToken: token);
            CurrentPhase = Phase.Duel;
        }

        public bool CheckStrategyPlanReady()
        {
            Card.CardInfo[] cardInfos = new Card.CardInfo[3];
            for (int i = 0; i < 3; i++)
            {
                if (PlayerController.Instance.GetCard(i) == null)
                    return false;
                cardInfos[i] = PlayerController.Instance.GetCard(i).CardInfo;
            }

            PhotonNetwork.RaiseEvent((byte)DuelEventCode.SendCardInfos,
                cardInfos,
                RaiseEventOptions.Default,
                SendOptions.SendReliable);
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
            ChangeNextPhase();
        }
        #endregion

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (byte)DuelEventCode.SetActionToken)
            {
                getsActionToken = true;
            }
        }

    }
}
