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
        public GameObject PlayerObject { get; private set; }

        private Phase currentPage = Phase.WaitPlayer;
        [SerializeField]
        private GameObject counterField;

        public Phase CurrentPhase
        {
            get => currentPage;
            set
            {
                currentPage = value;

#if UNITY_EDITOR
                Debug.Log($"--- Current Page: {currentPage} ---");
#endif
                PhaseEventBus.Publish(currentPage);
            }
        }

        private void Awake()
        {
            gameManager = this;
            
            PlayerObject = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        }

        public void LeaveRoom()
        {
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

        public override void OnDisconnected(DisconnectCause cause)
        {
            SceneManager.LoadScene(0);
        }
    }
}
