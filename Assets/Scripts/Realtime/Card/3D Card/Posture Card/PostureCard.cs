using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Card
{
    public class PostureCard : MonoBehaviour
    {
        private Posture currentPosture = Posture.VomTag;
        public Posture CurrentPosture
        {
            get => currentPosture;
            set
            {
                switch (value)
                {
                    case Posture.VomTag:
                        transform.rotation = Quaternion.Euler(0, 0, 0);
                        break;
                    case Posture.Pflug:
                        transform.rotation = Quaternion.Euler(0, 45, 0);
                        break;
                    case Posture.Ochs:
                        transform.rotation = Quaternion.Euler(0, 90, 0);
                        break;
                    case Posture.Alber:
                        transform.rotation = Quaternion.Euler(0, 0, 180);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }

    [Flags]
    public enum Posture
    {
        None = 0,
        VomTag = 1,
        Pflug = 2,
        Ochs = 4,
        Alber = 8,
    }
}
