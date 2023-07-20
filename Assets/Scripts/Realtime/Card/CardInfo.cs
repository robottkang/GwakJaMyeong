using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Card
{
    [CreateAssetMenu(fileName = "New CardInfo", menuName = "Card/CardInfo")]
    public class CardInfo : ScriptableObject
    {
        [SerializeField]
        protected Sprite cardSprite;

        [SerializeField]
        protected AttackType attack;
        [SerializeField]
        protected AttackType guardPoint;
        [SerializeField]
        protected int damage;
        [SerializeField]
        protected Posture requiredPosture;
        [SerializeField]
        protected Posture finishingPosture;
        protected bool canBeInvaild;

        public string CardName => cardSprite.name;
        public Sprite CardSprite => cardSprite;
        public int Damage => damage;
        public AttackType Attack => attack;
        public AttackType GuardPoint => guardPoint;
        public bool CanBeInvaild => canBeInvaild;

        public UnityEvent onCardOpen = new();
        public UnityEvent onCardTurn = new();
        public UnityEvent onCardSum = new();
        public UnityEvent onCardEffect = new();

        [Flags]
        public enum AttackType
        {
            none = 0,
            /// <summary>
            /// 내려베기
            /// </summary>
            uberhauw = 1,
            /// <summary>
            /// 올려베기
            /// </summary>
            unterhau = 2,
            /// <summary>
            /// 횡베기
            /// </summary>
            horizontal = 4,
            /// <summary>
            /// 찌르기
            /// </summary>
            stechen = 8,
        }

        [Flags]
        public enum Posture
        {
            None = 0,
            /// <summary>
            /// 세로
            /// </summary>
            VomTag = 1,
            /// <summary>
            /// 대각선
            /// </summary>
            Pflug = 2,
            /// <summary>
            /// 가로
            /// </summary>
            Ochs = 4,
            /// <summary>
            /// 뒤
            /// </summary>
            Alber = 8,
        }
    }
}
