using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Card;

namespace DeckBuilding
{
    public class Inventory : MonoBehaviour, IDropHandler
    {
        private static List<GameObject> content = new();
        public static GameObject[] Content => content.ToArray();
        [SerializeField]
        private int maxCardCount = 11;

        private void Awake()
        {
            content.Clear();
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (transform.childCount >= maxCardCount) return;

            AddCard(eventData.pointerDrag);
        }

        private void AddCard(GameObject card)
        {
            card.transform.SetParent(transform);
            card.GetComponent<DeckBuildingCard>().addedToInventory = true;

            content.Add(card);
            SortChildren();
        }

        public static void RemoveCard(GameObject card)
        {
            content.Remove(card);
            SortChildren();
        }

        private static void SortChildren()
        {
            content.Sort((x, y) => (x.GetComponent<DragalbeCard>().CardData.ThisCardCode > y.GetComponent<DragalbeCard>().CardData.ThisCardCode) ? 1 : -1);

            // 자식 순서를 정렬된 리스트 순서로 업데이트
            for (int i = 0; i < content.Count; i++)
            {
                content[i].transform.SetSiblingIndex(i);
            }
        }
    }
}
