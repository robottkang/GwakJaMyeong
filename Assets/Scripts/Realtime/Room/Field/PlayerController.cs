using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Room
{
    public class PlayerController : FieldController
    {
        [SerializeField]
        PlanCardController planCardController;


        public PlanCardController PlanCardController => planCardController;
        public bool IsReadyPlanCard { get; set; } = false;
        public override FieldController OpponentField => Opponent.OpponentController.Instance;

        public static PlayerController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}
