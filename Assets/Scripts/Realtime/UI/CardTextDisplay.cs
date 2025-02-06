using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Card;

namespace UI
{
    public class CardTextDisplay : MonoBehaviour, IInfoDisplay
    {
        [SerializeField]
        private Image cardSprite;
        [SerializeField]
        private TextMeshProUGUI cardText;
        private CardData displayCardData = null;

        public bool TrySetDisplayData(GameObject dataGameObejct)
        {
            CardData cardData = null;

            if (dataGameObejct.TryGetComponent(out PlanCard planCard) &&
                (planCard.CurrentDeployment == CardDeployment.Opened || planCard.OnwerFieldCtrl is Room.PlayerController))
            {
                cardData = planCard.CardData;
            }
            if (dataGameObejct.TryGetComponent(out DragalbeCard dragableCard))
            {
                cardData = dragableCard.CardData;
            }

            if (cardData != null)
            {
                displayCardData = cardData;
                return true;
            }
            return false;
        }

        public void DisplayInfo()
        {
            cardSprite.sprite = displayCardData.CardSprite;
            if (string.IsNullOrEmpty(displayCardData.CardText))
                cardText.text = "효과 없음";
            else
                cardText.text = displayCardData.CardText;
        }

        public void SetActiveDisplay(bool value)
        {
            cardSprite.transform.parent.gameObject.SetActive(value);
            cardText.transform.parent.gameObject.SetActive(value);
        }
    }
}