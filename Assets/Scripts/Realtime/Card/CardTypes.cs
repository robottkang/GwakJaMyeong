using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Card;
using Room;

namespace Card.Types
{
    public abstract class CombatTechnique : ICardEffect
    {
        /// <summary>
        /// targetField이 현재 카드가 무효화 불가가 아니라면, 그 카드를 무효화 시키고,<br/>
        /// 카드이 꺾인 상태이라면 targetField의 자세 변환을 취소시킨다.
        /// </summary>
        protected virtual void Invaildate(FieldController targetField)
        {
            PlanCard planCard = targetField.CurrentCard;
            if (!planCard.IsNotInvaild)
            {
                planCard.Invaildated = true;
                if (planCard.CurrentCardDeployment == CardDeployment.Turned)
                {
                    targetField.PostureController.UndoPosture();
                }
            }
        }

        /// <summary>
        /// targetField에게 대미지를 가합니다.<br/>
        /// 만약 대미지가 0보다 크다면, targetField의 현재 카드를 무효화 시킵니다.
        /// </summary>
        protected void Attack(int damage, FieldController targetField)
        {
            Attack(damage, targetField.CurrentCard, targetField);
        }
        protected void Attack(int damage, PlanCard opponentCard, FieldController target)
        {
            damage -= opponentCard.Defense;
            if (damage > 0)
            {
                TakeDamage(damage, target);
                Disable(opponentCard);
            }
        }
        protected void Attack(PlanCard myCard, FieldController targetField)
        {
            Attack(myCard, targetField.CurrentCard, targetField);
        }
        protected void Attack(PlanCard myCard, PlanCard opponentCard, FieldController targetField)
        {
            int damage = CalculateDamage(myCard, opponentCard);
            if (damage > 0)
            {
                TakeDamage(damage, targetField);
                Disable(opponentCard);
            }
        }

        public bool CheckRequiredPosture(FieldController myCard)
        {
            return myCard.PostureController.CurrentPosture.HasFlag(myCard.CurrentCard.CardData.RequiredPosture);
        }

        /// <summary>
        /// 적의 가드 포인트에 해당하는 유형의 카드와 합하고 있을 경우, me를 무효화 시킵니다.
        /// </summary>
        protected void CheckGarudPoint(PlanCard myCard, FieldController opponentField)
        {
            CheckGarudPoint(myCard, opponentField.CurrentCard, opponentField);
        }

        protected void CheckGarudPoint(PlanCard myCard, PlanCard targetCard, FieldController opponentField)
        {
            if (targetCard.CardData.GuardPoint.HasFlag(myCard.CardData.Attack))
            {
                Invaildate(opponentField);
            }
        }

        protected void TakeDamage(int damage, FieldController targetField)
        {
            targetField.TakeDamage(damage);
        }

        protected void Disable(PlanCard targetCard)
        {
            targetCard.onCardOpen = null;
            targetCard.onCardTurn = null;
            targetCard.onCardSumStart = null;
            targetCard.onCardSum = null;
            targetCard.onCardSumEnd = null;
        }

        protected virtual int CalculateDamage(PlanCard myCard, PlanCard opponentCard)
        {
            return myCard.CardData.Damage + myCard.DamageDelta - opponentCard.Defense;
        }

        public virtual void Open(FieldController me, FieldController opponent)
        {

        }

        public void Turn(FieldController me, FieldController opponent)
        {
            me.PostureController.SelectPosture(~Posture.Posture.None);
        }

        public virtual void SumStart(FieldController me, FieldController opponent)
        {
            if (me.CurrentCard.CardData == opponent.CurrentCard.CardData &&
                me.CurrentCard.CurrentCardDeployment == CardDeployment.Opened &&
                opponent.CurrentCard.CurrentCardDeployment == CardDeployment.Opened)
            {
                Invaildate(me);
            }
        }

        public abstract void Sum(FieldController me, FieldController opponent);

        public virtual void SumEnd(FieldController me, FieldController opponent)
        {
            if (me.CurrentCard.CardData.FinishingPosture != Posture.Posture.None)
                me.PostureController.SelectPosture(me.CurrentCard.CardData.FinishingPosture);
        }
    }

    /// <summary>
    /// 무효화 될 시, 2딜 증가
    /// </summary>
    public class Duhbacel : CombatTechnique
    {
        public const int extraDamage = 2;

