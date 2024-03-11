using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Room;

namespace Card.Types
{
    public abstract class AttackCard : ICardAction, ICardEffect
    {
        public virtual void Invaildate(PlanCard targetCard)
        {
            if (!targetCard.IsNotInvaild || targetCard.CurrentCardDeployment != CardDeployment.Disabled)
            {
                targetCard.Invaildated = true;
                if (targetCard.CurrentCardDeployment == CardDeployment.Turned)
                {
                    targetCard.PlayerFieldCtrl.PostureCtrl.UndoPosture();
                }
            }
        }

        /// <summary>
        /// targetField에게 대미지를 가합니다.<br/>
        /// 만약 대미지가 0보다 크다면, targetField의 현재 카드를 비활성화 시킵니다.
        /// </summary>
        public void Execute(PlanCard playerCard, FieldController targetField)
        {
            Execute(playerCard, targetField.CurrentCard, targetField.OpponentField, targetField);
        }
        public void Execute(PlanCard playerCard, PlanCard opponentCard, FieldController playerField, FieldController targetField)
        {
            if (playerCard.Invaildated) return;

            int damage = CalculateDamage(playerCard, opponentCard);
            if (damage > 0)
            {
                TakeDamage(damage, targetField);
                Disable(opponentCard);
            }
            if (playerField.CurrentCard.CardData.FinishingPosture != Posture.PostureType.None)
                playerField.PostureCtrl.SelectPosture(playerField.CurrentCard.CardData.FinishingPosture);
        }

        public bool IsPostureVaild(PlanCard playerCard)
        {
            return playerCard.PlayerFieldCtrl.PostureCtrl.CurrentPosture.HasFlag(playerCard.CardData.RequiredPosture);
        }

