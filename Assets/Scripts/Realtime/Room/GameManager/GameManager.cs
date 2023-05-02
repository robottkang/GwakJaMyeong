using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace Room
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager gameManager;

        [field:SerializeField]public GameObject SowrdCard { get; private set; }
        [field:SerializeField]public Transform FirstIdeaCardTransform { get; private set; }
        [field:SerializeField]public Transform SecondIdeaCardTransform { get; private set; }
        [field:SerializeField]public Transform ThirdIdeaCardTransform { get; private set; }
        [field:SerializeField]public Transform DeckTransform { get; private set; }
        [field:SerializeField]public Transform DustTransform { get; private set; }
        
        [SerializeField]
        private GameObject playerPrefab;
        private GameObject player;

        private void Awake()
        {
            gameManager = this;
        }

        private void Start()
        {
            player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
            Debug.Log(PhotonNetwork.CurrentRoom);
        }

        public void LeftRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.JoinLobby();
            PhotonNetwork.LoadLevel("Lobby");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            SceneManager.LoadScene("Ready");
        }
    }
}
