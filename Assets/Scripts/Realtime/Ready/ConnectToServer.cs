using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TextMeshProUGUI loadingButtonText;
    [SerializeField]
    private Inventory inventory;

    public void ConnectUsingSettings()
    {
        if (inventory.transform.childCount == 11)
        {
            Room.DeckController.deckList.Clear();

            for (int i = 0; i < inventory.transform.childCount; i++)
            {
                Room.DeckController.deckList.Add(inventory.transform.GetChild(i).GetComponent<DragalbeCard>().CardInfo);
#if UNITY_EDITOR
                Debug.Log(Room.DeckController.deckList[i]);
#endif
            }
            loadingButtonText.text = "Loading...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    
    public override void OnDisconnected(DisconnectCause cause)
    {
        loadingButtonText.text = $"{cause}\nRetry";
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
