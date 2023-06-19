using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room
{
    public class CardStackController : MonoBehaviour
    {
        [Header("- Reference")]
        public GameObject cardPrefab;
        
        [Header("- Setting")]
        public bool activatesCardOnGeneration = true;
        public int cardCount = 0;
        public float cardOffsetY = 0.08f;
        private int lastCardIndex;
        private List<GameObject> cardObjects = new();

        private void Awake()
        {
            GenerateCard(cardCount);
        }

        private void GenerateCard(int count)
        {
            cardObjects = new(count);
            lastCardIndex = activatesCardOnGeneration ? 0 : count - 1;

            for (int i = 0; i < count; i++)
            {
                GameObject cardObject = Instantiate(cardPrefab);
                cardObjects.Add(cardObject);

                Vector3 cardPosition = transform.position + i * cardOffsetY * Vector3.up;

                cardObject.SetActive(activatesCardOnGeneration);
                cardObject.transform.SetParent(transform);
                cardObject.transform.SetPositionAndRotation(cardPosition, Quaternion.Euler(0f, 0f, 180f));
            }
        }

        public void DrawCard(int count)
        {
            lastCardIndex += 1;
            
            while (count > 0)
            {
                lastCardIndex -= 1;
                count -= 1;

                cardObjects[lastCardIndex].SetActive(false);
            }
        }

        public void StackCard(int count)
        {
            lastCardIndex -= 1;

            while (count > 0)
            {
                cardObjects[lastCardIndex].SetActive(true);

                lastCardIndex += 1;
                count += 1;
            }
        }
    }
}
