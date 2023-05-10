using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Card
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
    public class CardInfo : ScriptableObject
    {
        [SerializeField]
        protected Sprite cardSprite;
        [SerializeField]
        protected int damage;
        [SerializeField]
        protected AttackType attack;
        [SerializeField]
        protected AttackType guardPoint;
        [SerializeField]
        protected Posture useConditions;
        [SerializeField]
        protected Posture postAttackPosture;

        public string CardName => cardSprite.name;
        public Sprite CardSprite => cardSprite;
        public int Damage => damage;
        public AttackType Attack => attack;
        public AttackType GuardPoint => guardPoint;
        public Posture UseConditions => useConditions;
        public Posture PostAttackPosture => postAttackPosture;

        public enum AttackType
        {
            none,
            cuttingDown,
            cuttingUp,
            cuttingSideways,
            poking
        }
    }
}
