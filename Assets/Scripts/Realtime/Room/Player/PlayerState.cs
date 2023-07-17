using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room
{
    public class PlayerState : MonoBehaviour, IPunObservable
    {
        [ReadOnly]
        public bool isReadyToPlay = false;
        [ReadOnly]
        public bool setPosture = false;
        [ReadOnly]
        public bool hasActionToken = false;
        [ReadOnly]
        public int StrategyPlanOrder = 0;
        [ReadOnly]
        public StrategyPlan.IdeaCardInfo currentIdeaCard;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // Write
            if (stream.IsWriting)
            {
                stream.SendNext(isReadyToPlay);
                stream.SendNext(setPosture);
                stream.SendNext(hasActionToken);
                stream.SendNext(GameManager.Instance.StrategyPlans[StrategyPlanOrder].PlacedCardInfo);
            }
            // Read
            else
            {
                isReadyToPlay = (bool)stream.ReceiveNext();
                setPosture = (bool)stream.ReceiveNext();
                hasActionToken = (bool)stream.ReceiveNext();
                currentIdeaCard = (StrategyPlan.IdeaCardInfo)stream.ReceiveNext();
            }
        }
    }
}