        public override void Sum(FieldController me, FieldController opponent)
        {
            PlanCard myPlanCard = me.CurrentCard;
            int endDmage = CalculateDamage(myPlanCard, opponent.CurrentCard);
            if (myPlanCard.Invaildated)
            {
                Attack(endDmage + extraDamage, opponent);
            }
            else
            {
                Attack(endDmage, opponent);
            }
            myPlanCard.Invaildated = false;
            myPlanCard.IsNotInvaild = true;
        }
    }

    /// <summary>
    /// 이전 카드 복사 (미구현)
    /// </summary>
    public class Dupliaren : CombatTechnique
    {
        public override void Open(FieldController me, FieldController opponent)
        {
            base.Open(me, opponent);

            int order = DuelManager.StrategyPlanOrder;
            if (order == 0) return;
            me.PlaceCard(ObjectPool.GetObject("Card Pool" , me.GetCard(order - 1).gameObject));
        }

        public override void Sum(FieldController me, FieldController opponent) { }
    }

    public class Rangote : CombatTechnique
    {
        public override void Sum(FieldController me, FieldController opponent)
        {
            //if (me.CurrentCard.Invaildated) return;

            Attack(me.CurrentCard, opponent);
        }
    }

    /// <summary>
    /// 상대 꺾기 금지,<br/>
    /// 무효화 시, 딜 이후 자세 전환
    /// </summary>
    public class Velpuren : CombatTechnique
    {
        public override void Open(FieldController me, FieldController opponent)
        {
            base.Open(me, opponent);

            opponent.CurrentCard.onCardTurn = null;
        }

        public override void Sum(FieldController me, FieldController opponent)
        {
            PlanCard myPlanCard = me.CurrentCard;
            Attack(myPlanCard, opponent);

            if (myPlanCard.Invaildated)
            {
                me.PostureController.SelectPosture();
            }
            myPlanCard.Invaildated = false;
            myPlanCard.IsNotInvaild = true;
        }
    }

    /// <summary>
    /// 꺾은 카드 or 행동 카드와 합하면 그 카드 무효화/<br/>
    /// 자세 전환 (상대 방도 동일하게 전환)
    /// </summary>
    public class Boudun : CombatTechnique
    {
        public override void Open(FieldController me, FieldController opponent)
        {
            base.Open(me, opponent);

            PlanCard opponentCard = opponent.CurrentCard;
            if (opponentCard.CurrentCardDeployment == CardDeployment.Turned ||
                opponentCard.CurrentCardDeployment == CardDeployment.Opened && opponentCard.CardData.Damage == 0)
            {
                Invaildate(opponent);
            }
        }

        public override void Sum(FieldController me, FieldController opponent)
        {
            var myPostureChanger = me.PostureController;
            myPostureChanger.SelectPosture();
            UniTask.Void(async () =>
            {
                await UniTask.WaitUntil(() => myPostureChanger.IsPostureChanging);
                opponent.PostureController.ChangePosture(myPostureChanger.CurrentPosture);
            });
        }
    }

    /// <summary>
    /// 올려치기 공격카드 무효화
    /// </summary>
    public class Shaitelhau : CombatTechnique
    {
        public override void Open(FieldController me, FieldController opponent)
        {
            base.Open(me, opponent);

            EffectShaitelhau(opponent);
        }

        public override void Sum(FieldController me, FieldController opponent)
        {
            EffectShaitelhau(opponent);

            Attack(me.CurrentCard, opponent);
        }

        private void EffectShaitelhau(FieldController opponent)
        {
            if (opponent.CurrentCard.CurrentCardDeployment == CardDeployment.Opened && 
                opponent.CurrentCard.CardData.Attack.HasFlag(CardData.AttackType.UpwardCut))
            {
                Invaildate(opponent);
            }
        }
    }

    public class Shilhau : CombatTechnique
    {
        public override void Sum(FieldController me, FieldController opponent)
        {
            Attack(me.CurrentCard, opponent);
        }
    }

