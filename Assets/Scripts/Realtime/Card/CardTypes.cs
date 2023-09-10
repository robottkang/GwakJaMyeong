using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Room;

namespace Card.Types
{
    public abstract class CombatTechnique : ICardEffect
    {
        /// <summary>
        /// targetCard가 무효화 불가가 아니라면, 그 카드를 무효화 시키고,<br/>
        /// 카드이 꺾인 상태이라면 targetCard의 자세 변환을 취소시킨다.
        /// </summary>
        protected virtual void Invaildate(PlanCard targetCard)
        {
            if (!targetCard.IsNotInvaild || targetCard.CurrentCardDeployment != CardDeployment.Disabled)
            {
                targetCard.Invaildated = true;
                if (targetCard.CurrentCardDeployment == CardDeployment.Turned)
                {
                    targetCard.MyFieldController.PostureController.UndoPosture();
                }
            }
        }

        /// <summary>
        /// targetField에게 대미지를 가합니다.<br/>
        /// 만약 대미지가 0보다 크다면, targetField의 현재 카드를 비활성화 시킵니다.
        /// </summary>
        protected void Attack(PlanCard myCard, FieldController targetField)
        {
            Attack(myCard, targetField.CurrentCard, targetField);
        }
        protected void Attack(PlanCard myCard, PlanCard opponentCard, FieldController targetField)
        {
            Attack(myCard, opponentCard, targetField.OpponentField, targetField);
        }
        protected void Attack(PlanCard myCard, PlanCard opponentCard, FieldController myField, FieldController targetField)
        {
            if (myCard.Invaildated) return;

            int damage = CalculateDamage(myCard, opponentCard);
            if (damage > 0)
            {
                TakeDamage(damage, targetField);
                Disable(opponentCard);
            }
            if (myField.CurrentCard.CardData.FinishingPosture != Posture.Posture.None)
                myField.PostureController.SelectPosture(myField.CurrentCard.CardData.FinishingPosture);
        }

        public bool CheckRequiredPosture(PlanCard myCard)
        {
            return myCard.MyFieldController.PostureController.CurrentPosture.HasFlag(myCard.CardData.RequiredPosture);
        }

        /// <summary>
        /// 상대 카드의 가드 포인트에 해당하는 유형의 카드와 합하고 있을 경우, targetCard를 무효화 시킵니다.
        /// </summary>
        protected void CheckGarudPoint(PlanCard opponentCard, PlanCard targetCard)
        {
            if (opponentCard.CardData.GuardPoint.HasFlag(targetCard.CardData.Attack))
            {
                Invaildate(targetCard);
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
            bool isOpponentPflug = opponentCard.MyFieldController.PostureController.CurrentPosture == Posture.Posture.Pflug;
            return myCard.CardData.Damage + myCard.DamageBuff - myCard.DamageDebuff - (isOpponentPflug ? 1 : 0);
        }

        public virtual void Open(FieldController me, FieldController opponent) { }

        public virtual void Turn(FieldController me, FieldController opponent)
        {
            me.PostureController.SelectPosture(~Posture.Posture.None);
            Disable(me.CurrentCard);
        }

        public virtual void SumStart(FieldController me, FieldController opponent)
        {
            if (me.CurrentCard.CardData == opponent.CurrentCard.CardData &&
                me.CurrentCard.CurrentCardDeployment == CardDeployment.Opened &&
                opponent.CurrentCard.CurrentCardDeployment == CardDeployment.Opened)
            {
                Invaildate(me.CurrentCard);
            }
        }

        public abstract void Sum(FieldController me, FieldController opponent);

        public virtual void SumEnd(FieldController me, FieldController opponent) { }
    }

    /// <summary>
    /// 무효화 될 시, 2딜 증가
    /// </summary>
    public class Duhbacel : CombatTechnique
    {
        public const int extraDamage = 2;

        public override void Open(FieldController me, FieldController opponent)
        {
            RegistInvaild(me.CurrentCard);
        }

        public override void Sum(FieldController me, FieldController opponent)
        {
            RegistInvaild(me.CurrentCard);
            Attack(me.CurrentCard, opponent);
        }

        private void RegistInvaild(PlanCard myPlanCard)
        {
            if (myPlanCard.Invaildated)
            {
                myPlanCard.IsNotInvaild = true;
                myPlanCard.Invaildated = false;
                myPlanCard.DamageBuff += extraDamage;
            }
        }
    }

    /// <summary>
    /// 이전 카드 복사
    /// </summary>
    public class Dupliaren : CombatTechnique
    {
        public override void Open(FieldController me, FieldController opponent)
        {
            int order = DuelManager.StrategyPlanOrder;
            if (order == 0) return;

            var prevPlanCardObj = ObjectPool.GetObject("Card Pool");
            prevPlanCardObj.GetComponent<PlanCard>().Initialize(me.StrategyPlans[order - 1].FirstPlacedPlanCard.CardData, me);
            me.PlaceCard(prevPlanCardObj);

            prevPlanCardObj.GetComponent<PlanCard>().onCardOpen.Invoke();
        }

        public override void Sum(FieldController me, FieldController opponent) { }
    }

