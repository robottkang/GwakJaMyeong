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
        public PlayerState PlayerState { get; private set; }

        private Phase currentPhase = Phase.WaitPlayer;
        [SerializeField]
        private GameObject counterField;

        public Phase CurrentPhase
        {
            get => currentPhase;
            set
            {
                currentPhase = value;

#if UNITY_EDITOR
                Debug.Log($"--- Current Page: {currentPhase} ---");
#endif
                PhaseEventBus.Publish(currentPhase);
            }
        }

        private void Awake()
        {
            gameManager = this;
            
            PlayerState = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity).GetComponent<PlayerState>();
        }

        private void Start()
        {
            Debug.Log(PhotonNetwork.CurrentRoom.ToString());
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

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (CurrentPhase != Phase.WaitPlayer)
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
