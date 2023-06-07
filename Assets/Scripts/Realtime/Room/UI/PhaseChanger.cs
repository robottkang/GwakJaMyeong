using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using TMPro;
using System;
using System.Threading;

namespace Room
{
    public class PhaseChanger : MonoBehaviourPun
    {
        [SerializeField]
        private TextMeshProUGUI text;
        private Button thisButton;
        PlayerState[] playerStates;

        private CancellationTokenSource ctsTimeout = new();
        private CancellationTokenSource ctsHasOpponent = new();

        private void Awake()
        {
            thisButton = GetComponent<Button>();
        }

        private void Start()
        {
            text.text = "Ready";
        }

        public void ChangeNextPhase()
        {
            thisButton.interactable = false;
            GameManager gameManager = GameManager.gameManager;
            
            UniTask.Void(async (CancellationToken cts) =>
            {
                switch (gameManager.CurrentPage)
                {
                    case Phase.WaitPlayer:
                        await WaitPlayer();
                        if (PhotonNetwork.IsMasterClient) ExecuteCoinToss();
                        await WaitCoinTossResult();
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

            text.text = gameManager.CurrentPage.ToString();
            thisButton.interactable = true;
        }

        public void ExecuteCoinToss()
        {
            bool coin = UnityEngine.Random.Range(0, 2) > 0; // true is first

            SetCoinTossResult(coin);
            GameManager.gameManager.PlayerObject.GetPhotonView().RPC(nameof(SetCoinTossResult), PhotonNetwork.PlayerList[1], !coin);
        }

        public void SetCoinTossResult(bool coinResult)
        {
            GameManager.gameManager.PlayerObject.GetComponent<PlayerState>().isMyAttackTurn = coinResult;
        }

        public async UniTask WaitPlayer()
        {
            text.text = "Wait Player";

            while (true)
            {
                if (PhotonNetwork.CountOfPlayers == 2 && playerStates[0].isReadyToPlay && playerStates[1].isReadyToPlay)
                {
                    return;
                }
                await UniTask.Yield(ctsHasOpponent.Token);
            }
        }

        public async UniTask WaitCoinTossResult()
        {
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
                    Debug.Log("Connecting fail");
                }
            }
        }
    }
}
