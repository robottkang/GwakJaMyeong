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

        private Page currentPage = Page.WaitPlayer;
        [SerializeField]
        private GameObject counterField;

        public Page CurrentPage
        {
            get => currentPage;
            set
            {
                currentPage = value;

#if UNITY_EDITOR
                Debug.Log($"--- Current Page: {currentPage} ---");
#endif
                PageEventBus.Publish(currentPage);
            }
        }

        private void Awake()
        {
            gameManager = this;
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
    }
}
