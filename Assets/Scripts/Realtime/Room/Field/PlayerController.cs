using Card;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public void SendPlanCards() // 이거를 준비 되었다는 표시만 보내고 카드 내용물은 카드 깔 때 or 효과 시전할 때 보이도록 수정(일단 게임 완성되면 수정)
        {
            var cardCodeList = new List<CardData.CardCode>() { GetCard(0).CardData.ThisCardCode, GetCard(1).CardData.ThisCardCode, GetCard(2).CardData.ThisCardCode };

            new RaiseEventData<List<CardData.CardCode>>(UserType.Player, cardCodeList).RaiseDuelEvent(DuelEventCode.SendCardData);
            //PhotonNetwork.RaiseEvent((byte)DuelEventCode.SendCardsData,
            //    JsonUtility.ToJson(GetCard(i).CardData),
            //    RaiseEventOptions.Default,
            //    SendOptions.SendReliable);

#if UNITY_EDITOR
            Debug.Log("Sending Cards: " +
                cardCodeList[0].ToString() + '\n' +
                cardCodeList[1].ToString() + '\n' +
                cardCodeList[2].ToString() + '\n');
#endif

            IsReadyPlanCard = true;
        }
    }
}
