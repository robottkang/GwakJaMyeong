using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

namespace Lobby
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private TMP_InputField createInput;
        [SerializeField]
        private TMP_InputField joinInput;


        public void CreateRoom()
        {
            if (createInput.text.Length == 0)
            {;
                Debug.Log("no room name");
                return;
            }
            PhotonNetwork.CreateRoom(createInput.text, new RoomOptions { MaxPlayers = 2 });
        }

        public void JoinRoom()
        {
            if (joinInput.text.Length == 0)
            {
                Debug.Log("no room name2");
                return;
            }
            PhotonNetwork.JoinRoom(joinInput.text);
        }

        public void LeaveLobby()
        {
            PhotonNetwork.LeaveLobby();
        }
        

        public override void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevel("Room");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("Failed to join room: " + message);
        }

        public override void OnLeftLobby()
        {
            SceneManager.LoadScene("Ready");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            SceneManager.LoadScene("Ready");
            Debug.LogError(cause);
        }
    }
}
