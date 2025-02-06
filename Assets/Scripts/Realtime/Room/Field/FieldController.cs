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
        protected PostureController postureCtrl;
        protected int hp = 10;

        public static bool IsChangingAnyPosture => PlayerController.Instance.PostureCtrl.IsPostureChanging
            || Opponent.OpponentController.Instance.PostureCtrl.IsPostureChanging;

        public int Hp => hp;
        public PostureController PostureCtrl => postureCtrl;
        public StrategyPlan[] StrategyPlans => strategyPlans;
        public Card.PlanCard CurrentCard => strategyPlans[DuelManager.StrategyPlanOrder].PlacedPlanCardOnTop;
        public abstract FieldController OpponentField { get; }

        public Card.PlanCard GetCard(int index)
        {
            return strategyPlans[index].PlacedPlanCardOnTop;
        }

        public void PlaceCard(GameObject planCard)
        {
            strategyPlans[DuelManager.StrategyPlanOrder].PlaceCard(planCard);
        }

        public virtual void ApplyDamage(int damage)
        {
            hp -= damage;

            if (hp <= 0)
            {
                DuelManager.Instance.EndGame(this);
            }
        }
    }
}
