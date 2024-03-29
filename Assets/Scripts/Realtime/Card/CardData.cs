using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Card.Types;
using Room;
using Card.Posture;

namespace Card
{
    [CreateAssetMenu(fileName = "New CardData", menuName = "Card/CardData")]
    [Serializable]
    public class CardData : ScriptableObject
    {
        [SerializeField]
        protected CardCode cardCode;
        private readonly Dictionary<CardCode, ICardAction> cardTypes = new()
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
        public Sprite CardSprite => cardSprite;
        public int Damage => damage;
        public AttackType Attack => attack;
        public AttackType GuardPoint => guardPoint;
        public PostureType RequiredPosture => requiredPosture;
        public PostureType FinishingPosture => finishingPosture;
        public string CardText => cardText;

        public void Open(FieldController me, FieldController target) => cardTypes[cardCode].Open(me, target);
        public void Turn(FieldController me, FieldController target) => cardTypes[cardCode].Turn(me, target);
        public void SumStart(FieldController me, FieldController target) => cardTypes[cardCode].SumStart(me, target);
        public void Sum(FieldController me, FieldController target) => cardTypes[cardCode].Sum(me, target);
        public void SumEnd(FieldController me, FieldController target) => cardTypes[cardCode].SumEnd(me, target);

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
        public void Execute(PlanCard myCard, PlanCard opponentCard, FieldController myField, FieldController targetField);
        public bool IsPostureVaild(PlanCard myCard);
        /// <summary>
        /// 상대 카드의 가드 포인트에 해당하는 유형의 카드와 합하고 있을 경우, targetCard를 무효화 시킵니다.
        /// </summary>
        public bool CheckGarudPoint(PlanCard opponentCard, PlanCard targetCard);
    }

    public interface ICardAction
    {
        public void Open(FieldController me, FieldController opponent);
        public void Turn(FieldController me, FieldController opponent);
        public void SumStart(FieldController me, FieldController opponent);
        public void Sum(FieldController me, FieldController opponent);
        public void SumEnd(FieldController me, FieldController opponent);
    }
}
