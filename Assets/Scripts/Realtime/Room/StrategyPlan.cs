using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room
{
    public class StrategyPlan : MonoBehaviour
    {
        [SerializeField]
        private CardInfo placedCard;
        public CardInfo PlacedCard
        {
            get => placedCard;
            set
            {
                placedCard = value;
                var cardObject = ObjectPool.GetObject("Card Pool");
                cardObject.GetComponent<CardController>().CardInfo = value;
                cardObject.transform.SetParent(transform);
                cardObject.transform.SetPositionAndRotation(transform.position + Vector3.up * 0.08f, Quaternion.Euler(0f, 0f, 180f));
            }
        }
    }
}
