using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using EasyButtons;

namespace Room
{
    public class DeckController : MonoBehaviour
    {
#if UNITY_EDITOR
        [Button] private void DrowTest() => Drow();
        [Button] private void SpawnTestCard()
        {
            for (int i = 0; i < 11; i++)
            {
                deck.Add(new());
            }
            Start();
        }
#endif
        public static List<CardInfo> deckList = new(11);
        private List<CardInfo> deck = new(11);
        private List<GameObject> cardObjects = new(11);
        [SerializeField]
        private GameObject cardPrefab;
        [SerializeField]
        private HandController handController;

        private readonly string cardPoolName = "Card Pool";
        private GameManager gameManager;
        
        private void Awake()
        {
            gameManager = GameManager.gameManager;
        }

        private void Start()
        {
            Initialize();

            for (int i = 0; i < deck.Count; i++)
            {
                CardInfo card = deck[i];
                Vector3 cardPosition = gameManager.DeckTransform.position + (i + 1) * 0.08f * Vector3.up;

                GameObject spawnedCard = ObjectPool.GetObject(cardPoolName, cardPrefab);
                spawnedCard.transform.position = cardPosition;
                spawnedCard.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                spawnedCard.GetComponent<CardController>().CardInfo = card;

                cardObjects.Add(spawnedCard);
            }

            Drow(5);
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
#if UNITY_EDITOR
                Debug.Log(cards[randomIndex] + " " + randomIndex);
#endif
                resultCardsList.Add(cards[randomIndex]);
                cards.RemoveAt(randomIndex);
            }
            
            return resultCardsList;
        }

        public void Drow(int count = 1)
        {
            while (count > 0)
            {
                if (deck.Count <= 0) return;

                int deckIndex = deck.Count - 1;
                CardInfo card = deck[deckIndex];
                handController.AddCard(card);
                ObjectPool.ReturnObject(cardPoolName, cardObjects[deckIndex]);
                deck.RemoveAt(deckIndex);

                count--;
            }
        }

        public void Initialize()
        {
            deck = Shuffle(deckList);
        }
    }
}