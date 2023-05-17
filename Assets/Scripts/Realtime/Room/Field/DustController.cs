using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using Card;

namespace Room
{
    public class DustController : MonoBehaviour
    {
#if UNITY_EDITOR
        [Button] private void ClearTest() => ClearStrategyPlans();
        [Button] private void PassTest() => PassDust2Deck();
#endif
        private StrategyPlan[] strategyPlan;
        private Queue<CardInfo> cardsInDust = new(9);
        private List<GameObject> cardObjectsInDust = new(9);

        private void Awake()
        {
            GameManager gameManager = GameManager.gameManager;
            strategyPlan = new StrategyPlan[3]
            {
                gameManager.FirstIdeaCard,
                gameManager.SecondIdeaCard,
                gameManager.ThirdIdeaCard
            };
        }

        public void ClearStrategyPlans()
        {
            for (int i = 0; i < 3; i++)
            {
                cardsInDust.Enqueue(strategyPlan[i].PlacedCardInfo);
                strategyPlan[i].ClearStrategyPlan();
                GameObject cardObject = ObjectPool.GetObject("Card Pool", transform);
                cardObjectsInDust.Add(cardObject);
                
                Vector3 cardPosition = transform.position + cardsInDust.Count * 0.08f * Vector3.up;

                cardObject.GetComponent<IdeaCard>().CardInfo = cardsInDust.Peek();
                cardObject.transform.SetPositionAndRotation(cardPosition, Quaternion.Euler(0f, 0f, 0f));
            }
        }

        public void PassDust2Deck()
        {
            int cardsInDustCount = cardsInDust.Count;
            DeckController deckController = GameManager.gameManager.DeckController;

            for (int i = 0; i < cardsInDustCount; i++)
            {
                CardInfo card = cardsInDust.Dequeue();
                deckController.SpawnCard(card);
                deckController.Deck.Add(card);
                ObjectPool.ReturnObject("Card Pool", cardObjectsInDust[i]);
            }

            deckController.Deck = deckController.Shuffle(deckController.Deck);
        }
    }
}
