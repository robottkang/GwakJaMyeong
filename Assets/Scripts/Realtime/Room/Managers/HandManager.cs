using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Card;

namespace Room
{
    public class HandManager : MonoBehaviour
    {
        public static HandManager Instance { get; private set; }
        
        [SerializeField]
        private GameObject handCardPrefab;

        private readonly string handPoolName = "Hand Pool";

        private void Awake()
        {
            Instance = this;
        }

        public void AddCard(CardInfo cardInfo)
        {
            var cardObject = ObjectPool.GetObject(handPoolName, handCardPrefab, transform);
            cardObject.GetComponent<RectTransform>().sizeDelta = handCardPrefab.GetComponent<RectTransform>().sizeDelta * 0.8f;

            cardObject.GetComponent<DragalbeCard>().CardInfo = cardInfo;
        }
    }
}
