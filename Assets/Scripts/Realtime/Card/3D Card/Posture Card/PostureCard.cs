using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Card.Posture
{
    public class PostureCard : MonoBehaviour
    {
        private Posture currentPosture = Posture.VomTag;
        public Posture CurrentPosture
        {
            get => currentPosture;
            set
            {
                currentPosture = value;

                RotatePosture(currentPosture);
            }
        }

        private void RotatePosture(Posture posture)
        {
            transform.rotation = posture switch
            {
                Posture.VomTag => Quaternion.Euler(0, 0, 0),
                Posture.Pflug => Quaternion.Euler(0, 45, 0),
                Posture.Ochs => Quaternion.Euler(0, 90, 0),
                Posture.Alber => Quaternion.Euler(0, 0, 180),
                _ => throw new InvalidOperationException(),
            };
        }
    }

    [Flags]
    public enum Posture
    {
        None = 0,
        /// <summary>
        /// 세로
        /// </summary>
        VomTag = 1,
        /// <summary>
        /// 대각선
        /// </summary>
        Pflug = 2,
        /// <summary>
        /// 가로
        /// </summary>
        Ochs = 4,
        /// <summary>
        /// 뒤
        /// </summary>
        Alber = 8,
    }
}
