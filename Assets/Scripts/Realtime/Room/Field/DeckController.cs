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
        [Button] public void DrowTest() => Drow();
        [Button] public void SpawnTestCard()
        {
            deck = new(deckList);
            
            for (int i = 0; i < 11 - deck.Count; i++)
            {
                deck.Add(new());
            }
            
            Start();
        }
#endif
        [Header("- Reference")]
        [SerializeField]
        private GameObject cardPrefab;
        [SerializeField]
        private HandController handController;

        [Header("- Setting")]
        [SerializeField]
        private int firstDrowCount = 5;
        [SerializeField]
        private int defaultDrowCount = 3;

        public static List<CardInfo> deckList = new(11);
        private List<CardInfo> deck = new(11);
        private List<GameObject> cardObjects = new(11);

        public List<CardInfo> Deck => deck;
        public int DefaultDrowCount => defaultDrowCount;

        private const string cardPoolName = "Card Pool";
        
        private void Awake()
        {
            deck = new(deckList);

            void FirstDrow()
            {
                Drow(firstDrowCount);
                PhaseEventBus.Unsubscribe(Phase.Drow, FirstDrow);
                PhaseEventBus.Subscribe(Phase.Drow, BasicDrow);
            }
            void BasicDrow()
            {
                if (deck.Count < defaultDrowCount)
                {
                    GameManager.gameManager.DustController.FillDeck();
                }

                Drow(defaultDrowCount);
            }

            PhaseEventBus.Subscribe(Phase.Drow, FirstDrow);
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
            string debugCardOrder = "shuffled card: ";
            
            while (cards.Count > 0)
            {
                int randomIndex = Random.Range(0, cards.Count);
#if UNITY_EDITOR
                debugCardOrder += "\n(" + cards[randomIndex].CardName + ", " + randomIndex + ")";
#endif
                resultCardsList.Add(cards[randomIndex]);
                cards.RemoveAt(randomIndex);
            }
#if UNITY_EDITOR
            Debug.Log(debugCardOrder);
#endif
            return resultCardsList;
        }

        public void ShuffleDeck()
        {
            deck = Shuffle(deck);
        }

        public void Drow(int count = 1)
        {
            while (count > 0)
            {
                if (deck.Count <= 0)
                {
                    Debug.LogError("You try drowing, but deck is empty.");
                    return;
                }

                int deckIndex = deck.Count - 1;
                CardInfo card = deck[deckIndex];
                handController.AddCard(card);
                ObjectPool.ReturnObject(cardPoolName, cardObjects[deckIndex]);
                cardObjects.RemoveAt(deckIndex);
                deck.RemoveAt(deckIndex);

                count -= 1;
            }
        }

        public void SpawnCard()
        {
            GameObject spawnedCard = ObjectPool.GetObject(cardPoolName, cardPrefab);
            cardObjects.Add(spawnedCard);

            Vector3 cardPosition = transform.position + cardObjects.Count * 0.08f * Vector3.up;
            
            spawnedCard.transform.SetPositionAndRotation(cardPosition, Quaternion.Euler(0f, 0f, 180f));
        }

        public void Initialize()
        {
            deck = Shuffle(deck);

            int deckCount = deck.Count;
            for (int i = 0; i < deckCount; i++)
            {
                SpawnCard();
            }
        }
    }
}