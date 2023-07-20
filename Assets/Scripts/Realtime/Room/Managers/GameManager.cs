using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

namespace Room
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance { get; private set; }
        
        [SerializeField]
        private GameObject counterField;
        
        [field:SerializeField]public StrategyPlan[] StrategyPlans { get; private set; } = new StrategyPlan[3];
        public PlayerView MyPlayerView { get; private set; }
        public PlayerView OpponentPlayerView { get; private set; }


        private void Awake()
        {
            Instance = this;
            
            MyPlayerView = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity).GetComponent<PlayerView>();
        }

        private void Start()
        {
            Debug.Log(PhotonNetwork.CurrentRoom.ToString());
        }

        private void Update()
        {
            if (OpponentPlayerView == null)
            {
                foreach (var playerView in FindObjectsOfType<PlayerView>())
                {
                    if (playerView == MyPlayerView) return;
                    OpponentPlayerView = playerView;
                }
            }
        }

        public void LeaveRoom()
        {
            PhaseEventBus.Clear();

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
            OpponentPlayerView = null;

            if (PhaseManager.Instance.CurrentPhase != Phase.WaitPlayer)
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
