using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Card.Types;
using Room;
using Card.Posture;
using static Card.CardData;

namespace Card
{
    [CreateAssetMenu(fileName = "New CardData", menuName = "Card/CardData")]
    public class CardData : ScriptableObject
    {
        private static readonly Dictionary<CardCode, ICardAction> cardTypes = new()
        {
            { CardCode.Aufschtraechen, new Aufschtraechen() },
            { CardCode.Boudun, new Boudun() },
            { CardCode.Duhbacel, new Duhbacel() },
            { CardCode.Dupliaren, new Dupliaren() },
            { CardCode.Haeng, new Haeng() },
            { CardCode.Johnhoot, new Johnhoot() },
            { CardCode.Jonhau, new Jonhau() },
            { CardCode.Krumphau, new Krumphau() },
            { CardCode.Rangote, new Rangote() },
            { CardCode.Shaitelhau, new Shaitelhau() },
            { CardCode.Shilhau, new Shilhau() },
            { CardCode.Shulschel, new Shulschel() },
            { CardCode.Velpuren, new Velpuren() },
            { CardCode.Zverkhau, new Zverkhau() },
            { CardCode.Ochs_Attack, new Ochs_Attack() }
        };
        [SerializeField]
        protected CardCode cardCode;
        [SerializeField]
        protected Sprite cardSprite;

        [SerializeField]
        protected AttackType attack;
        [SerializeField]
        protected AttackType guardPoint;
        [SerializeField]
        protected int damage;
        [SerializeField]
        protected PostureType requiredPosture = (PostureType)(-1);
        [SerializeField]
        protected PostureType finishingPosture;
        [SerializeField, TextArea]
        private string cardText;

        public CardCode ThisCardCode => cardCode;
        public ICardAction CardType => cardTypes[ThisCardCode];
        public Sprite CardSprite
        {
            get
            {
                if (cardSprite != null)
                {
                    return cardSprite;
                }
                else
                {
                    return AssetDatabase.Instance.CardSprites[ThisCardCode];
                }
            }
        }
        public int Damage => damage;
        public AttackType Attack => attack;
        public AttackType GuardPoint => guardPoint;
        public PostureType RequiredPosture => requiredPosture;
        public PostureType FinishingPosture => finishingPosture;
        public string CardText => cardText;

        public void Open(FieldController target) => cardTypes[cardCode].Open(target, target.OpponentField);
        public void Turn(FieldController target) => cardTypes[cardCode].Turn(target);
        public void Disable(FieldController target) => cardTypes[cardCode].Disable(target);
        public void SumStart(FieldController target) => cardTypes[cardCode].SumStart(target, target.OpponentField);
        public void Sum(FieldController target) => cardTypes[cardCode].Sum(target, target.OpponentField);
        public void SumEnd(FieldController target) => cardTypes[cardCode].SumEnd(target, target.OpponentField);

        [Flags, Serializable]
        public enum AttackType
        {
            None = 0,
            /// <summary>
            /// 내려베기
            /// </summary>
            DownwardCut = 1,
            /// <summary>
            /// 올려베기
            /// </summary>
            UpwardCut = 2,
            /// <summary>
            /// 찌르기
            /// </summary>
            Stab = 4,
            /// <summary>
            /// 횡베기
            /// </summary>
            HorizontalCut = 8,
        }

        [Serializable]
        public enum CardCode
        {
            Duhbacel,
            Dupliaren,
            Rangote,
            Velpuren,
            Boudun,
            Shaitelhau,
            Shilhau,
            Shulschel,
            Aufschtraechen,
            Jonhau,
            Johnhoot,
            Zverkhau,
            Krumphau,
            Haeng,
            Ochs_Attack,
        }
    }

    public interface ICardEffect
    {
        /// <summary>
        /// targetCard가 무효화 불가가 아니라면, 그 카드를 무효화 시키고,<br/>
        /// 카드이 꺾인 상태이라면 targetCard의 자세 변환을 취소시킨다.
        /// </summary>
        public void Invaildate(PlanCard targetCard);
        /// <summary>
        /// targetCard를 비활성화 시킨다.
        /// </summary>
        public void Disable(PlanCard targetCard);
        public void Execute(PlanCard ownerCard, PlanCard targetCard, FieldController ownerField, FieldController targetField);
        /// <summary>
        /// targetCard의 발동 자세가 유효한지 확인한다.
        /// </summary>
        public bool IsPostureVaild(PlanCard targetCard);
        /// <summary>
        /// 상대 카드의 가드 포인트에 해당하는 유형의 카드와 합하고 있을 경우, opponentCard를 무효화 시킵니다.
        /// </summary>
        public bool CheckGuardPoint(PlanCard guardCard, PlanCard opponentCard);
    }

    public interface ICardAction
    {
        public void Open(FieldController owner, FieldController opponet);
        public void Turn(FieldController owner);
        public void Disable(FieldController owner);
        public void SumStart(FieldController owner, FieldController opponet);
        public void Sum(FieldController owner, FieldController opponet);
        public void SumEnd(FieldController owner, FieldController opponet);
    }
}
