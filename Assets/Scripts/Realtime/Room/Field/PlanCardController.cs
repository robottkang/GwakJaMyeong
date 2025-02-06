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
            HandlePlanCard();
        }

        private void HandlePlanCard()
        {
            if (isSelecting && Input.GetMouseButtonUp(0))
            {
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo);

                PlanCard planCard = CurrentStrategyPlan.FirstPlacedPlanCard;
                float angle = Vector3.SignedAngle(Vector3.forward, hitInfo.point - transform.position, Vector3.up);

                // forwards, Open
                if (angle <= 90f && angle >= -90f && !planCard.Disabled && CanOpen)
                {
                    planCard.Open();
                    SendDeployment(planCard, CardDeployment.Opened);
                }
                // backwards, Turn
                else if ((angle > 90f || angle < -90f) && !planCard.Disabled && CanTurn)
                {
                    planCard.Turn();
                    SendDeployment(planCard, CardDeployment.Turned);
                }
                // Disable
                else
                {
                    planCard.Disable();
                    SendDeployment(planCard, CardDeployment.Disabled);
                }

                isSelecting = false;
                InactivateOptions();

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

        private void SendDeployment(PlanCard target, CardDeployment deployment)
        {
            target.CurrentDeployment = deployment;

            new RaiseEventData<CardDeployment>(UserType.Player, deployment).RaiseDuelEvent(DuelEventCode.SetCardDepolyment);
            //PhotonNetwork.RaiseEvent((byte)DuelEventCode.SetCardDepolyment,
            //    JsonUtility.ToJson(new RaiseEventData(UserType.Player, (int)deployment)),
            //    RaiseEventOptions.Default,
            //    SendOptions.SendReliable);
        }

        private void ActivateOptions(PlanCard planCard)
        {
            if (planCard.Disabled)
            {
                disabling.SetActive(true);
                return;
            }

            if (planCard.CardData.RequiredPosture.HasFlag(PlayerController.Instance.PostureCtrl.CurrentPosture))
            {
                opening.SetActive(true);
            }
            else if (!planCard.CanTurn)
            {
                disabling.SetActive(true);
            }

            if (planCard.CanTurn)
                turning.SetActive(true);
        }

        private void InactivateOptions()
        {
            opening.SetActive(false);
            disabling.SetActive(false);
            turning.SetActive(false);
        }
    }
}
