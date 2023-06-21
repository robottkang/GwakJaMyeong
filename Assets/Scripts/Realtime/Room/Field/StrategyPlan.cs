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
        private CardInfo placedCardInfo;

        public GameObject PlacedCardObejct => placedCardObject;
        public CardInfo PlacedCardInfo
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
                placedCardObject.GetComponent<IdeaCard>().CurrentStrategyPlan = this;
                
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
    }
}
