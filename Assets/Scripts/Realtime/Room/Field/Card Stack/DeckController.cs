using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using Card;

namespace Room
{
    public class DeckController : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private List<CardInfo> testCardList;
        [Button] private void SetCard()
        {
            if (!Application.isPlaying) return;
            deck = Shuffle(testCardList);
        }
#endif
        [Header("- Reference")]
        private CardStackController cardStackController;
        [SerializeField]
        private DustController dustController;

        [Header("- Setting")]
        [SerializeField]
        private int firstDrawCount = 5;
        [SerializeField]
        private int defaultDrawCount = 3;

        public static List<CardInfo> deckList = new(11);
        private List<CardInfo> deck = new(11);

        public CardInfo[] Deck => deck.ToArray();
        
        
        private void Awake()
        {
            cardStackController = GetComponent<CardStackController>();
            deck = new(deckList);
            
            void Draw()
            {
                if (deck.Count < defaultDrawCount)
                {
                    deck.AddRange(dustController.CardsInDust);
                    dustController.EmptyDust();
                    deck = Shuffle(deck);
                    cardStackController.StackCard(9);
                }

                this.Draw(PhaseManager.Instance.TurnCount == 1 ? firstDrawCount : defaultDrawCount);
            }

            PhaseEventBus.Subscribe(Phase.Draw, Draw);
        }

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Shuffle the cards
        /// </summary>
        /// <param name="cards">the cards to shuffle</param>
        /// <returns>return the shuffled cards</returns>
        public List<CardInfo> Shuffle(List<CardInfo> cards)
        {
            List<CardInfo> resultCardsList = new(cards.Count);
            
            while (cards.Count > 0)
            {
                int randomIndex = Random.Range(0, cards.Count);
                resultCardsList.Add(cards[randomIndex]);
                cards.RemoveAt(randomIndex);
            }
#if UNITY_EDITOR
            string debugCardOrder = "shuffled card: ";
            foreach (var card in resultCardsList)
            {
                debugCardOrder += '\n' + card.CardName;
            }
            Debug.Log(debugCardOrder);
#endif
            return resultCardsList;
        }

        public void Draw(int count)
        {
            cardStackController.DrawCard(count);

            while (count > 0)
            {
                if (deck.Count <= 0)
                {
                    Debug.LogError("You try Drawing, but deck is empty.");
                    return;
                }

                int deckIndex = deck.Count - 1;
                HandManager.Instance.AddCard(deck[deckIndex]);
                deck.RemoveAt(deckIndex);

                count -= 1;
            }
        }

        public void Initialize()
        {
            deck = Shuffle(deck);
        }
    }
}