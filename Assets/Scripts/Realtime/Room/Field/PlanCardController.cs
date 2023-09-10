using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Card;
using Card.Posture;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace Room
{
    public class PlanCardController : MonoBehaviour
    {
        [Header("- References")]
        [SerializeField]
        private GameObject opening;
        [SerializeField]
        private GameObject turning;
        [SerializeField]
        private GameObject disabling;
        private bool isSelecting = false;
        private bool selectsDeployment = false;
        private StrategyPlan CurrentStrategyPlan => PlayerController.Instance.StrategyPlans[DuelManager.StrategyPlanOrder];
        public bool IsSelecting => isSelecting;
        public bool CanOpen => opening.activeSelf;
        public bool CanTurn => turning.activeSelf;

        private void Awake()
        {
            InactivateOptions();
        }

        private void Update()
        {
            if (isSelecting && !selectsDeployment && Input.GetMouseButtonUp(0))
            {
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo);

                bool isDisabled = CurrentStrategyPlan.FirstPlacedPlanCard.onCardOpen == null;
                CardDeployment deployment = (hitInfo.point.z - transform.position.z) switch
                {
                    >= 0f when !isDisabled && CanOpen => CardDeployment.Opened,
                    < 0f  when !isDisabled && CanTurn => CardDeployment.Turned,
                    _ => CardDeployment.Disabled,
                };
                ChangeDeployment(CurrentStrategyPlan.FirstPlacedPlanCard, deployment);
                selectsDeployment = true;

                UniTask.Void(async () =>
                {
                    await UniTask.WaitUntil(() => !FieldController.IsChangingAnyPosture);
                    isSelecting = false;
                    InactivateOptions();
                    DuelManager.SwapActionToken();
                });
                Debug.Log("Complete to select");
            }
        }

        /// <summary>
        /// Appear the options to select deployment of the plan card.<br/>
        /// After selecting, send changed deployment of plan card to opponent and swap action token.
        /// </summary>
        public void SelectDeployment(PlanCard planCard)
        {
            isSelecting = true;
            selectsDeployment = false;
            transform.position = planCard.CurrentStrategyPlan.transform.position;
            ActivateOptions(planCard);
        }

        private void ChangeDeployment(PlanCard target, CardDeployment deployment)
        {
            target.CurrentCardDeployment = deployment;

            PhotonNetwork.RaiseEvent(
                (byte)DuelEventCode.SendCardDepolyment,
                (int)deployment,
                RaiseEventOptions.Default,
                SendOptions.SendReliable);
        }

        private void ActivateOptions(PlanCard planCard)
        {
            if (planCard.CardData.RequiredPosture.HasFlag(PlayerController.Instance.PostureController.CurrentPosture))
            {
                if (planCard.onCardOpen != null) opening.SetActive(true);
                else disabling.SetActive(true);
            }
            else
            {
                if (planCard.onCardTurn == null) disabling.SetActive(true);
            }

            if (planCard.onCardTurn != null) turning.SetActive(true);
        }

        private void InactivateOptions()
        {
            opening.SetActive(false);
            disabling.SetActive(false);
            turning.SetActive(false);
        }
    }
}
