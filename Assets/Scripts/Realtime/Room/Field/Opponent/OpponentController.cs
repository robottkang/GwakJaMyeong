using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace Room.Opponent
{
    public class OpponentController : FieldController, IInRoomCallbacks, IOnEventCallback
    {
        [SerializeField]
        private CardStackController deck;
        [SerializeField]
        private CardStackController dust;
        private int planCardOrder = 0;
        public bool IsReadyToPlay { get; private set; } = false;
        public bool IsReadyPlanCard { get; set; } = false;
        public override FieldController OpponentField => PlayerController.Instance;

        public static OpponentController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            PhotonNetwork.AddCallbackTarget(this);

            PhaseEventBus.Subscribe(Phase.Launch, () => deck.DrawCard(5));

            PhaseEventBus.Subscribe(Phase.Draw, () =>
            {
                if (deck.CardObjects.Length == 0)
                {
                    dust.DrawCard(9);
                    deck.StackCard(9);
                }
                
                deck.DrawCard(3);
            });

            PhaseEventBus.Subscribe(Phase.End, () =>
            {
                foreach (var strategyPlan in StrategyPlans)
                {
                    strategyPlan.ClearStrategyPlan();
                }
            });
            PhaseEventBus.Subscribe(Phase.End, () => dust.StackCard(3));
        }

        private void Start()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                deck.DrawCard(deck.cardCount);
            }
        }

        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            deck.StackCard(deck.cardCount);
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            deck.DrawCard(deck.cardCount);
        }

        public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (byte)DuelEventCode.Ready)
            {
                IsReadyToPlay = (bool)photonEvent.CustomData;
            }

            if (photonEvent.Code == (byte)DuelEventCode.SendCardsData && PhaseManager.CurrentPhase == Phase.StrategyPlan)
            {
                Card.CardData cardData = ScriptableObject.CreateInstance<Card.CardData>();
                JsonUtility.FromJsonOverwrite((string)photonEvent.CustomData, cardData);
                Debug.Log($" Receive {(string)photonEvent.CustomData}");

                GameObject planCardObj = ObjectPool.GetObject("Card Pool");
                planCardObj.GetComponent<Card.PlanCard>().Initialize(cardData, this);
                planCardObj.transform.rotation = Quaternion.Euler(0, 0, 180f);
                planCardObj.GetComponent<Card.PlanCard>().CanMove = false;
                StrategyPlans[planCardOrder].PlaceCard(planCardObj);

                planCardOrder += 1;
                if (planCardOrder == 3)
                {
                    IsReadyPlanCard = true;
                    planCardOrder = 0;
                }
            }

            if (photonEvent.Code == (byte)DuelEventCode.SendCardDepolyment)
            {
                CurrentCard.CurrentCardDeployment = (Card.CardDeployment)photonEvent.CustomData;
            }
        }
    }
}
