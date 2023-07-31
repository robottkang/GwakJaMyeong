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
        private GameObject planCardPrefab;
        [SerializeField]
        private CardStackController deck;
        [SerializeField]
        private CardStackController dust;
        public bool IsReadyToPlay { get; private set; } = false;
        public bool IsReadyPlanCard { get; private set; } = false;

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

            if (photonEvent.Code == (byte)DuelEventCode.SendCardInfos && PhaseManager.CurrentPhase == Phase.StrategyPlan)
            {
                var planCards = (Card.CardInfo[])photonEvent.CustomData;
                for (int i = 0; i < planCards.Length; i++)
                {
                    GameObject planCard = ObjectPool.GetObject("Card Pool", planCardPrefab);
                    planCard.GetComponent<Card.PlanCard>().CardInfo = planCards[i];
                    PlaceCard(planCard);
                }
                IsReadyPlanCard = true;
            }
        }
    }
}
