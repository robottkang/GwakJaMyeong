using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Card;

namespace Room
{
    public class HandManager : MonoBehaviour
    {
        public static HandManager Instance { get; private set; }

        [Header("- References")]
        [SerializeField]
        private GameObject handCardPrefab;

        public const string handPoolName = "Hand Pool";

        private void Awake()
        {
            Instance = this;
        }

        public void AddCard(CardData cardData)
        {
            var cardObject = ObjectPool.GetObject(handPoolName, transform);
            cardObject.GetComponent<RectTransform>().sizeDelta = handCardPrefab.GetComponent<RectTransform>().sizeDelta * 0.8f;

            cardObject.GetComponent<RoomCard>().CardData = cardData;
        }
    }
}
