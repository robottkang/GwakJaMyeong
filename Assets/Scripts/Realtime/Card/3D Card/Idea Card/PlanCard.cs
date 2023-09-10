using DG.Tweening;
using Room;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Card
{
    [Serializable]
    public class PlanCard : MonoBehaviour
    {
        private Camera mainCamera;
        [SerializeField]
        private SpriteRenderer cardSprite;
        [SerializeField]
        private SpriteRenderer backgroundSprite;
        [SerializeField, ReadOnly]
        private CardData cardData;
        private CardDeployment currentCardDeployment;
        public Action onCardOpen, onCardTurn, onCardSumStart, onCardSum, onCardSumEnd;
        [SerializeField, ReadOnly]
        private StrategyPlan currentStrategyPlan;
        private Vector3 originPosition;

        public bool CanMove { get; set; } = false;
        public int DamageBuff { get; set; }
        public int DamageDebuff { get; set; }
        public bool IsNotInvaild { get; set; }
        public bool Invaildated { get; set; }
        public FieldController MyFieldController { get; private set; }
        public CardDeployment CurrentCardDeployment
        {
            get => currentCardDeployment;
            set
            {
                currentCardDeployment = value;
                switch (value)
                {
                    case CardDeployment.Placed:
                        transform.SetPositionAndRotation(originPosition, Quaternion.Euler(0f, 0f, 0f));
                        break;
                    case CardDeployment.Opened:
                        transform.DOLocalMoveZ(0.5f, 0.5f);
                        transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.5f);
                        break;
                    case CardDeployment.Turned:
                        transform.DOLocalMoveZ(0.5f, 0.5f);
                        transform.DOLocalRotate(new Vector3(0f, 90f, 180f), 0.5f);
                        onCardTurn.Invoke();
                        break;
                    case CardDeployment.Disabled:
                        transform.DOLocalMoveZ(0.5f, 0.5f);
                        transform.DOLocalRotate(new Vector3(0f, 0f, 180f), 0.5f);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
        public StrategyPlan CurrentStrategyPlan
        {
            get => currentStrategyPlan;
            set
            {
                currentStrategyPlan = value;
                originPosition = transform.position;
            }
        }
        public CardData CardData => cardData;

        private void Awake()
        {
            mainCamera = Camera.main;

            PhaseEventBus.Subscribe(Phase.StrategyPlan, () => CanMove = true);
            PhaseEventBus.Subscribe(Phase.Duel, () => CanMove = false);
        }

        public void Initialize(CardData cardData, FieldController myFieldController)
        {
            this.cardData = cardData;
            DamageBuff = 0;
            DamageDebuff = 0;
            IsNotInvaild = false;
            Invaildated = false;
            currentCardDeployment = CardDeployment.Placed;
            CurrentStrategyPlan = null;
            MyFieldController = myFieldController;
            onCardOpen = () => cardData.Open(myFieldController, myFieldController.OpponentField);
            onCardTurn = () => cardData.Turn(myFieldController, myFieldController.OpponentField);
            onCardSumStart = () => cardData.SumStart(myFieldController, myFieldController.OpponentField);
            onCardSum = () => cardData.Sum(myFieldController, myFieldController.OpponentField);
            onCardSumEnd = () => cardData.SumEnd(myFieldController, myFieldController.OpponentField);
            cardSprite.sprite = DuelManager.CardSprites[(int)cardData.ThisCardCode];
            cardSprite.color = Color.white;
        }

        private void PickUp()
        {
            cardSprite.color = new(1f, 1f, 1f, 1f);
            originPosition = transform.position;
        }

        private void Move()
        {
            Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Board"));
            transform.position = hitInfo.point + Vector3.up;
        }

        private void Drop()
        {
            // if Card moves to StrategyPlan
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Plan Field")) &&
                hitInfo.transform.TryGetComponent(out StrategyPlan strategyPlan))
            {
                cardSprite.color = new(1f, 1f, 1f, 0.5f);

                // if Card doesn't move out of your StrategyPlan
                if (strategyPlan == CurrentStrategyPlan)
                {
                    transform.position = originPosition;
                    return;
                }
                // if Card moves to other StrategyPlan
                else
                {
                    CurrentStrategyPlan.TradeCard(strategyPlan);
                }
            }
            else
            {
                HandManager.Instance.AddCard(CardData);
                CurrentStrategyPlan.ClearStrategyPlan();
                cardSprite.color = new(1f, 1f, 1f, 1f);
            }
        }

        private void OnMouseDown()
        {
            if (!CanMove) return;

            PickUp();
        }

        private void OnMouseDrag()
        {
            if (!CanMove) return;

            Move();
        }

        private void OnMouseUp()
        {
            if (!CanMove) return;

            Drop();
        }
    }

    public enum CardDeployment
    {
        Placed,
        Opened,
        Turned,
        Disabled,
    }
}
