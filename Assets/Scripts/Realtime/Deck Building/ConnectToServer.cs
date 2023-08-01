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
        [SerializeField]
        private Inventory inventory;
        private bool triesEnteringLobby;

        private void Awake()
        {
            if (!PhotonNetwork.IsConnected) ConnectUsingSettings();
        }

        public void JoinLobby()
        {
            if (inventory.transform.childCount == 11)
            {
                triesEnteringLobby = true;

                Room.DeckController.deckList.Clear();

                for (int i = 0; i < inventory.transform.childCount; i++)
                {
                    Room.DeckController.deckList.Add(inventory.transform.GetChild(i).GetComponent<DragalbeCard>().CardData);
                }
#if UNITY_EDITOR
                string debugCardList = "Card list: ";
                foreach (var card in Room.DeckController.deckList)
                {
                    debugCardList += '\n' + card.ThisCardCode;
                }
                Debug.Log(debugCardList);
#endif
                if (PhotonNetwork.IsConnected) PhotonNetwork.JoinLobby();
                else ConnectUsingSettings();
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
