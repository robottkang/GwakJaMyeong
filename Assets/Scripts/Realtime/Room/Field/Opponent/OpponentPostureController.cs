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

        private void SetPosture(PostureType posture)
        {
            prevPosture = postureCard.CurrentPosture;
            postureCard.CurrentPosture = posture;
            isPostureChanging = false;
        }

        public override void ChangePosture(PostureType posture)
        {
            SetPosture(posture);

            PhotonNetwork.RaiseEvent((byte)DuelEventCode.SendPosture,
                JsonUtility.ToJson(new PostureEventData()
                {
                    changer = UserType.Opponent,
                    posture = posture
                }),
                RaiseEventOptions.Default,
                SendOptions.SendReliable);
        }

        public override void SelectPosture(PostureType availablePosture = (PostureType)(-1))
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
            if (photonEvent.Code == (byte)DuelEventCode.SendPosture)
            {
                var data = JsonUtility.FromJson<PostureEventData>((string)photonEvent.CustomData);

                if (data.changer == UserType.Player)
                {
                    SetPosture(data.posture);

                    if (PhaseManager.CurrentPhase != Phase.Duel) return;
                    if (data.posture == PostureType.Ochs && OpponentController.Instance.CurrentCard.CurrentCardDeployment == CardDeployment.Turned)
                    {
                        PlanCard ochsCard = ObjectPool.GetObject("Card Pool").GetComponent<PlanCard>();
                        ochsCard.Initialize(ochs_attack, OpponentController.Instance);
                        OpponentController.Instance.PlaceCard(ochsCard.gameObject);
                        ochsCard.CurrentCardDeployment = CardDeployment.Turned;
                    }
                }
            }
        }
    }
}
