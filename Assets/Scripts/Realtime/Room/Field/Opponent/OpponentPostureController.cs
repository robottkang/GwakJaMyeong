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

            new RaiseEventData<PostureType>(UserType.Opponent, posture).RaiseDuelEvent(DuelEventCode.SendPosture);
            //PhotonNetwork.RaiseEvent((byte)DuelEventCode.SendPosture,
            //    JsonUtility.ToJson(new RaiseEventData(UserType.Opponent, posture)),
            //    RaiseEventOptions.Default,
            //    SendOptions.SendReliable);
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
            if (photonEvent.AreEventCodesEqual(DuelEventCode.SendPosture))
            {
                var data = photonEvent.ConvertEventData<PostureType>();

                if (data.TargetUser == UserType.Opponent)
                {
                    SetPosture(data.content);

                    if (PhaseManager.CurrentPhase != Phase.Duel) return;
                    if (data.content == PostureType.Ochs && OpponentController.Instance.CurrentCard.CurrentDeployment == CardDeployment.Turned)
                    {
                        PlaceOchs_acttack(OpponentController.Instance);
                    }
                }
            }
        }
    }
}
