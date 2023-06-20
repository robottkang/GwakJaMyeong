using Room;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Card
{
    public class IdeaCard : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer cardSprite;
        [SerializeField]
        private SpriteRenderer backgroundSprite;
        [SerializeField]
        private CardInfo cardInfo;
        private Vector3 originPosition;
        [SerializeField, ReadOnly]
        private StrategyPlan currentStrategyPlan;

        private Camera mainCamera;

        public SpriteRenderer CardSprite => cardSprite;
        public SpriteRenderer BackgroundSprite => backgroundSprite;
        public StrategyPlan CurrentStrategyPlan
        {
            get => currentStrategyPlan;
            set
            {
                currentStrategyPlan = value;

                if (value != null)
                {
                    CardInfo = CurrentStrategyPlan.PlacedCardInfo;
                    cardSprite.color = new(1f, 1f, 1f, 0.5f);
                }
            }
        }
        public CardInfo CardInfo
        {
            get => cardInfo;
            set
            {
                cardInfo = value;
                
                if (value != null) cardSprite.sprite = cardInfo.CardSprite;
                else Debug.LogWarning("null is invaild for cardInfo");
            }
        }

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void OnDisable()
        {
            CurrentStrategyPlan = null;
            cardSprite.color = Color.white;
        }

        private void OnMouseDown()
        {
            cardSprite.color = new(1f, 1f, 1f, 1f);
            originPosition = transform.position;
        }

        private void OnMouseDrag()
        {
            Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Board"));
            transform.position = hitInfo.point + Vector3.up;
        }

        private void OnMouseUp()
        {
            Ray mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Idea Field")))
            {
                cardSprite.color = new(1f, 1f, 1f, 0.5f);
                StrategyPlan strategyPlan = hitInfo.collider.GetComponentInParent<StrategyPlan>();

                // if Card doesn't move
                if (strategyPlan == CurrentStrategyPlan)
                {
                    transform.position = originPosition;
                    return;
                }
                // if PlacedCard is null
                else if (strategyPlan.PlacedCardInfo == null)
                {
                    strategyPlan.PlacedCardInfo = CardInfo;
                    CurrentStrategyPlan.ClearStrategyPlan();
                }
                // if PlacedCard is not null, trade card
                else if (strategyPlan.PlacedCardInfo != null)
                {
                    IdeaCard targetIdeaCard = strategyPlan.PlacedCardObejct.GetComponent<IdeaCard>();
                    (CardInfo, targetIdeaCard.CardInfo) = (targetIdeaCard.CardInfo, CardInfo);
                    transform.position = originPosition;
                    return;
                }
            }
            else
            {
                GameManager.gameManager.HandController.AddCard(CardInfo);
                CurrentStrategyPlan.ClearStrategyPlan();
                cardSprite.color = new(1f, 1f, 1f, 1f);
            }
        }
    }
}
