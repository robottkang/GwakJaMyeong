using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;
using Card;

namespace DeckBuilding
{
    public class ConnectToServer : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private TextMeshProUGUI loadingButtonText;
        private bool triesEnteringLobby;

        private void Awake()
        {
            if (!PhotonNetwork.IsConnected) ConnectUsingSettings();
        }

        public void JoinLobby()
        {
            if (!PhotonNetwork.IsConnected)
            {
                ConnectUsingSettings();
                return;
            }

            if (Inventory.Content.Length == 11)
            {
                triesEnteringLobby = true;

                Room.DeckZoneController.deckList.Clear();

                for (int i = 0; i < Inventory.Content.Length; i++)
                {
                    Room.DeckZoneController.deckList.Add(Inventory.Content[i].GetComponent<DragalbeCard>().CardData);
                }
#if UNITY_EDITOR
                string debugCardList = "Card list: ";
                foreach (var card in Room.DeckZoneController.deckList)
                {
                    debugCardList += '\n' + card.ThisCardCode.ToString();
                }
                Debug.Log(debugCardList);
#endif
                PhotonNetwork.JoinLobby();
            }
        }

        private void ConnectUsingSettings()
        {
            PhotonNetwork.ConnectUsingSettings();
            loadingButtonText.text = "Loading...";
        }

        public override void OnConnectedToMaster()
        {
            loadingButtonText.text = "Press to Enter\nLobby";
            if (triesEnteringLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            loadingButtonText.text = "Connect Error\nPress To Retry";
            Debug.LogError(cause);
        }

        public override void OnJoinedLobby()
        {
            SceneManager.LoadScene("Lobby");
        }
    }
}
