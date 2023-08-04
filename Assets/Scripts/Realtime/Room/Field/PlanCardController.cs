using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Card;
using Card.Posture;
using Cysharp.Threading.Tasks;

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
        [SerializeField]
        private float cancelDistance = 1f;
        private bool isSelecting = false;
        private StrategyPlan CurrentStrategyPlan => PlayerController.Instance.StrategyPlans[DuelManager.StrategyPlanOrder];
        public bool IsSelecting => isSelecting;

        private void Awake()
        {
            InactivateOptions();
        }

        private void Update()
        {
            if (isSelecting && Input.GetMouseButtonUp(0))
            {
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo);
                if (Vector3.Distance(hitInfo.transform.position, transform.position) < cancelDistance) return;

                bool isDisabled = CurrentStrategyPlan.FirstPlacedPlanCard.onCardOpen == null;
                CardDeployment deployment = (hitInfo.transform.position.z - transform.position.z) switch
                {
                    >= 0f when !isDisabled => CardDeployment.Opened,
                    < 0f  when !isDisabled => CardDeployment.Turned,
                    _     when  isDisabled => CardDeployment.Disabled,
                    _ => throw new System.NotImplementedException(),
                };
                ChangeDeployment(CurrentStrategyPlan.FirstPlacedPlanCard, deployment);

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
            transform.position = planCard.CurrentStrategyPlan.transform.position;
            ActivateOptions(planCard);
        }

        private void ChangeDeployment(PlanCard target, CardDeployment deployment)
        {
            target.CurrentCardDeployment = deployment;
        }

        private void ActivateOptions(PlanCard planCard)
        {
            if (planCard.onCardOpen != null) opening.SetActive(true);
            else disabling.SetActive(true);

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
