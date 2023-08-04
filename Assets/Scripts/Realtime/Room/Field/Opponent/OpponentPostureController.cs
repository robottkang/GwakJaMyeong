using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Card.Posture;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

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
                SetPosture((Posture)photonEvent.CustomData);
            }
        }
    }
}
