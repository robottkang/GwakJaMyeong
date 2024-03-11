using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Threading;

namespace Room.UI
{
    public class ReadyButton : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField]
        private Button readyButton;
        [SerializeField]
        private TextMeshProUGUI readyButtonText;
        private CancellationTokenSource cts = new();

        private void Awake()
        {
            PhotonNetwork.AddCallbackTarget(this);

            PhaseEventBus.Subscribe(Phase.Launch, () => DisappearButton(cts.Token).Forget());
        }

        private void Start()
        {
            try
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
                {
                    readyButtonText.text = "Ready";
                    readyButton.interactable = true;
                }
                else
                {
                    readyButtonText.text = "Wait Player";
                    readyButton.interactable = false;
                }
            }
            catch (Exception) when (GameManager.Instance.EnabledDebug)
            {
                readyButton.interactable = true;
                readyButtonText.text = "Ready(Debug)";
            }
            catch
            {
            }
        }

        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);

            cts.Cancel();
            cts.Dispose();
        }

        public void Ready()
        {
            try
            {
                readyButton.interactable = false;

                RoomManager.IsReadyToPlay ^= true;
                bool failedToSend = !PhotonNetwork.RaiseEvent((byte)DuelEventCode.Ready,
                    true,
                    RaiseEventOptions.Default,
                    SendOptions.SendReliable);
                if (failedToSend)
                    throw new Exception("Failed to send ready event");

                UniTask.Void(async (token) =>
                {
                    await UniTask.WaitUntil(() => !RoomManager.IsReadyToPlay, cancellationToken: token);

                    PhaseManager.ChangeNextPhase();
                }, cts.Token);
            }
            catch (Exception) when (GameManager.Instance.EnabledDebug)
            {
                PhaseManager.ChangeNextPhase();
            }
        }

        private async UniTask DisappearButton(CancellationToken token)
        {
            readyButtonText.text = "DUEL!";
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
            await readyButton.GetComponent<CanvasGroup>().DOFade(0f, 0.5f)
            .OnComplete(() => readyButton.gameObject.SetActive(false))
            .WithCancellation(token);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            readyButton.interactable = true;
            readyButton.GetComponent<CanvasGroup>().alpha = 1;
            readyButtonText.text = "Ready";
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhaseManager.CurrentPhase == Phase.BeforeStart)
            {
                readyButton.interactable = false;
                readyButtonText.text = "Wait Player";
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (byte)DuelEventCode.Ready)
            {
                RoomManager.IsReadyToPlay ^= true;
            }
        }
    }
}
