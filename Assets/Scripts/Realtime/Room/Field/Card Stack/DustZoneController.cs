using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using Card;

namespace Room
{
    public class DustZoneController : MonoBehaviour
    {
#if UNITY_EDITOR
        [Button] private void ClearFieldTest() => ClearStrategyPlans();
        [Button] private void EmptyTest() => EmptyDust();
#endif
        private CardStackController cardStackController;
        private Queue<CardData> cardsInDust = new(9);

        public CardData[] CardsInDust => cardsInDust.ToArray();

        private void Awake()
        {
            cardStackController = GetComponent<CardStackController>();

            PhaseEventBus.Subscribe(Phase.End, ClearStrategyPlans);
        }

        public void ClearStrategyPlans()
        {
            for (int i = 0; i < 3; i++)
            {
                cardsInDust.Enqueue(PlayerController.Instance.StrategyPlans[i].FirstPlacedPlanCard.CardData);
                PlayerController.Instance.StrategyPlans[i].ClearStrategyPlan();
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
