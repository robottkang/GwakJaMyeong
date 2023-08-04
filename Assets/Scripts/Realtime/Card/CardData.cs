using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Card.Types;
using Room;

namespace Card
{
    [CreateAssetMenu(fileName = "New CardData", menuName = "Card/CardData")]
    [Serializable]
    public class CardData : ScriptableObject
    {
        [SerializeField]
        protected CardCode cardCode;
        private readonly Dictionary<CardCode, ICardEffect> cardTypes = new()
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
        protected Posture.Posture requiredPosture = (Posture.Posture)(-1);
        [SerializeField]
        protected Posture.Posture finishingPosture;

        public CardCode ThisCardCode => cardCode;
        public Sprite CardSprite => cardSprite;
        public int Damage => damage;
        public AttackType Attack => attack;
        public AttackType GuardPoint => guardPoint;
        public Posture.Posture RequiredPosture => requiredPosture;
        public Posture.Posture FinishingPosture => finishingPosture;

        public void Open(FieldController me, FieldController target) => cardTypes[cardCode].Open(me, target);
        public void Turn(FieldController me, FieldController target) => cardTypes[cardCode].Turn(me, target);
        public void SumStart(FieldController me, FieldController target) => cardTypes[cardCode].SumStart(me, target);
        public void Sum(FieldController me, FieldController target) => cardTypes[cardCode].Sum(me, target);
        //public void SumEnd(FieldController me, FieldController target) => cardTypes[cardCode].SumEnd(me, target);

        [Flags]
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
        public void Open(FieldController me, FieldController opponent);
        public void Turn(FieldController me, FieldController opponent);
        public void SumStart(FieldController me, FieldController opponent);
        public void Sum(FieldController me, FieldController opponent);
        //public void SumEnd(FieldController me, FieldController opponent);
    }
}
