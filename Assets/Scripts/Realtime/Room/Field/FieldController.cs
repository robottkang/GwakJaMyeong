using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Room
{
    public abstract class FieldController : MonoBehaviourPun
    {
        [Header("- References")]
        [SerializeField]
        private StrategyPlan[] strategyPlans = new StrategyPlan[3];
        [SerializeField]
        protected IPostureController postureController;
        protected int hp = 10;

        public int Hp => hp;
        public IPostureController PostureController => postureController;
        public Card.PlanCard CurrentCard => strategyPlans[DuelManager.StrategyPlanOrder].PlacedPlanCard;

        public Card.PlanCard GetCard(int index)
        {
            return strategyPlans[index].PlacedPlanCard;
        }

        public void PlaceCard(GameObject planCard)
        {
            strategyPlans[DuelManager.StrategyPlanOrder].PlaceCard(planCard);
        }

        public virtual void TakeDamage(int damage)
        {
            hp -= damage;
        }
    }
}
