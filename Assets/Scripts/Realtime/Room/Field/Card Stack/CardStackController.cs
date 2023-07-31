using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room
{
    public class CardStackController : MonoBehaviour
    {
#if UNITY_EDITOR
        [EasyButtons.Button] private void DrawTest(int count) => DrawCard(count);
        [EasyButtons.Button] private void StackTest(int count) => StackCard(count);
#endif
        [Header("- Reference")]
        public GameObject cardPrefab;
        
        [Header("- Setting")]
        public bool activatesCardOnGeneration = false;
        public int cardCount = 0;
        public float cardOffsetY = 0.08f;
        private int lastCardIndex;
        private List<GameObject> cardObjects = new();

        public GameObject[] CardObjects => cardObjects.ToArray();
        public int LastCardIndex => lastCardIndex;

        private void Awake()
        {
            GenerateCard(cardCount);
        }

        private void GenerateCard(int count)
        {
            cardObjects = new(count);
            lastCardIndex = activatesCardOnGeneration ? count - 1 : -1;

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
            while (count > 0 && lastCardIndex >= 0)
            {
                cardObjects[lastCardIndex].SetActive(false);

                lastCardIndex -= 1;
                count -= 1;
            }
        }

        public void StackCard(int count)
        {
            while (count > 0 && lastCardIndex < cardObjects.Count)
            {
                lastCardIndex += 1;
                count -= 1;
                
                cardObjects[lastCardIndex].SetActive(true);
            }
        }
    }
}
