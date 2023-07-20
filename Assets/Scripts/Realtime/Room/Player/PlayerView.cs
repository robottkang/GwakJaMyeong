using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room
{
    public class PlayerView : MonoBehaviourPun, IPunObservable
    {
        [ReadOnly]
        public bool readyToPlay = false;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // Write
            if (stream.IsWriting)
            {
                stream.SendNext(readyToPlay);
            }
            // Read
            else
            {
                readyToPlay = (bool)stream.ReceiveNext();
            }
        }
    }
}
