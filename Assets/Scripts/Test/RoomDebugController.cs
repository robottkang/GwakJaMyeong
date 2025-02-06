using Card;
using Card.Posture;
using Cysharp.Threading.Tasks;
using Room.Opponent;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Card.CardData;
using EasyButton = EasyButtons.ButtonAttribute;

namespace Room.Diagnostics
{
    public class RoomDebugController : MonoBehaviour
    {
        [Header("- ChangePhase")]
        [SerializeField] private Phase targetPhase = Phase.BeforeStart;
        [Header("- SetMyDeck")]
        [SerializeField] private bool shuffleMyDeck = true;
        [SerializeField] private List<CardData> myDeck = new();
        [Header("- SetActionToken")]
        [SerializeField] private UserType targetActionToken = UserType.Player;
        [Header("- ChangeOpponentPosture")]
        [SerializeField] private PostureType opponentPosture = PostureType.None;
        [Header("- SetOpponentStrategyPlans")]
        [SerializeField] private List<CardData> opponentStrategyPlans = new();
        [Header("- ControlOpponentPlanCard")]
        [SerializeField] private CardDeployment targetDeployment = CardDeployment.Placed;

        [EasyButton(Mode = EasyButtons.ButtonMode.EnabledInPlayMode)]
        private void Test(TestMethod method)
        {
            switch (method)
            {
                case TestMethod.ChangePhase:
                    ChangePhase();
                    break;
                case TestMethod.SetMyDeck:
                    SetMyDeck();
                    break;
                case TestMethod.SetActionToken:
                    SetActionToken();
                    break;
                case TestMethod.ChangeOpponentPosture:
                    ChangeOpponentPosture();
                    break;
                case TestMethod.SetOpponentStrategyPlans:
                    SetOpponentStrategyPlans();
                    break;
                case TestMethod.ControlOpponentPlanCard:
                    ControlOpponentPlanCard();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void Start()
        {
            if (!GameManager.Instance.EnabledDebug)
            {
                Destroy(this);
            }
        }

        private void SetMyDeck()
        {
            string debugCardList = "Set My Deck: ";
            foreach (var card in myDeck)
            {
                debugCardList += '\n' + card.ThisCardCode.ToString();
            }
            Debug.Log(debugCardList);
            
            FindObjectOfType<DeckZoneController>()
                .SetDeck(shuffleMyDeck ? DeckZoneController.Shuffle(myDeck) : myDeck);
        }

        private void SetActionToken()
        {
            DuelManager.SetPlayerActionToken(targetActionToken);
        }

        private void ChangePhase()
        {
            PhaseManager.CurrentPhase = targetPhase;
        }

        private void ChangeOpponentPosture()
        {
            var eventData = new ExitGames.Client.Photon.EventData() { Code = (byte)DuelEventCode.SendPosture };
            eventData.Parameters[Photon.Realtime.ParameterCode.Data] = JsonUtility.ToJson(new RaiseEventData<PostureType>(UserType.Player, opponentPosture));

            (FindObjectOfType<OpponentController>().PostureCtrl as OpponentPostureController).OnEvent(eventData);
        }

        private void SetOpponentStrategyPlans()
        {
            var cardCodes = new List<CardData.CardCode>() {
                opponentStrategyPlans[0].ThisCardCode,
                opponentStrategyPlans[1].ThisCardCode,
                opponentStrategyPlans[2].ThisCardCode
            };

            var eventData = new ExitGames.Client.Photon.EventData() { Code = (byte)DuelEventCode.SendCardData };
            eventData.Parameters[Photon.Realtime.ParameterCode.Data] = JsonUtility.ToJson(new RaiseEventData<List<CardData.CardCode>>(UserType.Opponent, cardCodes));
            eventData.Parameters[Photon.Realtime.ParameterCode.ActorNr] = 1;

            FindObjectOfType<OpponentController>().OnEvent(eventData);
        }

        private void ControlOpponentPlanCard()
        {
            switch (targetDeployment)
            {
                case CardDeployment.Placed:
                    OpponentController.Instance.CurrentCard.CurrentDeployment = CardDeployment.Placed;
                    break;
                case CardDeployment.Opened:
                    OpponentController.Instance.CurrentCard.Open();
                    break;
                case CardDeployment.Turned:
                    OpponentController.Instance.CurrentCard.Turn();
                    break;
                case CardDeployment.Disabled:
                    OpponentController.Instance.CurrentCard.CurrentDeployment = CardDeployment.Disabled;
                    break;
                default:
                    throw new NotImplementedException();
            }
            DuelManager.TurnOverActionToken();

            var eventData = new ExitGames.Client.Photon.EventData() { Code = (byte)DuelEventCode.CompleteCardAction };
            eventData.Parameters[Photon.Realtime.ParameterCode.ActorNr] = 1;
            DuelManager.Instance.OnEvent(eventData);
        }

        private enum TestMethod
        {
            ChangePhase,
            SetMyDeck,
            SetActionToken,
            ChangeOpponentPosture,
            SetOpponentStrategyPlans,
            ControlOpponentPlanCard,
        }
    }
}
