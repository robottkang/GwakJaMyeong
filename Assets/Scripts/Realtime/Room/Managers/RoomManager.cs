using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;

namespace Room
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        private static bool isReadyToPlay = false;
        public static bool IsReadyToPlay { get => isReadyToPlay; set => isReadyToPlay = value; }

        private void Start()
        {
#if UNITY_EDITOR
            if (PhotonNetwork.IsConnected)
                Debug.Log(PhotonNetwork.CurrentRoom.ToString());
            else if (GameManager.Instance.EnabledDebug)
                Debug.Log("Debug mode");
#endif
            if (!GameManager.Instance.EnabledDebug && !PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("Deck Building");
            }
        }

        public void LeaveRoom()
        {
            PhaseEventBus.Clear();

            if (PhotonNetwork.InRoom)
                PhotonNetwork.LeaveRoom();
            else
                OnDisconnected(DisconnectCause.ExceptionOnConnect);
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.JoinLobby();
            PhotonNetwork.LoadLevel("Lobby");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhaseManager.CurrentPhase != Phase.BeforeStart)
            {
                LeaveRoom();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            SceneManager.LoadScene(0);
        }
    }
}
