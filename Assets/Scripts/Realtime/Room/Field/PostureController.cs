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
        protected Posture prevPosture = Posture.None;
        [SerializeField, ReadOnly()]
        protected bool isPostureChanging = false;

        public PostureCard PostureCard => postureCard;
        public Posture CurrentPosture => postureCard.CurrentPosture;
        public bool IsPostureChanging => isPostureChanging;

        public abstract void ChangePosture(Posture posture);

        public abstract void SelectPosture(Posture availablePosture = (Posture)(-1));

        public abstract void UndoPosture();
    }
}
