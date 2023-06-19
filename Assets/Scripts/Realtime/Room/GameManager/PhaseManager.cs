using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;
using System.Threading;
using System.Linq;

namespace Room
{
    public class PhaseManager : MonoBehaviourPun
    {
        [SerializeField]
        private TextMeshProUGUI text;
        private Button thisButton;
        List<PlayerState> playerStates = new(2);
        PlayerState myPlayerState;
        private bool canPassNextPhase = true;

        private CancellationTokenSource ctsChangePhase = new();

        private void Awake()
        {
            thisButton = GetComponent<Button>();
        }

        private void Start()
        {
            text.text = "Ready";
            myPlayerState = GameManager.gameManager.PlayerState;
        }

        private void Update()
        {
            if (playerStates.Count != PhotonNetwork.ViewCount)
            {
                playerStates = FindObjectsOfType<PlayerState>().ToList();
            }
        }

        private void OnDestroy()
        {
            ctsChangePhase.Cancel();
        }

        public void ChangeNextPhase()
        {
            if (!canPassNextPhase) return;

            canPassNextPhase = false;
            GameManager gameManager = GameManager.gameManager;
            
            UniTask.Void(async (CancellationToken cts) =>
            {
                switch (gameManager.CurrentPhase)
                {
                    case Phase.WaitPlayer:
                        await WaitPlayer(cts);
                        if (PhotonNetwork.IsMasterClient) ExecuteCoinToss();
                        gameManager.CurrentPhase = Phase.Draw;
                        break;
                    case Phase.Draw:
                        gameManager.CurrentPhase = Phase.StrategyPlan;
                        break;
                    case Phase.StrategyPlan:
                        if (gameManager.FirstIdeaCard.PlacedCardInfo == null ||
                            gameManager.SecondIdeaCard.PlacedCardInfo == null ||
                            gameManager.ThirdIdeaCard.PlacedCardInfo == null) return;
                        gameManager.CurrentPhase = Phase.Duel;
                        break;
                    case Phase.Duel:
                        gameManager.CurrentPhase = Phase.End;
                        break;
                    case Phase.End:
                        gameManager.CurrentPhase = Phase.Draw;
                        break;
                }
            }, ctsChangePhase.Token);

            canPassNextPhase = true;
            text.text = gameManager.CurrentPhase.ToString();
        }

        public async UniTask WaitPlayer(CancellationToken cts)
        {
            text.text = "Wait Player";
            myPlayerState.isReadyToPlay = true;
            thisButton.onClick.AddListener(StopWaitingPlayer);

            while (true)
            {
                if (playerStates.Count == 2 && playerStates[0].isReadyToPlay && playerStates[1].isReadyToPlay)
                {
                    Debug.Log("All Player is connected");
                    thisButton.onClick.RemoveListener(StopWaitingPlayer);
                    return;
                }
                await UniTask.Yield(cts);
            }
        }

        public void StopWaitingPlayer()
        {
            ctsChangePhase.Cancel();
            ctsChangePhase = new();
            thisButton.onClick.RemoveListener(StopWaitingPlayer);

            myPlayerState.isReadyToPlay = false;
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
