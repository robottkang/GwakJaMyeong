using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Card;

namespace Room
{
    public class StrategyPlan : MonoBehaviour
    {
        private GameObject placedCardObject;
        [SerializeField, ReadOnly]
        private CardInfo placedCardInfo;
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

                IdeaCard ideaCard;
                placedCardObject = ObjectPool.GetObject("Card Pool", transform);
                (ideaCard = placedCardObject.GetComponent<IdeaCard>()).CurrentStrategyPlan = this;
                ideaCard.SetCardSpriteColor(new Color(1f, 1f, 1f, .5f));
                
                placedCardObject.transform.SetPositionAndRotation(transform.position + Vector3.up * 0.08f, Quaternion.Euler(0f, 0f, 0f));
            }
        }

        public void ClearStrategyPlan()
        {
            placedCardInfo = null;

            if (placedCardObject != null)
                ObjectPool.ReturnObject("Card Pool", placedCardObject);
            placedCardObject.GetComponent<IdeaCard>().SetCardSpriteColor(Color.white);
            placedCardObject = null;
        }
    }
}
