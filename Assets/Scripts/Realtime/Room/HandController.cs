using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room
{
    public class HandController : MonoBehaviour
    {
        [SerializeField]
        private GameObject handCardPrefab;

        private readonly string handPoolName = "Hand Pool";
        
        public void AddCard(CardInfo cardInfo)
        {
            var cardObject = ObjectPool.GetObject(handPoolName, handCardPrefab);
            cardObject.GetComponent<RectTransform>().sizeDelta *= 0.8f;
            cardObject.transform.SetParent(transform);

            cardObject.GetComponent<DragalbeCard>().CardInfo = cardInfo;
        }
    }
}
