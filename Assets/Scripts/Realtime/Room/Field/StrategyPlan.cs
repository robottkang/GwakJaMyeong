using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Card;

namespace Room
{
    public class StrategyPlan : MonoBehaviour
    {
        [SerializeField]
        private GameObject cardPrefab;
        private GameObject placedCardObject;
        [SerializeField, ReadOnly]
        private PlanCardInfo placedCardInfo;

        public GameObject PlacedCardObejct => placedCardObject;
        public PlanCardInfo PlacedCardInfo
        {
            get => placedCardInfo;
            set
            {
                placedCardInfo = value;

                if (placedCardInfo == null)
                {
                    ClearStrategyPlan();
                    return;
                }

                placedCardObject = ObjectPool.GetObject("Card Pool", cardPrefab);
                placedCardObject.GetComponent<PlanCard>().CurrentStrategyPlan = this;
                
                placedCardObject.transform.SetPositionAndRotation(transform.position + Vector3.up * 0.08f, Quaternion.Euler(0f, 0f, 0f));
            }
        }

        public void ClearStrategyPlan()
        {
            placedCardInfo = null;

            if (placedCardObject != null)
                ObjectPool.ReturnObject("Card Pool", placedCardObject);
            placedCardObject = null;
        }

        public class PlanCardInfo : CardInfo
        {
            public CardState cardState = CardState.Placed;

            public enum CardState
            {
                Placed,
                Opened,
                Turned,
            }
        }
    }
}