        public bool CheckGarudPoint(PlanCard opponentCard, PlanCard targetCard)
        {
            if (opponentCard.CardData.GuardPoint.HasFlag(targetCard.CardData.Attack))
            {
                Invaildate(targetCard);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void TakeDamage(int damage, FieldController targetField)
        {
            targetField.TakeDamage(damage);
        }

        public void Disable(PlanCard targetCard)
        {
            targetCard.onCardOpen = null;
            targetCard.onCardTurn = null;
            targetCard.onCardSumStart = null;
            targetCard.onCardSum = null;
            targetCard.onCardSumEnd = null;
        }

        protected virtual int CalculateDamage(PlanCard playerCard, PlanCard opponentCard)
        {
            bool isOpponentPflug = opponentCard.PlayerFieldCtrl.PostureCtrl.CurrentPosture == Posture.PostureType.Pflug;
            return playerCard.CardData.Damage + playerCard.DamageBuff - playerCard.DamageDebuff - (isOpponentPflug ? 1 : 0);
        }

        public virtual void Open(FieldController player, FieldController opponent) { }

        public virtual void Turn(FieldController player, FieldController opponent)
        {
            player.PostureCtrl.SelectPosture(~Posture.PostureType.None);
            Disable(player.CurrentCard);
        }

        public virtual void SumStart(FieldController player, FieldController opponent)
        {
            if (player.CurrentCard.CardData == opponent.CurrentCard.CardData &&
                player.CurrentCard.CurrentCardDeployment == CardDeployment.Opened &&
                opponent.CurrentCard.CurrentCardDeployment == CardDeployment.Opened)
            {
                Invaildate(player.CurrentCard);
            }
        }

        public abstract void Sum(FieldController player, FieldController opponent);

        public virtual void SumEnd(FieldController player, FieldController opponent) { }
    }

    public abstract class ActionCard : ICardAction, ICardEffect
    {
        public virtual void Invaildate(PlanCard targetCard)
        {
            if (!targetCard.IsNotInvaild || targetCard.CurrentCardDeployment != CardDeployment.Disabled)
            {
                targetCard.Invaildated = true;
                if (targetCard.CurrentCardDeployment == CardDeployment.Turned)
                {
                    targetCard.PlayerFieldCtrl.PostureCtrl.UndoPosture();
                }
            }
        }

        public void Execute(PlanCard playerCard, PlanCard opponentCard, FieldController playerField, FieldController targetField) { }

        public bool IsPostureVaild(PlanCard playerCard)
        {
            return playerCard.PlayerFieldCtrl.PostureCtrl.CurrentPosture.HasFlag(playerCard.CardData.RequiredPosture);
        }

        public bool CheckGarudPoint(PlanCard opponentCard, PlanCard targetCard)
        {
            if (opponentCard.CardData.GuardPoint.HasFlag(targetCard.CardData.Attack))
            {
                Invaildate(targetCard);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Disable(PlanCard targetCard)
        {
            targetCard.onCardOpen = null;
            targetCard.onCardTurn = null;
            targetCard.onCardSumStart = null;
            targetCard.onCardSum = null;
            targetCard.onCardSumEnd = null;
        }

        public virtual void Open(FieldController player, FieldController opponent) { }

        public virtual void Turn(FieldController player, FieldController opponent) { }

        public virtual void SumStart(FieldController player, FieldController opponent) { }

        public virtual void Sum(FieldController player, FieldController opponent) { }

        public virtual void SumEnd(FieldController player, FieldController opponent) { }
    }

    /// <summary>
    /// 무효화 될 시, 2딜 증가
    /// </summary>
    public class Duhbacel : AttackCard
    {
        public const int extraDamage = 2;

        public override void Open(FieldController player, FieldController opponent)
        {
            RegistInvaild(player.CurrentCard);
        }

        public override void Sum(FieldController player, FieldController opponent)
        {
            RegistInvaild(player.CurrentCard);
            Execute(player.CurrentCard, opponent);
        }

        private void RegistInvaild(PlanCard playerPlanCard)
        {
            if (playerPlanCard.Invaildated)
            {
                playerPlanCard.IsNotInvaild = true;
                playerPlanCard.Invaildated = false;
                playerPlanCard.DamageBuff += extraDamage;
            }
        }
    }

    /// <summary>
    /// 이전 카드 복사
    /// </summary>
    public class Dupliaren : ActionCard
    {
        public override void Open(FieldController player, FieldController opponent)
        {
            int order = DuelManager.StrategyPlanOrder;
            if (order == 0) return;

            var prevPlanCardObj = ObjectPool.GetObject("Card Pool");
            prevPlanCardObj.GetComponent<PlanCard>().Initialize(player.StrategyPlans[order - 1].FirstPlacedPlanCard.CardData, player);
            player.PlaceCard(prevPlanCardObj);

            prevPlanCardObj.GetComponent<PlanCard>().onCardOpen.Invoke();
        }

        public override void Sum(FieldController player, FieldController opponent) { }
    }

    public class Rangote : AttackCard
    {
        public override void Sum(FieldController player, FieldController opponent)
        {
            Execute(player.CurrentCard, opponent);
        }
    }

    /// <summary>
    /// 상대 꺾기 금지,<br/>
    /// 무효화 시, 딜 이후 자세 전환
    /// </summary>
    public class Velpuren : AttackCard
    {
        public override void Open(FieldController player, FieldController opponent)
        {
            RegistInvaild(player.CurrentCard);
            opponent.CurrentCard.onCardTurn = null;
        }

        public override void Sum(FieldController player, FieldController opponent)
        {
            PlanCard playerPlanCard = player.CurrentCard;

            RegistInvaild(playerPlanCard);
            Execute(playerPlanCard, opponent);

            if (playerPlanCard.IsNotInvaild)
            {
                player.PostureCtrl.SelectPosture();
            }
        }

        private void RegistInvaild(PlanCard playerPlanCard)
        {
            if (playerPlanCard.Invaildated)
            {
                playerPlanCard.IsNotInvaild = true;
                playerPlanCard.Invaildated = false;
            }
        }
    }

    /// <summary>
    /// 꺾은 카드 or 행동 카드와 합하면 그 카드 무효화/<br/>
    /// 자세 전환 (상대 방도 동일하게 전환)
    /// </summary>
    public class Boudun : ActionCard
    {
        public override void Open(FieldController player, FieldController opponent)
        {
            PlanCard opponentPlanCard = opponent.CurrentCard;
            if (opponentPlanCard.CurrentCardDeployment == CardDeployment.Turned ||
                opponentPlanCard.CurrentCardDeployment == CardDeployment.Opened && opponentPlanCard.CardData.Damage == 0)
            {
                Invaildate(opponentPlanCard);
            }
        }

        public override void Sum(FieldController player, FieldController opponent)
        {
            var playerPostureChanger = player.PostureCtrl;
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
        public override void Open(FieldController player, FieldController opponent)
        {
            EffectShaitelhau(opponent.CurrentCard);
        }

        public override void Sum(FieldController player, FieldController opponent)
        {
            EffectShaitelhau(opponent.CurrentCard);

            Execute(player.CurrentCard, opponent);
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

    public class Shilhau : AttackCard
    {
        public override void Sum(FieldController player, FieldController opponent)
        {
            Execute(player.CurrentCard, opponent);
        }
    }

    /// <summary>
    /// 플루크 이행, 반사딜 1, 다음 횡베기 카드 2딜증 /
    /// </summary>
    public class Shulschel : ActionCard
    {
        public override void Open(FieldController player, FieldController opponent)
        {
            player.PostureCtrl.ChangePosture(Posture.PostureType.Pflug);

            int order = DuelManager.StrategyPlanOrder;
            PlanCard nextPlanCard;
            if (order < 3 && (nextPlanCard = player.GetCard(order + 1))
                .CardData.Attack == CardData.AttackType.Stab)
            {
                nextPlanCard.DamageBuff += 2;
            }

            UniTask.Void(async () =>
            {
                int prevHp = player.Hp;
                int prevOrder = DuelManager.StrategyPlanOrder;
                await UniTask.WaitUntil(() => player.Hp != prevHp || DuelManager.StrategyPlanOrder != prevOrder);
                //if (player.Hp < prevHp)
                //    TakeDamage(1, opponent);
            });
        }

        public override void Sum(FieldController player, FieldController opponent) { }
    }

    /// <summary>
    /// 상대 공격 무효화 못시키면, 내 다음 카드 비활성화
    /// </summary>
    public class Aufschtraechen : AttackCard
    {
        public override void Sum(FieldController player, FieldController opponent)
        {
            Execute(player.CurrentCard, opponent);
        }

        public override void SumEnd(FieldController player, FieldController opponent)
        {
            if (opponent.CurrentCard.Invaildated)
                Disable(player.GetCard(DuelManager.StrategyPlanOrder + 1));
        }
    }

    /// <summary>
    /// 댐 감소 X
    /// </summary>
    public class Jonhau : AttackCard
    {
        public override void Sum(FieldController player, FieldController opponent)
        {
            Execute(player.CurrentCard, opponent);
        }

        protected override int CalculateDamage(PlanCard player, PlanCard opponent)
        {
            return player.CardData.Damage + player.DamageBuff;
        }
    }

    /// <summary>
    /// 상대 올려, 내려베기 2딜 감소 /
    /// 폼탁 전환, 다음 카드 무효화 불가, 1딜 증가
    /// </summary>
    public class Johnhoot : ActionCard
    {
        public override void Open(FieldController player, FieldController opponent)
        {
            PlanCard opponentPlanCard = opponent.CurrentCard;
            if (opponentPlanCard.CardData.Attack == CardData.AttackType.UpwardCut ||
                opponentPlanCard.CardData.Attack == CardData.AttackType.DownwardCut)
            {
                opponentPlanCard.DamageDebuff -= 2;
            }
        }

        public override void Sum(FieldController player, FieldController opponent)
        {
            player.PostureCtrl.ChangePosture(Posture.PostureType.VomTag);

            if (DuelManager.StrategyPlanOrder < 3)
            {
                PlanCard nextPlanCard = player.GetCard(DuelManager.StrategyPlanOrder + 1);
                nextPlanCard.IsNotInvaild = true;
                nextPlanCard.DamageBuff += 1;
            }
        }
    }

    /// <summary>
    /// 상대 공격카드가 폼탁으로 시작 시, 공격카드를 무효화.
    /// </summary>
    public class Zverkhau : AttackCard
    {
        public override void Open(FieldController player, FieldController opponent)
        {
            ZverkhauEffect(opponent);
        }

        public override void Sum(FieldController player, FieldController opponent)
        {
            ZverkhauEffect(opponent);
            Execute(player.CurrentCard, opponent);
        }

        private void ZverkhauEffect(FieldController opponentField)
        {
            if (opponentField.PostureCtrl.CurrentPosture == Posture.PostureType.VomTag)
            {
                Invaildate(opponentField.CurrentCard);
            }
        }
    }

    /// <summary>
    /// 2 딜 이하 공격 무효화 /
    /// 공격 무효화 성공 시, 상대 다음 카드 비활성화
    /// </summary>
    public class Krumphau : ActionCard
    {
        public override void Open(FieldController player, FieldController opponent)
        {
            PlanCard opponentPlanCard = opponent.CurrentCard;

            if (opponentPlanCard.CardData.Damage != 0 &&
                opponentPlanCard.CardData.Damage + opponentPlanCard.DamageBuff - opponentPlanCard.DamageDebuff < 2)
            {
                Invaildate(opponentPlanCard);
            }
        }

        public override void Sum(FieldController player, FieldController opponent)
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
    public class Haeng : ActionCard
    {
        public override void Sum(FieldController player, FieldController opponent)
        {
            if (opponent.CurrentCard.Invaildated)
            {
                opponent.PostureCtrl.ChangePosture(Posture.PostureType.Alber);

                if (player == PlayerController.Instance)
                {
                    DuelManager.SetPlayerActionToken(UserType.Player);
                }
            }
        }
    }

    public class Ochs_Attack : AttackCard
    {
        public override void Sum(FieldController player, FieldController opponent)
        {
            Execute(player.CurrentCard, opponent);
        }

        public override void Turn(FieldController player, FieldController opponent) { }
    }
}
