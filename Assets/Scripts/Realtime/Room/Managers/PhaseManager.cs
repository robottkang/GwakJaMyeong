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
    public class PhaseManager : MonoBehaviour
    {
        private static UniTask phaseChangingTask = new();
        private CancellationTokenSource cts = new();
        private static UnityEvent onSecondCall = new();
        
        private static Phase currentPhase = Phase.BeforeStart;
        private static int turnCount = 1;

        public static Phase CurrentPhase
        {
            get => currentPhase;
            set
            {
                currentPhase = value;

#if UNITY_EDITOR
                Debug.Log($"--- Current Phase: {currentPhase} ---");
#endif
                PhaseEventBus.Publish(currentPhase);
            }
        }
        public static int TurnCount => turnCount;


        private void Awake()
        {
            currentPhase = Phase.BeforeStart;
            turnCount = 1;
            PhaseEventBus.Subscribe(Phase.Launch, () => phaseChangingTask = LaunchPhase(cts.Token));
            PhaseEventBus.Subscribe(Phase.Draw, () => CurrentPhase = Phase.StrategyPlan);
            PhaseEventBus.Subscribe(Phase.StrategyPlan, () => phaseChangingTask = StrategyPlanPhase(cts.Token));
            PhaseEventBus.Subscribe(Phase.Duel, () => phaseChangingTask = DuelPhase(cts.Token));
            PhaseEventBus.Subscribe(Phase.End, () => CurrentPhase = Phase.Draw);
            PhaseEventBus.Subscribe(Phase.End, () => turnCount++);
        }

        private void OnDestroy()
        {
            cts.Cancel();
            cts.Dispose();
        }

        public static void ChangeNextPhase()
        {
            if (phaseChangingTask.Status == UniTaskStatus.Pending)
            {
                onSecondCall.Invoke();
                onSecondCall.RemoveAllListeners();
                return;
            }

            CurrentPhase = CurrentPhase switch
            {
                Phase.BeforeStart => Phase.Launch,
                Phase.Launch => Phase.StrategyPlan,
                Phase.Draw => Phase.StrategyPlan,
                Phase.StrategyPlan => Phase.Duel,
                Phase.Duel => Phase.End,
                Phase.End => Phase.Draw,
                _ => throw new NotImplementedException(),
            };
            //phaseChangingTask = CurrentPhase switch
            //{
            //    Phase.BeforeStart => BeforeStartPhase(cts.Token),
            //    Phase.Launch => LaunchPhase(cts.Token),
            //    Phase.Draw => DrawPhase(cts.Token),
            //    Phase.StrategyPlan => StrategyPlanPhase(cts.Token),
            //    Phase.Duel => DuelPhase(cts.Token),
            //    Phase.End => EndPhase(cts.Token),
            //    _ => UniTask.Create(() => throw new NotImplementedException()),
            //};
        }

        private void CancelPhaseChanging()
        {
            cts.Cancel();
            cts = new();
            phaseChangingTask = UniTask.CompletedTask;
        }

        #region LaunchPhase
        private async UniTask LaunchPhase(CancellationToken token)
        {
            if (PhotonNetwork.IsMasterClient) ExecuteCoinToss();
            await ChooseOpeningPosture(token);
            CurrentPhase = Phase.StrategyPlan;
        }

        public void ExecuteCoinToss()
        {
            UserType coin = (UserType)UnityEngine.Random.Range(0, 2);

            DuelManager.SetPlayerActionToken(coin);
            Debug.Log("Coin Toss: " + (coin == UserType.Player ? "mine" : "opponent"));
        }

        public async UniTask ChooseOpeningPosture(CancellationToken token)
        {
            OpponentController.Instance.PostureCtrl.SelectPosture();
            await UniTask.WaitUntil(() => DuelManager.ActionToken == UserType.Player, cancellationToken: token);

            PlayerController.Instance.PostureCtrl.SelectPosture();
            await UniTask.WaitUntil(() => !PlayerController.Instance.PostureCtrl.IsPostureChanging, cancellationToken: token);
            DuelManager.SwapActionToken();
            await UniTask.WaitUntil(() => !OpponentController.Instance.PostureCtrl.IsPostureChanging, cancellationToken: token);
        }
        #endregion

        #region DrawPhase
        #endregion

        #region StrategyPlanPhase
        private async UniTask StrategyPlanPhase(CancellationToken token)
        {
            DuelManager.ResetStrategyPlans();
            await UniTask.WaitUntil(() => 
            PlayerController.Instance.IsReadyPlanCard && OpponentController.Instance.IsReadyPlanCard, cancellationToken: token);
            CurrentPhase = Phase.Duel;
        }
        #endregion

        #region DuelPhase
        private async UniTask DuelPhase(CancellationToken token)
        {
            await DuelManager.StartDuel();
            CurrentPhase = Phase.End;
        }
        #endregion

        #region EndPhase
        #endregion
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
