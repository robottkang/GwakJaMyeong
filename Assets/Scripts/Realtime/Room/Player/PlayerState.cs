using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room
{
    public class PlayerState : MonoBehaviourPun, IPunObservable
    {
        [HideInInspector]
        public bool isReadyToPlay;
        [HideInInspector]
        public bool isMyAttackTurn;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // Write
            if (stream.IsWriting)
            {
                stream.SendNext(isReadyToPlay);
                stream.SendNext(isMyAttackTurn);
            }
            // Read
            else
            {
                isReadyToPlay = (bool)stream.ReceiveNext();
                isMyAttackTurn = (bool)stream.ReceiveNext();
            }
        }
    }
}
