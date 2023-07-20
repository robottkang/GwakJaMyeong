using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Card;

namespace DeckBuilding
{
    public class Inventory : MonoBehaviour, IDropHandler
    {
        [SerializeField]
        private RectOffset padding;
        [SerializeField]
        private Vector2 spacing;

        private List<GameObject> content = new();

        public void OnDrop(PointerEventData eventData)
        {
            if (transform.childCount >= 11) return;

            AddToInventory(eventData.pointerDrag);
        }

        private void AddToInventory(GameObject card)
        {
            card.transform.SetParent(transform);

            content.Add(card);
            SortChildren();
        }

        private void SortChildren()
        {
            content.Sort((x, y) => string.Compare(x.name, y.name));

            // 자식 순서를 정렬된 리스트 순서로 업데이트
            for (int i = 0; i < content.Count; i++)
            {
                content[i].transform.SetSiblingIndex(i);
            }

        }
    }
}
