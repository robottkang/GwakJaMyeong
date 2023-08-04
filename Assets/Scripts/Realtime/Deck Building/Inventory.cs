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
        private static List<GameObject> content = new();
        public static GameObject[] Content => content.ToArray();

        public static Inventory Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (transform.childCount >= 11) return;

            AddToInventory(eventData.pointerDrag);
        }

        public static void AddToInventory(GameObject card)
        {
            content.Add(card);
            SortChildren();
        }

        public static void RemoveFromInventory(GameObject card)
        {
            content.Remove(card);
            SortChildren();
        }

        private static void SortChildren()
        {
            content.Sort((x, y) => string.Compare(x.name, y.name));

            for (int i = 0; i < content.Count; i++)
            {
                RectTransform rectTransform = Instance.GetComponent<RectTransform>();
                content[i].transform.position = new(
                    rectTransform.anchoredPosition.x - rectTransform.rect.width / 2 + Instance.padding.left + content[i].GetComponent<RectTransform>().rect.width + (Instance.spacing.x + content[i].GetComponent<RectTransform>().rect.width) * i,
                    rectTransform.anchoredPosition.y + rectTransform.rect.height / 2 + Instance.padding.bottom);
            }
        }
    }
}
