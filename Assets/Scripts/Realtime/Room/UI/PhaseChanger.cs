using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using TMPro;
using System;
using System.Threading;
using Photon.Realtime;

namespace Room
{
    public class PhaseChanger : MonoBehaviourPun
    {
        [SerializeField]
        private TextMeshProUGUI text;
        private Button thisButton;
        PlayerState[] playerStates;
        PlayerState myPlayerState;
        private int countOfPlayers = -1;
        private bool canPassNextPhase = true;

        private CancellationTokenSource ctsTimeout = new();
        private CancellationTokenSource ctsHasOpponent = new();

        private void Awake()
        {
            thisButton = GetComponent<Button>();
        }

        private void Start()
        {
            text.text = "Ready";
            myPlayerState = GameManager.gameManager.PlayerObject.GetComponent<PlayerState>();
        }

        private void Update()
        {
            if (countOfPlayers != PhotonNetwork.CountOfPlayers)
            {
                playerStates = FindObjectsOfType<PlayerState>();
                countOfPlayers = PhotonNetwork.CountOfPlayers;
            }
        }

        public void ChangeNextPhase()
        {
            if (!canPassNextPhase) return;

            canPassNextPhase = false;
            GameManager gameManager = GameManager.gameManager;
            
            UniTask.Void(async (CancellationToken cts) =>
            {
                switch (gameManager.CurrentPage)
                {
                    case Phase.WaitPlayer:
                        await WaitPlayer();
                        if (PhotonNetwork.IsMasterClient) ExecuteCoinToss();
                        await WaitCoinTossResult();
                        gameManager.CurrentPage = Phase.Drow;
                        break;
                    case Phase.Drow:
                        gameManager.CurrentPage = Phase.StrategyPlan;
                        break;
                    case Phase.StrategyPlan:
                        if (gameManager.FirstIdeaCard.PlacedCardInfo == null ||
                            gameManager.SecondIdeaCard.PlacedCardInfo == null ||
                            gameManager.ThirdIdeaCard.PlacedCardInfo == null) return;
                        gameManager.CurrentPage = Phase.Duel;
                        break;
                    case Phase.Duel:
                        gameManager.CurrentPage = Phase.End;
                        break;
                    case Phase.End:
                        gameManager.CurrentPage = Phase.Drow;
                        break;
                }
            }, ctsHasOpponent.Token);

            canPassNextPhase = true;
            text.text = gameManager.CurrentPage.ToString();
        }

        public async UniTask WaitPlayer()
        {
            text.text = "Wait Player";
            myPlayerState.isReadyToPlay = true;
            thisButton.onClick.AddListener(StopWaitingPlayer);

            while (true)
            {
                if (playerStates.Length == 2 && playerStates[0].isReadyToPlay && playerStates[1].isReadyToPlay)
                {
                    Debug.Log("Pass");
                    return;
                }
                await UniTask.Yield(ctsHasOpponent.Token);
            }
        }

        public void StopWaitingPlayer()
        {
            ctsHasOpponent.Cancel();
            ctsHasOpponent = new();
            thisButton.onClick.RemoveListener(StopWaitingPlayer);

            myPlayerState.isReadyToPlay = false;
            text.text = "Ready";
            canPassNextPhase = true;
        }

        public void ExecuteCoinToss()
        {
            bool coin = UnityEngine.Random.Range(0, 2) > 0; // true is first

            foreach (var player in playerStates)
            {
                player.isMyAttackTurn = coin;
                coin = !coin;
            }
        }

        public async UniTask WaitCoinTossResult()
        {
            text.text = "coin toss";
            thisButton.onClick.RemoveListener(StopWaitingPlayer);

            ctsTimeout = new();
            ctsTimeout.CancelAfter(TimeSpan.FromSeconds(15));

            try
            {
                while (true)
                {
                    if (playerStates[0].isMyAttackTurn ^ playerStates[1].isMyAttackTurn) return;

                    await UniTask.Yield(ctsTimeout.Token);
                }
            }
            catch (OperationCanceledException ex)
            {
                if (ctsTimeout.Token == ex.CancellationToken)
                {
                    Debug.Log(ex.Message);
                }
            }
        }
    }
}
