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
        [Button] private void ClearFieldTest() => ClearStrategyPlans();
        [Button] private void EmptyTest() => EmptyDust();
#endif
        private CardStackController cardStackController;
        private StrategyPlan[] strategyPlan;
        private Queue<CardInfo> cardsInDust = new(9);

        public CardInfo[] CardsInDust => cardsInDust.ToArray();

        private void Awake()
        {
            GameManager gameManager = GameManager.gameManager;
            cardStackController = GetComponent<CardStackController>();

            strategyPlan = new StrategyPlan[3]
            {
                gameManager.FirstIdeaCard,
                gameManager.SecondIdeaCard,
                gameManager.ThirdIdeaCard
            };

            PhaseEventBus.Subscribe(Phase.End, ClearStrategyPlans);
        }

        public void ClearStrategyPlans()
        {
            for (int i = 0; i < 3; i++)
            {
                cardsInDust.Enqueue(strategyPlan[i].PlacedCardInfo);
                strategyPlan[i].ClearStrategyPlan();
            }

            cardStackController.StackCard(3);
        }

        public void EmptyDust()
        {
            cardStackController.DrawCard(cardsInDust.Count);
            cardsInDust.Clear();
        }
    }
}
