using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Room;
using System;

namespace Card.Types
{
    public abstract class AttackCard : ICardAction, ICardEffect
    {
        protected Action onInvaildate = null;

        public virtual void Invaildate(PlanCard targetCard)
        {
            onInvaildate?.Invoke();

            targetCard.Invaildated = true;
            if (targetCard.CurrentDeployment == CardDeployment.Turned)
            {
                targetCard.OnwerFieldCtrl.PostureCtrl.UndoPosture();
            }
        }

        /// <summary>
        /// targetField에게 대미지를 가합니다.<br/>
        /// 만약 대미지가 0보다 크다면, targetField의 현재 카드를 비활성화 시킵니다.
        /// </summary>
        public void Execute(PlanCard ownerCard, FieldController targetField)
        {
            Execute(ownerCard, targetField.CurrentCard, targetField.OpponentField, targetField);
        }
        public void Execute(PlanCard ownerCard, PlanCard targetCard, FieldController onwerField, FieldController targetField)
        {
            if (ownerCard.Invaildated || ownerCard.Disabled) return;

            int damage = CalculateDamage(ownerCard, targetCard);
            if (damage > 0)
            {
                ApplyDamage(damage, targetField);
                Disable(targetCard);
            }
            if (onwerField.CurrentCard.CardData.FinishingPosture != Posture.PostureType.None)
                onwerField.PostureCtrl.SelectPosture(onwerField.CurrentCard.CardData.FinishingPosture);
        }

        public bool IsPostureVaild(PlanCard ownerCard)
        {
            return ownerCard.OnwerFieldCtrl.PostureCtrl.CurrentPosture.HasFlag(ownerCard.CardData.RequiredPosture);
        }

        public bool CheckGuardPoint(PlanCard guardCard, PlanCard oppoentCard)
        {
            if (guardCard.CurrentDeployment == CardDeployment.Opened &&
                oppoentCard.CurrentDeployment == CardDeployment.Opened &&
                guardCard.CardData.GuardPoint.HasFlag(oppoentCard.CardData.Attack))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void ApplyDamage(int damage, FieldController targetField)
        {
            targetField.ApplyDamage(damage);
        }

        public void Disable(PlanCard targetCard)
        {
            targetCard.Disabled = true;
        }

        protected virtual int CalculateDamage(PlanCard attackCard, PlanCard guardCard)
        {
            bool isOpponentPflug = guardCard.OnwerFieldCtrl.PostureCtrl.CurrentPosture == Posture.PostureType.Pflug;
            return attackCard.CardData.Damage + attackCard.DamageBuff - attackCard.DamageDebuff - (isOpponentPflug ? 1 : 0);
        }

        public virtual void Open(FieldController owner, FieldController opponent)
        {
            if (CheckGuardPoint(opponent.CurrentCard, owner.CurrentCard))
            {
                Invaildate(owner.CurrentCard);
            }
        }

        public virtual void Turn(FieldController owner)
        {
            owner.PostureCtrl.SelectPosture(~Posture.PostureType.None);
            Disable(owner.CurrentCard);
        }

        public virtual void Disable(FieldController owner)
        {
            owner.CurrentCard.Disabled = false;
        }

        /// <summary>
        /// 합하는 카드가 동일 시(공격_옥스 제외) 서로 무효화
        /// </summary>
        public virtual void SumStart(FieldController owner, FieldController opponent)
        {
            if (owner.CurrentCard.CardData == opponent.CurrentCard.CardData &&
                owner.CurrentCard.CurrentDeployment == CardDeployment.Opened &&
                opponent.CurrentCard.CurrentDeployment == CardDeployment.Opened)
            {
                Invaildate(owner.CurrentCard);
            }
        }

        public abstract void Sum(FieldController owner, FieldController opponent);

        public virtual void SumEnd(FieldController owner, FieldController opponent)
        {
            onInvaildate = null;
        }
    }

    public abstract class ActionCard : ICardAction, ICardEffect
    {
        protected Action onInvaildate = null;

        public virtual void Invaildate(PlanCard targetCard)
        {
            onInvaildate?.Invoke();

            targetCard.Invaildated = true;
            if (targetCard.CurrentDeployment == CardDeployment.Turned)
            {
                targetCard.OnwerFieldCtrl.PostureCtrl.UndoPosture();
            }
        }