    /// <summary>
    /// 플루크 이행, 반사딜 1, 다음 횡베기 카드 2딜증 /
    /// </summary>
    public class Shulschel : CombatTechnique
    {
        public override void Open(FieldController me, FieldController opponent)
        {
            base.Open(me, opponent);

            me.PostureController.ChangePosture(Posture.Posture.Pflug);

            int order = DuelManager.StrategyPlanOrder;
            PlanCard nextPlanCard;
            if (order < 3 && (nextPlanCard = me.GetCard(order + 1))
                .CardData.Attack.HasFlag(CardData.AttackType.Stab))
            {
                nextPlanCard.DamageDelta += 2;
            }

            UniTask.Void(async () =>
            {
                int prevHp = me.Hp;
                int prevOrder = DuelManager.StrategyPlanOrder;
                await UniTask.WaitUntil(() => me.Hp != prevHp || DuelManager.StrategyPlanOrder != prevOrder);
                if (me.Hp < prevHp)
                    Attack(1, opponent);
            });
        }

        public override void Sum(FieldController me, FieldController opponent) { }
    }

    /// <summary>
    /// 상대 공격 무효화 못시키면, 내 다음 카드 비활성화
    /// </summary>
    public class Aufschtraechen : CombatTechnique
    {
        public override void Sum(FieldController me, FieldController opponent)
        {
            if (!opponent.CurrentCard.Invaildated)
                Disable(me.GetCard(DuelManager.StrategyPlanOrder + 1));

            Attack(me.CurrentCard, opponent);
        }
    }

    /// <summary>
    /// 댐 감소 X
    /// </summary>
    public class Jonhau : CombatTechnique
    {
        public override void Sum(FieldController me, FieldController opponent)
        {
            Attack(me.CurrentCard, opponent);
        }

        protected override int CalculateDamage(PlanCard me, PlanCard opponent)
        {
            return me.CardData.Damage + me.DamageDelta;
        }
    }

    /// <summary>
    /// 상대 올려, 내려베기 2딜 감소 /
    /// 폼탁 전환, 다음 카드 무효화 불가, 1딜 증가
    /// </summary>
    public class Johnhoot : CombatTechnique
    {
        public override void Open(FieldController me, FieldController opponent)
        {
            base.Open(me, opponent);

            PlanCard opponentPlanCard = opponent.CurrentCard;
            if (opponentPlanCard.CardData.Attack.HasFlag(CardData.AttackType.UpwardCut) ||
                opponentPlanCard.CardData.Attack.HasFlag(CardData.AttackType.DownwardCut))
            {
                opponentPlanCard.DamageDelta -= 2;
            }
        }

        public override void Sum(FieldController me, FieldController opponent)
        {
            me.PostureController.ChangePosture(Posture.Posture.VomTag);

            if (DuelManager.StrategyPlanOrder < 3)
            {
                PlanCard nextPlanCard = me.GetCard(DuelManager.StrategyPlanOrder + 1);
                nextPlanCard.Invaildated = false;
                nextPlanCard.DamageDelta += 1;
            }
        }
    }

    public class Zverkhau : CombatTechnique
    {
        public override void Sum(FieldController me, FieldController opponent)
        {
            Attack(me.CurrentCard, opponent);
        }
    }

    /// <summary>
    /// 2 딜 이하 공격 무효화 /
    /// 공격 무효화 성공 시, 상대 다음 카드 비활성화
    /// </summary>
    public class Krumphau : CombatTechnique
    {
        private bool invaildatedAttack = false;

        public override void Open(FieldController me, FieldController opponent)
        {
            base.Open(me, opponent);

            PlanCard opponentPlanCard = opponent.CurrentCard;
            if (opponentPlanCard.CardData.Damage == 0) return;
            if (opponentPlanCard.CardData.Damage + opponentPlanCard.DamageDelta < 2)
            {
                opponentPlanCard.DamageDelta = -527;
                invaildatedAttack = true;
            }
        }

        public override void Sum(FieldController me, FieldController opponent)
        {
            if (invaildatedAttack)
            {
                Disable(opponent.GetCard(DuelManager.StrategyPlanOrder + 1));
            }
        }
    }

    /// <summary>
    /// /
    /// 상대 공격 무효화 시 알버 전환, 다음 카드 먼저 공개
    /// </summary>
    public class Haeng : CombatTechnique
    {
        public override void Sum(FieldController me, FieldController opponent)
        {
            if (opponent.CurrentCard.Invaildated)
            {
                opponent.PostureController.ChangePosture(Posture.Posture.Alber);
            }
        }
    }

    public class Ochs_Attack : CombatTechnique
    {
        public override void Sum(FieldController me, FieldController opponent)
        {
            Attack(me.CurrentCard, opponent);
        }
    }
}
