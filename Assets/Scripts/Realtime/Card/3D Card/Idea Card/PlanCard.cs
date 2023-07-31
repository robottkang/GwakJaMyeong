using Room;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Card
{
    public class PlanCard : MonoBehaviour
    {
        private Camera mainCamera;
        [SerializeField]
        private SpriteRenderer cardSprite;
        [SerializeField]
        private SpriteRenderer backgroundSprite;
        [SerializeField, ReadOnly]
        private CardInfo cardInfo;
        private DuelData duelData;
        public Action<FieldController, FieldController> onCardOpen;
        public Action<FieldController, FieldController> onCardTurn;
        public Action<FieldController, FieldController> onCardSumStart;
        public Action<FieldController, FieldController> onCardSum;
        public Action<FieldController, FieldController> onCardSumEnd;
        [SerializeField, ReadOnly]
        private StrategyPlan currentStrategyPlan;
        private Vector3 originPosition;

        public SpriteRenderer CardSprite => cardSprite;
        public SpriteRenderer BackgroundSprite => backgroundSprite;
        public int DamageDelta
        {
            get => duelData.damageDelta;
            set
            {
                if (cardInfo.ThisCardCode == CardInfo.CardCode.Jonhau && value < 0) return;
                duelData.damageDelta = value;
            }
        }
        public int Defense { get => duelData.defense; set => duelData.defense = value; }
        public bool IsNotInvaild { get => duelData.isNotInvaild; set => duelData.isNotInvaild = value; }
        public bool Invaildated { get => duelData.invaildated; set => duelData.invaildated = value; }
        public CardDeployment CurrentCardDeployment { get => duelData.currentCardDeployment; set => duelData.currentCardDeployment = value; }
        public StrategyPlan CurrentStrategyPlan
        {
            get => currentStrategyPlan;
            set => currentStrategyPlan = value;
        }
        public CardInfo CardInfo
        {
            get => cardInfo;
            set
            {
                cardInfo = value;

                if (value == null)
                {
                    Debug.LogWarning("null is invaild for cardInfo");
                    return;
                }
                cardSprite.sprite = cardInfo.CardSprite;
                onCardOpen = cardInfo.Open;
                onCardTurn = cardInfo.Turn;
                onCardSumStart = cardInfo.SumStart;
                onCardSum = cardInfo.Sum;
                onCardSumEnd = cardInfo.SumEnd;
            }
        }

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void Initialize()
        {
            duelData = new DuelData() {
                damageDelta = 0,
                defense = 0,
                isNotInvaild = false,
                invaildated = false,
                currentCardDeployment = CardDeployment.Placed
            };
            CurrentStrategyPlan = null;
            cardSprite.color = Color.white;
        }

        #region StrategyPlan Phase
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
            Ray mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Plan Field")))
            {
                cardSprite.color = new(1f, 1f, 1f, 0.5f);
                StrategyPlan strategyPlan = hitInfo.collider.GetComponentInParent<StrategyPlan>();

                // if Card doesn't move
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
                HandManager.Instance.AddCard(CardInfo);
                CurrentStrategyPlan.ClearStrategyPlan();
                cardSprite.color = new(1f, 1f, 1f, 1f);
            }
        }
        #endregion

        #region Duel Phase

        #endregion

        private void OnMouseDown()
        {
            switch (PhaseManager.CurrentPhase)
            {
                case Phase.StrategyPlan:
                    PickUp();
                    break;
                case Phase.Duel:
                    break;
            }
        }

        private void OnMouseDrag()
        {
            switch (PhaseManager.CurrentPhase)
            {
                case Phase.StrategyPlan:
                    Move();
                    break;
                case Phase.Duel:
                    break;
            }
        }

        private void OnMouseUp()
        {
            switch (PhaseManager.CurrentPhase)
            {
                case Phase.StrategyPlan:
                    Drop();
                    break;
                case Phase.Duel:
                    break;
            }
        }

        private struct DuelData
        {
            public int damageDelta;
            public int defense;
            public bool isNotInvaild;
            public bool invaildated;
            public CardDeployment currentCardDeployment;
        }
    }

    [Flags]
    public enum CardDeployment
    {
        Placed = 0,
        Opened = 1,
        Turned = 2,
    }
}
