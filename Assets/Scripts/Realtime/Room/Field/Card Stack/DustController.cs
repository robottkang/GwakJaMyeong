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
        private Queue<CardInfo> cardsInDust = new(9);

        public CardInfo[] CardsInDust => cardsInDust.ToArray();

        private void Awake()
        {
            GameManager gameManager = GameManager.Instance;
            cardStackController = GetComponent<CardStackController>();

            PhaseEventBus.Subscribe(Phase.End, ClearStrategyPlans);
        }

        public void ClearStrategyPlans()
        {
            for (int i = 0; i < 3; i++)
            {
                cardsInDust.Enqueue(GameManager.Instance.StrategyPlans[i].PlacedCardInfo);
                GameManager.Instance.StrategyPlans[i].ClearStrategyPlan();
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
