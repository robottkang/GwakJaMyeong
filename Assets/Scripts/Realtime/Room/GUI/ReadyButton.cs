using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Room.UI
{
    public class ReadyButton : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField]
        private Button readyButton;

        private void Awake()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void Start()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Ready";
                readyButton.interactable = true;
            }
            else
            {
                GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Wait Player";
                readyButton.interactable = false;
            }
        }

        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void Ready()
        {
            RoomManager.IsReadyToPlay ^= true;
            PhotonNetwork.RaiseEvent((byte)DuelEventCode.Ready,
                true,
                RaiseEventOptions.Default,
                SendOptions.SendReliable);

            readyButton.interactable = false;

            UniTask.Void(async (token) =>
            {
                await UniTask.WaitUntil(() => !RoomManager.IsReadyToPlay, cancellationToken: token);

                GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "DUEL!";
                await UniTask.Delay(System.TimeSpan.FromSeconds(1), cancellationToken: token);
                _ = readyButton.GetComponent<CanvasGroup>().DOFade(0f, 0.5f)
                .OnComplete(() => readyButton.gameObject.SetActive(false));

                PhaseManager.ChangeNextPhase();
            }, gameObject.GetCancellationTokenOnDestroy());
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            readyButton.interactable = true;
            readyButton.GetComponent<CanvasGroup>().alpha = 1;
            GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Ready";
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhaseManager.CurrentPhase == Phase.BeforeStart)
            {
                readyButton.interactable = false;
                GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Wait Player";
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
