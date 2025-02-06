using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Card.Posture
{
    public class PostureCard : MonoBehaviour
    {
        private PostureType currentPosture = PostureType.VomTag;
        public PostureType CurrentPosture
        {
            get => currentPosture;
            set
            {
                currentPosture = value;

                RotatePosture(currentPosture);
            }
        }

        private void RotatePosture(PostureType posture)
        {
            transform.DOLocalRotateQuaternion(posture switch
            {
                PostureType.VomTag => Quaternion.Euler(0, 0, 0),
                PostureType.Pflug => Quaternion.Euler(0, 45, 0),
                PostureType.Ochs => Quaternion.Euler(0, 90, 0),
                PostureType.Alber => Quaternion.Euler(0, 0, 180),
                _ => throw new InvalidOperationException(),
            }, 0.25f);
        }
    }

    [Flags, Serializable]
    public enum PostureType
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
