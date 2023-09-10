using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Card.Posture;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Card;

namespace Room.Opponent
{
    public class OpponentPostureController : PostureController, IOnEventCallback
    {
        private void Awake()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void SetPosture(Posture posture)
        {
            prevPosture = postureCard.CurrentPosture;
            postureCard.CurrentPosture = posture;
            isPostureChanging = false;
        }

        public override void ChangePosture(Posture posture)
        {
            SetPosture(posture);

            PhotonNetwork.RaiseEvent((byte)DuelEventCode.SendOpponentPosture,
                posture,
                RaiseEventOptions.Default,
                SendOptions.SendReliable);
        }

        public override void SelectPosture(Posture availablePosture = (Posture)(-1))
        {
            isPostureChanging = true;
        }

        public override void UndoPosture()
        {
            ChangePosture(prevPosture);
        }

        // Receive Opponent's Posture Change event
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (byte)DuelEventCode.SendMyPosture)
            {
                Posture posture = (Posture)photonEvent.CustomData;
                SetPosture(posture);

                if (PhaseManager.CurrentPhase != Phase.Duel) return;
                if (posture == Posture.Ochs && OpponentController.Instance.CurrentCard.CurrentCardDeployment == CardDeployment.Turned)
                {
                    GameObject ochsCard = ObjectPool.GetObject("Card Pool");
                    ochsCard.GetComponent<PlanCard>().Initialize(ochs_attack, OpponentController.Instance);
                    OpponentController.Instance.PlaceCard(ochsCard);
                    ochsCard.GetComponent<PlanCard>().CurrentCardDeployment = CardDeployment.Turned;
                }
            }
        }
    }
}
