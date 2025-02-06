using Cysharp.Threading.Tasks;
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
        [SerializeField]
        private SpriteRenderer cardSprite;
        [SerializeField]
        private SpriteRenderer backgroundSprite;
        [SerializeField, ReadOnly]
        private CardData cardData;
        private CardDeployment currentDeployment;
        [SerializeField, ReadOnly]
        private StrategyPlan currentStrategyPlan;
        private Vector3 originPosition;
        private bool invaildated = false;
        private bool disabled = false;
        public Action onInvaildate = null;

        public bool CanMove { get; set; } = false;
        public int DamageBuff { get; set; } = 0;
        public int DamageDebuff { get; set; } = 0;
        /// <summary>
        /// 무효화 가능 여부
        /// </summary>
        public bool CanInvaildate { get; set; } = true;
        /// <summary>
        /// 무효화 여부
        /// </summary>
        public bool Invaildated
        {
            get => invaildated;
            set
            {
                if (CanInvaildate)
                    invaildated = value;
            }
        }
        /// <summary>
        /// 비활성화 여부
        /// </summary>
        public bool Disabled
        {
            get => disabled;
            set
            {
                cardSprite.color = backgroundSprite.color = Color.red;
                disabled = value;
            }
        }
        /// <summary>
        /// 꺾기 가능 여부
        /// </summary>
        public bool CanTurn { get; set; } = true;

        public FieldController OnwerFieldCtrl { get; private set; }
        public CardDeployment CurrentDeployment
        {
            get => currentDeployment;
            set
            {
                currentDeployment = value;
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
            PhaseEventBus.Subscribe(Phase.StrategyPlan, () => CanMove = true);
            PhaseEventBus.Subscribe(Phase.Duel, () => CanMove = false);
        }

        public void Initialize(CardData cardData, FieldController ownerFieldCtrl)
        {
            this.cardData = cardData;
            DamageBuff = 0;
            DamageDebuff = 0;
            CanInvaildate = true;
            Invaildated = false;
            disabled = false;
            CanTurn = true;
            currentDeployment = CardDeployment.Placed;
            CurrentStrategyPlan = null;
            OnwerFieldCtrl = ownerFieldCtrl;
            cardSprite.sprite = AssetDatabase.Instance.CardSprites[cardData.ThisCardCode];
            cardSprite.color = Color.white;
        }
        public void Initialize(CardData.CardCode cardCode, FieldController ownerFieldCtrl)
        {
            Initialize(Array.Find(AssetDatabase.Instance.CardsData, x => x.ThisCardCode == cardCode), ownerFieldCtrl);
        }

        public void Open()
        {
            CurrentDeployment = CardDeployment.Opened;

            if (Disabled || Invaildated)
            {
                cardData.Open(OnwerFieldCtrl);
            }
#if UNITY_EDITOR
            else
            {
                Debug.Log("Open is Denied");
            }
#endif
        }

        public void Turn()
        {
            CurrentDeployment = CardDeployment.Turned;
            cardData.Turn(OnwerFieldCtrl);
        }

        public void Disable()
        {
            CurrentDeployment = CardDeployment.Disabled;
            cardData.Disable(OnwerFieldCtrl);
        }

        public async UniTask SumStart()
        {
            if (Disabled || Invaildated)
            {
                cardData.SumStart(OnwerFieldCtrl);
            }
#if UNITY_EDITOR
            else
            {
                Debug.Log("Sum is Denied");
            }
#endif
        }

        public async UniTask Sum()
        {
            if (Disabled || Invaildated)
            {
                cardData.Sum(OnwerFieldCtrl);
            }
        }

        public async UniTask SumEnd()
        {
            cardData.SumEnd(OnwerFieldCtrl);
        }

        private void PickUp()
        {
            cardSprite.color = new(1f, 1f, 1f, 1f);
            originPosition = transform.position;
        }

        private void Move()
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Board"));
            transform.position = hitInfo.point + Vector3.up;
        }

        private void Drop()
        {
            // if Card moves to StrategyPlan
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Plan Field")) &&
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

    public enum CardActionType
    {
        Open,
        Turn,
        SumStart,
        Sum,
        SumEnd,
    }
}
