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
        private List<PlayerState> playerStateList;
        
        [field:SerializeField]public StrategyPlan[] StrategyPlans { get; private set; } = new StrategyPlan[3];
        public PlayerState MyPlayerState { get; private set; }
        public PlayerState[] PlayerStateList => playerStateList.ToArray();


        private void Awake()
        {
            Instance = this;
            
            MyPlayerState = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity).GetComponent<PlayerState>();
        }

        private void Start()
        {
            Debug.Log(PhotonNetwork.CurrentRoom.ToString());
        }

        private void Update()
        {
            if (playerStateList.Count != PhotonNetwork.CurrentRoom.PlayerCount)
            {
                playerStateList = FindObjectsOfType<PlayerState>().ToList();
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
