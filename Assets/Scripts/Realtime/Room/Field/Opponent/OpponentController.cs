using Card;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace Room.Opponent
{
    public class OpponentController : FieldController, IInRoomCallbacks, IOnEventCallback
    {
        [SerializeField]
        private CardStackController deck;
        [SerializeField]
        private CardStackController dust;
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
            if (PhotonNetwork.IsConnected &&
                PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
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

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) { }

        public void OnMasterClientSwitched(Player newMasterClient) { }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.AreEventCodesEqual(DuelEventCode.Ready))
            {
                IsReadyToPlay = photonEvent.ConvertEventData<bool>().content;
            }

            if (photonEvent.AreEventCodesEqual(DuelEventCode.SendCardData) && PhaseManager.CurrentPhase == Phase.StrategyPlan)
            {
                List<CardData.CardCode> cardCodeList = photonEvent.ConvertEventData<List<CardData.CardCode>>().content;
                Debug.Log($"Receive {cardCodeList[0]}, {cardCodeList[1]}, {cardCodeList[2]}");

                for (int i = 0; i < cardCodeList.Count; i++)
                {
                    GameObject planCardObj = ObjectPool.GetObject("Card Pool");
                    planCardObj.GetComponent<PlanCard>().Initialize(cardCodeList[i], this);
                    planCardObj.transform.rotation = Quaternion.Euler(0, 0, 180f);
                    planCardObj.GetComponent<PlanCard>().CanMove = false;
                    StrategyPlans[i].PlaceCard(planCardObj);
                }

                IsReadyPlanCard = true;
            }

            if (photonEvent.AreEventCodesEqual(DuelEventCode.SetCardDepolyment))
            {
                var data = photonEvent.ConvertEventData<CardDeployment>();

                if (data.TargetUser == UserType.Opponent)
                {
                    CurrentCard.CurrentDeployment = data.content;

                    switch (CurrentCard.CurrentDeployment)
                    {
                        case CardDeployment.Opened:
                            CurrentCard.Open();
                            break;
                        case CardDeployment.Turned:
                            CurrentCard.Turn();
                            break;
                        case CardDeployment.Disabled:
                            CurrentCard.Disable();
                            break;
                        default:
                            throw new System.NotImplementedException();
                    }
                }
            }
        }
    }
}
