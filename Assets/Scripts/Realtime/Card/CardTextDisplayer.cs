using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Card.UI
{
    public class CardTextDisplayer : MonoBehaviour
    {
        [SerializeField]
        private Image cardSprite;
        [SerializeField]
        private TextMeshProUGUI cardText;
        private bool isDisplaying = false;

        public CardTextDisplayer Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else Destroy(gameObject);
        }

        private void Start()
        {
            SetActiveDisplayer(false);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (isDisplaying)
                {
                    SetActiveDisplayer(false);
                }
                else
                {
                    CardData cardData = null;
                    Vector3 mousePosition = Input.mousePosition;
                    mousePosition.z = -100f;

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out RaycastHit screenPointHit, float.PositiveInfinity, LayerMask.GetMask("Card")))
                    {
                        screenPointHit.transform.TryGetComponent(out PlanCard planCard);
                        cardData = planCard.CardData;
                    }
                    PointerEventData pointerData = new(EventSystem.current) { position = mousePosition};
                    List<RaycastResult> raycastResults = new();
                    EventSystem.current.RaycastAll(pointerData, raycastResults);
                    foreach (RaycastResult raycastResult in raycastResults)
                    {
                        if (raycastResult.gameObject.TryGetComponent(out DragalbeCard dragableCard))
                        {
                            cardData = dragableCard.CardData;
                            break;
                        }
                    }

                    if (cardData != null)
                    {
                        DisplayCardInfo(cardData);
                        SetActiveDisplayer(true);
                    }
                }
            }
        }

        private void DisplayCardInfo(CardData cardData)
        {
            cardSprite.sprite = cardData.CardSprite;
            if (string.IsNullOrEmpty(cardData.CardText))
                cardText.text = "효과 없음";
            else
                cardText.text = cardData.CardText;
        }

        private void SetActiveDisplayer(bool value)
        {
            cardSprite.transform.parent.gameObject.SetActive(value);
            cardText.transform.parent.gameObject.SetActive(value);
            isDisplaying = value;
        }
    }
}