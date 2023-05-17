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
        public static GameManager gameManager;

        [field:SerializeField]public Card.PostureCard PostureCard { get; private set; }
        [field:SerializeField]public StrategyPlan FirstIdeaCard { get; private set; }
        [field:SerializeField]public StrategyPlan SecondIdeaCard { get; private set; }
        [field:SerializeField]public StrategyPlan ThirdIdeaCard { get; private set; }
        [field:SerializeField]public DeckController DeckController { get; private set; }
        [field:SerializeField]public DustController DustController { get; private set; }
        [field:SerializeField]public HandController HandController { get; private set; }

        private Page currentPage = Page.Start;
        [SerializeField]
        private GameObject playerPrefab;
        private GameObject player;

        public Page CurrentPage
        {
            get => currentPage;
            set
            {
                switch (value)
                {
                    case Page.Start:
                        currentPage = Page.Start;
                        break;
                    case Page.Drow:
                        currentPage = Page.Drow;
                        break;
                    case Page.StrategyPlan:
                        currentPage = Page.StrategyPlan;
                        break;
                    case Page.Duel:
                        currentPage = Page.Duel;
                        break;
                    case Page.End:
                        currentPage = Page.End;
                        break;
                }
            }
        }

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
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.LeaveRoom();
            else
                SceneManager.LoadScene(0);
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

        public enum Page
        {
            Start,
            Drow,
            StrategyPlan,
            Duel,
            End,
        }
    }
}