        public void Execute(PlanCard ownerCard, PlanCard targetCard, FieldController ownerField, FieldController targetField) { }

        public bool IsPostureVaild(PlanCard ownerCard)
        {
            return ownerCard.OnwerFieldCtrl.PostureCtrl.CurrentPosture.HasFlag(ownerCard.CardData.RequiredPosture);
        }

        public bool CheckGuardPoint(PlanCard guardCard, PlanCard opponentCard)
        {
            if (guardCard.CardData.GuardPoint.HasFlag(opponentCard.CardData.Attack))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Disable(PlanCard targetCard)
        {
            targetCard.Disabled = true;
        }

        public virtual void Open(FieldController owner, FieldController opponent)
        {
            if (CheckGuardPoint(opponent.CurrentCard, owner.CurrentCard))
            {
                Invaildate(owner.CurrentCard);
            }
        }

        public virtual void Turn(FieldController owner)
        {
            owner.PostureCtrl.SelectPosture(~Posture.PostureType.None);
            Disable(owner.CurrentCard);
        }

        public void Disable(FieldController owner)
        {
            owner.CurrentCard.Disabled = false;
        }

        public virtual void SumStart(FieldController owner, FieldController opponent)
        {
            if (owner.CurrentCard.CardData == opponent.CurrentCard.CardData &&
               owner.CurrentCard.CurrentDeployment == CardDeployment.Opened &&
               opponent.CurrentCard.CurrentDeployment == CardDeployment.Opened)
            {
                Invaildate(owner.CurrentCard);
            }
        }

        public abstract void Sum(FieldController owner, FieldController opponent);

        public virtual void SumEnd(FieldController owner, FieldController opponent)
        {
            onInvaildate = null;
        }
    }

    /// <summary>
    /// 무효화 될 시, 2딜 증가
    /// </summary>
    public class Duhbacel : AttackCard
    {
        private bool regsistedInvalidation = false;
        public const int extraDamage = 2;

        public override void Open(FieldController owner, FieldController opponent)
        {
            owner.CurrentCard.CanInvaildate = true;
            onInvaildate = () => RegsistInvaildation(owner.CurrentCard);

            base.Open(owner, opponent);
        }

        public override void Sum(FieldController owner, FieldController opponent)
        {
            Execute(owner.CurrentCard, opponent);
        }

        private void RegsistInvaildation(PlanCard target)
        {
            if (!regsistedInvalidation)
            {
                target.DamageBuff += extraDamage;
                regsistedInvalidation = true;
            }
        }
    }

    /// <summary>
    /// 상대 꺾기 금지, 이전 카드 복사 (나중에(언젠가(아마도 그냥 안할 것 같지만)) 작업)
    /// </summary>
    public class Dupliaren : ActionCard
    {
        public override void Open(FieldController owner, FieldController opponent)
        {
            // 꺾기 금지
            opponent.CurrentCard.CanTurn = true;

            base.Open(owner, opponent);

            // 카드 복사
            int order = DuelManager.StrategyPlanOrder;
            if (order == 0) return;

            var prevPlanCardObj = ObjectPool.GetObject("Card Pool");
            prevPlanCardObj.GetComponent<PlanCard>().Initialize(owner.StrategyPlans[order - 1].FirstPlacedPlanCard.CardData, owner);
            owner.PlaceCard(prevPlanCardObj);


        }

        public override void Sum(FieldController owner, FieldController opponent) { }
    }

    public class Rangote : AttackCard
    {
        public override void Sum(FieldController owner, FieldController opponent)
        {
            Execute(owner.CurrentCard, opponent);
        }
    }

    /// <summary>
    /// 상대 꺾기 금지,<br/>
    /// 무효화 시, 무효화 저항 및 합 이후 자세 전환
    /// </summary>
    public class Velpuren : AttackCard
    {
        private bool regsistedInvalidation = false;

        public override void Open(FieldController owner, FieldController opponent)
        {
            owner.CurrentCard.CanInvaildate = false;
            opponent.CurrentCard.CanTurn = false;

            onInvaildate = () => RegsistInvaildation();

            base.Open(owner, opponent);
        }

