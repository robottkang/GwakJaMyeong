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

        [field:SerializeField]public Card.PostureCard PostureCard { get; private set; }
        [field:SerializeField]public StrategyPlan FirstIdeaCard { get; private set; }
        [field:SerializeField]public StrategyPlan SecondIdeaCard { get; private set; }
        [field:SerializeField]public StrategyPlan ThirdIdeaCard { get; private set; }
        [field:SerializeField]public DeckController DeckController { get; private set; }
        [field:SerializeField]public DustController DustController { get; private set; }
        [field:SerializeField]public HandController HandController { get; private set; }
        
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
            SceneManager.LoadScene(0);
        }
    }
}
