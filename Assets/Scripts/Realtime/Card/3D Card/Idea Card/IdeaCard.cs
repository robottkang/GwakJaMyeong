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
        private bool CanMove = false;
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
                    cardInfo = CurrentStrategyPlan.PlacedCardInfo;
                    CanMove = true;
                }
            }
        }
        public CardInfo CardInfo
        {
            get => cardInfo;
            set => cardInfo = value;
        }

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Start()
        {
            Initialize();
        }

        public void SetCardSpriteColor(Color color)
        {
            cardSprite.color = color;
        }

        public void Initialize()
        {
            if (cardInfo != null) cardSprite.sprite = cardInfo.CardSprite;
        }

        private void OnMouseDown()
        {
            if (CanMove)
            {
                originPosition = transform.position;

                //CurrentStrategyPlan.PlacedCardInfo = null; // 드래그 시작 시 전략 구상을 비움, 나머지 구현은 언젠가
            }
        }

        private void OnMouseDrag()
        {
            if (CanMove)
            {
                Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Board"));
                transform.position = hitInfo.point + Vector3.up;
            }
        }

        private void OnMouseUp()
        {
            if (CanMove)
            {
                CardInfo resultCardInfo;
                Ray mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Idea Field")))
                {
                    StrategyPlan strategyPlan;
                    if ((strategyPlan = hitInfo.collider.GetComponentInParent<StrategyPlan>()).PlacedCardInfo == null)
                    {
                        strategyPlan.PlacedCardInfo = CardInfo;
                        resultCardInfo = null;
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
                    GameManager.gameManager.HandObject.AddCard(CardInfo);
                    resultCardInfo = null;
                }

                CurrentStrategyPlan.PlacedCardInfo = resultCardInfo;
                CanMove = false;
                ObjectPool.ReturnObject("Card Pool", gameObject);
                SetCardSpriteColor(new Color(1f, 1f, 1f, 1f));
            }
        }
    }
}
