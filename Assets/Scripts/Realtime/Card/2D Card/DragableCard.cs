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
        private CardInfo cardInfo;
        protected Vector3 offset;
        private Vector3 originPosition;
        [HideInInspector]
        public Transform targetParent;

        protected Image image;

        public CardInfo CardInfo
        {
            get => cardInfo;
            set
            {
                cardInfo = value;
                if (value != null) image.sprite = cardInfo.CardSprite;
                else Debug.LogWarning("null is invaild for cardInfo");
            }
        }
        protected Vector3 OriginPosition => originPosition;

        protected virtual void Awake()
        {
            image = GetComponent<Image>();
        }

        protected virtual void Start()
        {
            targetParent = transform.parent;
            image.sprite = cardInfo.CardSprite;
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            image.raycastTarget = false;
            transform.SetParent(transform.parent.parent);

            originPosition = transform.position;
            offset = transform.position - Input.mousePosition;
            offset.z = 0f;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition + offset;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            transform.SetParent(targetParent);
            image.raycastTarget = true;
        }
    }
}
