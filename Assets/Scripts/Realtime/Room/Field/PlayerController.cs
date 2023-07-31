using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room
{
    public class PlayerController : FieldController
    {
        public static PlayerController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}