        public override void Sum(FieldController owner, FieldController opponent)
        {
            Execute(owner.CurrentCard, opponent);

            if (regsistedInvalidation)
            {
                owner.PostureCtrl.SelectPosture();
            }
        }

        private void RegsistInvaildation()
        {
            regsistedInvalidation = true;
        }
    }

    /// <summary>
    /// 꺾은 카드 or 행동 카드와 합하면 그 카드 무효화/<br/>
    /// 자세 전환 (상대 방도 동일하게 전환)
    /// </summary>
    public class Boudun : ActionCard
    {
        public override void Open(FieldController owner, FieldController opponent)
        {
            PlanCard opponentPlanCard = opponent.CurrentCard;
            if (opponentPlanCard.CurrentDeployment == CardDeployment.Turned ||
                opponentPlanCard.CurrentDeployment == CardDeployment.Opened && opponentPlanCard.CardData.CardType is ActionCard)
            {
                Invaildate(opponentPlanCard);
            }

            base.Open(owner, opponent);
        }

        public override void Sum(FieldController owner, FieldController opponent)
        {
            var playerPostureChanger = owner.PostureCtrl;

            playerPostureChanger.SelectPosture();
            UniTask.Void(async () =>
            {
                await UniTask.WaitUntil(() => playerPostureChanger.IsPostureChanging);
                opponent.PostureCtrl.ChangePosture(playerPostureChanger.CurrentPosture);
            });
        }
    }

    /// <summary>
    /// 올려치기 공격카드 무효화
    /// </summary>
    public class Shaitelhau : AttackCard
    {
        public override void Open(FieldController owner, FieldController opponent)
        {
            EffectShaitelhau(opponent.CurrentCard);

            base.Open(owner, opponent);
        }

        public override void Sum(FieldController owner, FieldController opponent)
        {
            Execute(owner.CurrentCard, opponent);
        }

        private void EffectShaitelhau(PlanCard opponentCard)
        {
            if (opponentCard.CardData.Attack == CardData.AttackType.UpwardCut)
            {
                Invaildate(opponentCard);
            }
        }
    }

    public class Shilhau : AttackCard
    {
        public override void Sum(FieldController owner, FieldController opponent)
        {
            Execute(owner.CurrentCard, opponent);
        }
    }

    /// <summary>
    /// 플루크 이행, 반사딜 1, 다음 횡베기 카드 2딜증 /
    /// </summary>
    public class Shulschel : ActionCard
    {
        public const int reflectingDamage = 1;
        private System.Threading.CancellationTokenSource reflectingCTS = new();

        public override void Open(FieldController owner, FieldController opponent)
        {
            owner.PostureCtrl.ChangePosture(Posture.PostureType.Pflug);

            int order = DuelManager.StrategyPlanOrder;
            PlanCard nextPlanCard;
            if (order < 3 && (nextPlanCard = owner.GetCard(order + 1))
                .CardData.Attack == CardData.AttackType.Stab)
            {
                nextPlanCard.DamageBuff += 2;
            }

            UniTask.Void(async (token) =>
            {
                int prevHp = owner.Hp;
                int prevOrder = DuelManager.StrategyPlanOrder;

                await UniTask.WaitUntil(() => owner.Hp < prevHp, cancellationToken: token);
                opponent.ApplyDamage(reflectingDamage);
            }, reflectingCTS.Token);

            base.Open(owner, opponent);
        }

        public override void Sum(FieldController owner, FieldController opponent) { }

        public override void SumEnd(FieldController owner, FieldController opponent)
        {
            reflectingCTS.Cancel();
            reflectingCTS = new();

            base.SumEnd(owner, opponent);
        }
    }

    /// <summary>
    /// 상대 공격 무효화 못시키면, 내 다음 카드 비활성화
    /// </summary>
    public class Aufschtraechen : AttackCard
    {
        public override void Sum(FieldController owner, FieldController opponent)
        {
            Execute(owner.CurrentCard, opponent);
        }

        public override void SumEnd(FieldController owner, FieldController opponent)
        {
            int order = DuelManager.StrategyPlanOrder;
            if (!opponent.CurrentCard.Invaildated && order < 3)
                Disable(owner.GetCard(order + 1));
        }
    }

