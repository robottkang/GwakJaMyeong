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
        private bool canMove = false;
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
                    canMove = true;
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

            PageEventBus.Subscribe(Page.Duel, () =>
            {
                canMove = false;
            });
        }

        private void OnDisable()
        {
            CurrentStrategyPlan = null;
            canMove = false;
        }

        public void SetCardSpriteColor(Color color)
        {
            cardSprite.color = color;
        }

        private void OnMouseDown()
        {
            if (!canMove) return;

            originPosition = transform.position;
        }

        private void OnMouseDrag()
        {
            if (!canMove) return;

            Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Board"));
            transform.position = hitInfo.point + Vector3.up;
        }

        private void OnMouseUp()
        {
            if (!canMove) return;

            Ray mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Idea Field")))
            {
                StrategyPlan strategyPlan;
                if ((strategyPlan = hitInfo.collider.GetComponentInParent<StrategyPlan>()).PlacedCardInfo == null)
                {
                    strategyPlan.PlacedCardInfo = CardInfo;
                    CurrentStrategyPlan.ClearStrategyPlan();
                }
                else if (strategyPlan == CurrentStrategyPlan)
                {
                    transform.position = originPosition;
                    return;
                }
                else // if PlacedCard is not null, trade card <- implement later
                {
                    //resultCardInfo = strategyPlan.PlacedCardInfo;
                    //strategyPlan.PlacedCardInfo = CardInfo;
                    transform.position = originPosition;
                    return;
                }
            }
            else
            {
                GameManager.gameManager.HandController.AddCard(CardInfo);
                CurrentStrategyPlan.ClearStrategyPlan();
                SetCardSpriteColor(new Color(1f, 1f, 1f, 1f));
            }
        }
    }
}
