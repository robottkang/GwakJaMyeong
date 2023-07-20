using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using TMPro;
using System.Threading;

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
        [SerializeField]
        private PostureChanger postureChanger;
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
                        await ChoosePosture(cts);
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
            GameManager gameManager = GameManager.Instance;
            text.text = "Wait Player";
            gameManager.MyPlayerView.readyToPlay = true;
            phaseButton.onClick.AddListener(StopWaitingPlayer);

            while (true)
            {
                if (gameManager.MyPlayerView.readyToPlay && 
                    gameManager.OpponentPlayerView != null && 
                    gameManager.OpponentPlayerView.readyToPlay)
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

            GameManager.Instance.MyPlayerView.readyToPlay = false;
            text.text = "Ready";
            canPassNextPhase = true;
        }

        public void ExecuteCoinToss()
        {
            bool coin = UnityEngine.Random.Range(0, 2) > 0;

            //GameManager.Instance.MyPlayerView.duelManager.hasActionToken = coin;
            //GameManager.Instance.OpponentPlayerView.duelManager.hasActionToken = !coin;
        }

        public async UniTask ChoosePosture(CancellationToken cts)
        {
            GameManager gameManager = GameManager.Instance;
            //await UniTask.WaitUntil(() => gameManager.MyPlayerView.duelManager.hasActionToken, cancellationToken: cts);
            text.text = "Choose Posture";

            //await UniTask.WaitUntil(() => 
            //gameManager.MyPlayerView.duelManager.setPosture && 
            //gameManager.OpponentPlayerView.duelManager.setPosture, 
            //cancellationToken: cts);
        }
    }
}
