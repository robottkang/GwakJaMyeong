using Card.Posture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room
{
    public abstract class PostureController : MonoBehaviour
    {
        [Header("- References")]
        [SerializeField]
        protected PostureCard postureCard;
        [SerializeField]
        protected Card.CardData ochs_attack;
        protected PostureType prevPosture = PostureType.None;
        [SerializeField, ReadOnly()]
        protected bool isPostureChanging = false;

        public PostureCard PostureCard => postureCard;
        public PostureType CurrentPosture => postureCard.CurrentPosture;
        public bool IsPostureChanging => isPostureChanging;

        public abstract void ChangePosture(PostureType posture);

        public abstract void SelectPosture(PostureType availablePosture = (PostureType)(-1));

        public abstract void UndoPosture();
    }
}