    public class Rangote : CombatTechnique
    {
        public override void Sum(FieldController me, FieldController opponent)
        {
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
            RegistInvaild(me.CurrentCard);
            opponent.CurrentCard.onCardTurn = null;
        }

        public override void Sum(FieldController me, FieldController opponent)
        {
            PlanCard myPlanCard = me.CurrentCard;

            RegistInvaild(myPlanCard);
            Attack(myPlanCard, opponent);

            if (myPlanCard.IsNotInvaild)
            {
                me.PostureController.SelectPosture();
            }
        }

        private void RegistInvaild(PlanCard myPlanCard)
        {
            if (myPlanCard.Invaildated)
            {
                myPlanCard.IsNotInvaild = true;
                myPlanCard.Invaildated = false;
            }
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
            PlanCard opponentPlanCard = opponent.CurrentCard;
            if (opponentPlanCard.CurrentCardDeployment == CardDeployment.Turned ||
                opponentPlanCard.CurrentCardDeployment == CardDeployment.Opened && opponentPlanCard.CardData.Damage == 0)
            {
                Invaildate(opponentPlanCard);
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
            EffectShaitelhau(opponent.CurrentCard);
        }

        public override void Sum(FieldController me, FieldController opponent)
        {
            EffectShaitelhau(opponent.CurrentCard);

            Attack(me.CurrentCard, opponent);
        }

        private void EffectShaitelhau(PlanCard opponentCard)
        {
            if (opponentCard.CurrentCardDeployment == CardDeployment.Opened && 
                opponentCard.CardData.Attack == CardData.AttackType.UpwardCut)
            {
                Invaildate(opponentCard);
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
            me.PostureController.ChangePosture(Posture.Posture.Pflug);

            int order = DuelManager.StrategyPlanOrder;
            PlanCard nextPlanCard;
            if (order < 3 && (nextPlanCard = me.GetCard(order + 1))
                .CardData.Attack == CardData.AttackType.Stab)
            {
                nextPlanCard.DamageBuff += 2;
            }

            UniTask.Void(async () =>
            {
                int prevHp = me.Hp;
                int prevOrder = DuelManager.StrategyPlanOrder;
                await UniTask.WaitUntil(() => me.Hp != prevHp || DuelManager.StrategyPlanOrder != prevOrder);
                if (me.Hp < prevHp)
                    TakeDamage(1, opponent);
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
            Attack(me.CurrentCard, opponent);
        }

        public override void SumEnd(FieldController me, FieldController opponent)
        {
            if (opponent.CurrentCard.Invaildated)
                Disable(me.GetCard(DuelManager.StrategyPlanOrder + 1));
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
            return me.CardData.Damage + me.DamageBuff;
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
            PlanCard opponentPlanCard = opponent.CurrentCard;
            if (opponentPlanCard.CardData.Attack == CardData.AttackType.UpwardCut ||
                opponentPlanCard.CardData.Attack == CardData.AttackType.DownwardCut)
            {
                opponentPlanCard.DamageDebuff -= 2;
            }
        }

        public override void Sum(FieldController me, FieldController opponent)
        {
            me.PostureController.ChangePosture(Posture.Posture.VomTag);

            if (DuelManager.StrategyPlanOrder < 3)
            {
                PlanCard nextPlanCard = me.GetCard(DuelManager.StrategyPlanOrder + 1);
                nextPlanCard.IsNotInvaild = true;
                nextPlanCard.DamageBuff += 1;
            }
        }
    }

    /// <summary>
    /// 상대 공격카드가 폼탁으로 시작 시, 공격카드를 무효화.
    /// </summary>
    public class Zverkhau : CombatTechnique
    {
        public override void Open(FieldController me, FieldController opponent)
        {
            ZverkhauEffect(opponent);
        }

        public override void Sum(FieldController me, FieldController opponent)
        {
            ZverkhauEffect(opponent);
            Attack(me.CurrentCard, opponent);
        }

        private void ZverkhauEffect(FieldController opponentField)
        {
            if (opponentField.PostureController.CurrentPosture == Posture.Posture.VomTag)
            {
                Invaildate(opponentField.CurrentCard);
            }
        }
    }

    /// <summary>
    /// 2 딜 이하 공격 무효화 /
    /// 공격 무효화 성공 시, 상대 다음 카드 비활성화
    /// </summary>
    public class Krumphau : CombatTechnique
    {
        public override void Open(FieldController me, FieldController opponent)
        {
            PlanCard opponentPlanCard = opponent.CurrentCard;

            if (opponentPlanCard.CardData.Damage != 0 &&
                opponentPlanCard.CardData.Damage + opponentPlanCard.DamageBuff - opponentPlanCard.DamageDebuff < 2)
            {
                Invaildate(opponentPlanCard);
            }
        }

        public override void Sum(FieldController me, FieldController opponent)
        {
            if (opponent.CurrentCard.Invaildated)
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

                if (me == PlayerController.Instance)
                {
                    DuelManager.SetActionToken(true);
                }
            }
        }
    }

    public class Ochs_Attack : CombatTechnique
    {
        public override void Sum(FieldController me, FieldController opponent)
        {
            Attack(me.CurrentCard, opponent);
        }

        public override void Turn(FieldController me, FieldController opponent) { }
    }
}
