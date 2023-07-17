using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Card
{
    public class PostureCard : MonoBehaviour
    {
        private CardInfo.Posture currentPosture = CardInfo.Posture.VomTag;
        public CardInfo.Posture CurrentPosture
        {
            get => currentPosture;
            set
            {
                currentPosture = value;

                RotatePosture(currentPosture);
            }
        }

        private void RotatePosture(CardInfo.Posture posture)
        {
            transform.rotation = posture switch
            {
                CardInfo.Posture.VomTag => Quaternion.Euler(0, 0, 0),
                CardInfo.Posture.Pflug => Quaternion.Euler(0, 45, 0),
                CardInfo.Posture.Ochs => Quaternion.Euler(0, 90, 0),
                CardInfo.Posture.Alber => Quaternion.Euler(0, 0, 180),
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
