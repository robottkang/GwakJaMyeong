using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using Card;

namespace Room
{
    public class DeckZoneController : MonoBehaviour
    {
        [Header("- Reference")]
        [SerializeField]
        private CardStackController cardStackController;
        [SerializeField]
        private DustZoneController dustZoneController;

        [Header("- Setting")]
        [SerializeField]
        private int firstDrawCount = 5;
        [SerializeField]
        private int defaultDrawCount = 3;

        public static List<CardData> deckList = new(11);
        private List<CardData> deck = new(11);

        public CardData[] Deck => deck.ToArray();
        
        
        private void Awake()
        {
            PhaseEventBus.Subscribe(Phase.Launch, () => Draw(firstDrawCount));
            PhaseEventBus.Subscribe(Phase.Draw, () => Draw(defaultDrawCount));
        }

        private void Start()
        {
            SetDeck(deckList);

            cardStackController.OnDrawCard.AddListener(() =>
            {
                int deckIndex = deck.Count - 1;
                HandManager.Instance.AddCard(deck[deckIndex]);
                deck.RemoveAt(deckIndex);
            });
        }

        /// <summary>
        /// Shuffle the cards
        /// </summary>
        /// <param name="cards">the cards to shuffle</param>
        /// <returns>return the shuffled cards</returns>
        public static List<CardData> Shuffle(List<CardData> cards)
        {
            List<CardData> resultCardsList = new(cards);

            var count = resultCardsList.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = Random.Range(i, count);
                var tmp = resultCardsList[i];
                resultCardsList[i] = resultCardsList[r];
                resultCardsList[r] = tmp;
            }
#if UNITY_EDITOR
            string debugCardOrder = "shuffled card: ";
            foreach (var card in resultCardsList)
            {
                debugCardOrder += '\n' + card.ThisCardCode.ToString();
            }
            Debug.Log(debugCardOrder);
#endif
            return resultCardsList;
        }

        public void SetDeck(List<CardData> cards)
        {
            deck = new(cards);
        }

        public void Draw(int count)
        {
            if (deck.Count < defaultDrawCount)
            {
                deck.AddRange(dustZoneController.CardsInDust);
                dustZoneController.EmptyDust();
                deck = Shuffle(deck);
                cardStackController.StackCard(9);
            }

            cardStackController.DrawCard(count);
        }
    }
}