    /// <summary>
    /// 댐 감소 X
    /// </summary>
    public class Jonhau : AttackCard
    {
        public override void Sum(FieldController owner, FieldController opponent)
        {
            Execute(owner.CurrentCard, opponent);
        }

        protected override int CalculateDamage(PlanCard attackCard, PlanCard guardCard)
        {
            return attackCard.CardData.Damage + attackCard.DamageBuff;
        }
    }

    /// <summary>
    /// 상대 올려, 내려베기 2딜 감소 /
    /// 폼탁 전환, 다음 카드 무효화 불가, 1딜 증가
    /// </summary>
    public class Johnhoot : ActionCard
    {
        public override void Open(FieldController owner, FieldController opponent)
        {
            PlanCard opponentPlanCard = opponent.CurrentCard;
            if (opponentPlanCard.CardData.Attack == CardData.AttackType.UpwardCut ||
                opponentPlanCard.CardData.Attack == CardData.AttackType.DownwardCut)
            {
                opponentPlanCard.DamageDebuff += 2;
            }

            base.Open(owner, opponent);
        }

        public override void Sum(FieldController owner, FieldController opponent)
        {
            owner.PostureCtrl.ChangePosture(Posture.PostureType.VomTag);

            if (DuelManager.StrategyPlanOrder < 3)
            {
                PlanCard nextPlanCard = owner.GetCard(DuelManager.StrategyPlanOrder + 1);
                nextPlanCard.CanInvaildate = true;
                nextPlanCard.DamageBuff += 1;
            }
        }
    }

    /// <summary>
    /// 상대 공격카드가 폼탁으로 시작 시, 공격카드를 무효화.
    /// </summary>
    public class Zverkhau : AttackCard
    {
        public override void Open(FieldController owner, FieldController opponent)
        {
            ZverkhauEffect(opponent);

            base.Open(owner, opponent);
        }

        public override void Sum(FieldController owner, FieldController opponent)
        {
            Execute(owner.CurrentCard, opponent);
        }

        private void ZverkhauEffect(FieldController opponent)
        {
            if (opponent.PostureCtrl.CurrentPosture == Posture.PostureType.VomTag)
            {
                Invaildate(opponent.CurrentCard);
            }
        }
    }

    /// <summary>
    /// 2 딜 이하 공격 카드 무효화 /
    /// 무효화 성공 시, 상대 다음 카드 비활성화
    /// </summary>
    public class Krumphau : ActionCard
    {
        public override void Open(FieldController owner, FieldController opponent)
        {
            PlanCard opponentPlanCard = opponent.CurrentCard;

            if (opponentPlanCard.CardData.CardType is AttackCard &&
                opponentPlanCard.CardData.Damage + opponentPlanCard.DamageBuff - opponentPlanCard.DamageDebuff < 2)
            {
                Invaildate(opponentPlanCard);
            }

            base.Open(owner, opponent);
        }

        public override void Sum(FieldController owner, FieldController opponent)
        {
            int order = DuelManager.StrategyPlanOrder;
            if (opponent.CurrentCard.Invaildated && order < 3)
            {
                Disable(opponent.GetCard(order + 1));
            }
        }
    }

    /// <summary>
    /// /
    /// 상대 공격카드 무효화 시 알버 전환, 다음 카드 먼저 공개
    /// </summary>
    public class Haeng : ActionCard
    {
        public override void Sum(FieldController owner, FieldController opponent)
        {
            if (opponent.CurrentCard.CardData.CardType is AttackCard &&
                opponent.CurrentCard.Invaildated)
            {
                opponent.PostureCtrl.ChangePosture(Posture.PostureType.Alber);

                if (owner == PlayerController.Instance)
                {
                    DuelManager.SetPlayerActionToken(UserType.Player);
                }
            }
        }
    }

    public class Ochs_Attack : AttackCard
    {
        public override void SumStart(FieldController owner, FieldController opponent)
        {
            if (CheckGuardPoint(opponent.CurrentCard, owner.CurrentCard))
            {
                Invaildate(owner.CurrentCard);
            }
        }

        public override void Sum(FieldController owner, FieldController opponent)
        {
            Execute(owner.CurrentCard, opponent);
        }

        public override void Turn(FieldController owner) { }
    }
}
