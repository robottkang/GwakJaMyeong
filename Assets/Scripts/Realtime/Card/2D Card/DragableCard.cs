using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Card
{
    public class DragalbeCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private CardData cardData;
        protected Vector3 offset;
        private Vector3 prevPosition;
        private Transform originParent;

        protected Image image;

        public CardData CardData
        {
            get => cardData;
            set
            {
                cardData = value;
                if (value != null) image.sprite = cardData.CardSprite;
                else Debug.LogWarning("null is invaild for cardInfo");
            }
        }
        protected Vector3 PrevPosition => prevPosition;
        protected Transform OriginParent => originParent;

        protected virtual void Awake()
        {
            image = GetComponent<Image>();
        }

        protected virtual void Start()
        {
            originParent = transform.parent;
            image.sprite = cardData.CardSprite;
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            image.raycastTarget = false;
            transform.SetParent(GetComponentInParent<Canvas>().transform);

            prevPosition = transform.position;
            offset = transform.position - Input.mousePosition;
            offset.z = 0f;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition + offset;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            image.raycastTarget = true;
        }
    }
}
