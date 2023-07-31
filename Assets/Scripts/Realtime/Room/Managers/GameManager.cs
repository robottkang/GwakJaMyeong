using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace Room
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance { get; private set; }


        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Debug.Log(PhotonNetwork.CurrentRoom.ToString());
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
