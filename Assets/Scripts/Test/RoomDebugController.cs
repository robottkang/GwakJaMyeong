using Card;
using Card.Posture;
using Room.Opponent;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButton = EasyButtons.ButtonAttribute;

namespace Room.Diagnostics
{
    public class RoomDebugController : MonoBehaviour
    {
        [SerializeField] private Phase targetPhase = Phase.BeforeStart;
        [Space]
        [SerializeField] private bool shuffleMyDeck = true;
        [SerializeField] private List<CardData> myDeck = new();
        [Space]
        [SerializeField] private UserType playerActionToken = UserType.Player;
        [SerializeField] private PostureType opponentPosture = PostureType.None;
        [SerializeField] private List<CardData> opponentStrategyPlans = new();

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
            DuelManager.SetPlayerActionToken(playerActionToken);
        }

        private void ChangePhase()
        {
            PhaseManager.CurrentPhase = targetPhase;
        }

        private void ChangeOpponentPosture()
        {
            FindObjectOfType<OpponentController>().PostureCtrl.ChangePosture(opponentPosture);
        }

        private void SetOpponentStrategyPlans()
        {
            for (int i = 0; i < 3; i++)
            {
                var eventData = new ExitGames.Client.Photon.EventData() { Code = (byte)DuelEventCode.SendCardsData };
                eventData.Parameters[Photon.Realtime.ParameterCode.Data] = JsonUtility.ToJson(opponentStrategyPlans[i]);
                eventData.Parameters[Photon.Realtime.ParameterCode.ActorNr] = 1;

                FindObjectOfType<OpponentController>().OnEvent(eventData);
            }
        }

        private enum TestMethod
        {
            ChangePhase,
            SetMyDeck,
            SetActionToken,
            ChangeOpponentPosture,
            SetOpponentStrategyPlans
        }
    }
}
