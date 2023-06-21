using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using TMPro;
using System.Threading;
using System.Linq;

namespace Room
{
    public class PhaseButtonController : MonoBehaviourPun
    {
#if UNITY_EDITOR
        [EasyButtons.Button] private void ChangePhaseTest(int phase) => PhaseManager.Instance.CurrentPhase = (Phase)phase;
#endif
        [Header("- Reference")]
        [SerializeField]
        private TextMeshProUGUI text;
        [SerializeField]
        private Button phaseButton;
        List<PlayerState> playerStates = new(2);
        private bool canPassNextPhase = true;

        private CancellationTokenSource ctsChangePhase = new();

        private void Start()
        {
            text.text = "Ready";
        }

        private void OnDestroy()
        {
            ctsChangePhase.Cancel();
        }

        public void ChangeNextPhase()
        {
            if (!canPassNextPhase) return;

            canPassNextPhase = false;
            PhaseManager phaseManager = PhaseManager.Instance;

            UniTask.Void(async (CancellationToken cts) =>
            {
                switch (phaseManager.CurrentPhase)
                {
                    case Phase.WaitPlayer:
                        await WaitPlayer(cts);
                        if (PhotonNetwork.IsMasterClient) ExecuteCoinToss();
                        phaseManager.CurrentPhase = Phase.Draw;
                        break;
                    case Phase.Draw:
                        phaseManager.CurrentPhase = Phase.StrategyPlan;
                        break;
                    case Phase.StrategyPlan:
                        foreach (var strategyPlan in GameManager.Instance.StrategyPlans)
                            if (strategyPlan.PlacedCardInfo == null) return;
                        phaseManager.CurrentPhase = Phase.Duel;
                        break;
                    case Phase.Duel:
                        phaseManager.CurrentPhase = Phase.End;
                        break;
                    case Phase.End:
                        phaseManager.CurrentPhase = Phase.Draw;
                        break;
                }
            }, ctsChangePhase.Token);

            canPassNextPhase = true;
            text.text = phaseManager.CurrentPhase.ToString();
        }

        public async UniTask WaitPlayer(CancellationToken cts)
        {
            text.text = "Wait Player";
            GameManager.Instance.MyPlayerState.isReadyToPlay = true;
            phaseButton.onClick.AddListener(StopWaitingPlayer);

            while (true)
            {
                if (playerStates.Count == 2 && playerStates[0].isReadyToPlay && playerStates[1].isReadyToPlay)
                {
                    Debug.Log("All Player is connected");
                    phaseButton.onClick.RemoveListener(StopWaitingPlayer);
                    return;
                }
                await UniTask.Yield(cts);
            }
        }

        public void StopWaitingPlayer()
        {
            ctsChangePhase.Cancel();
            ctsChangePhase = new();
            phaseButton.onClick.RemoveListener(StopWaitingPlayer);

            GameManager.Instance.MyPlayerState.isReadyToPlay = false;
            text.text = "Ready";
            canPassNextPhase = true;
        }

        public void ExecuteCoinToss()
        {
            bool coin = UnityEngine.Random.Range(0, 2) > 0;

            foreach (var player in playerStates)
            {
                player.isMyAttackTurn = coin;
                coin = !coin;
            }
        }
    }
}
