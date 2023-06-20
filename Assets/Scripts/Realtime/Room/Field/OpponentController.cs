using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Room
{
    public class OpponentController : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private CardStackController deck;
        [SerializeField]
        private CardStackController dust;

        private void Awake()
        {
            PhaseEventBus.Subscribe(Phase.Draw, () =>
            {
                if (deck.CardObjects.Length == 0)
                {
                    dust.DrawCard(9);
                    deck.StackCard(9);
                }

                deck.DrawCard(GameManager.gameManager.TurnCount == 1 ? 5 : 3);
            });

            PhaseEventBus.Subscribe(Phase.End, () =>
            {
                dust.StackCard(3);
            });
        }

        private void Start()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                deck.StackCard(deck.cardCount);
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            deck.StackCard(deck.cardCount);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            deck.DrawCard(deck.cardCount);
        }
    }
}
