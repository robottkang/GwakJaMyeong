using Card;
using Card.Posture;
using Room.Opponent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

        protected void PlaceOchs_acttack(FieldController targetField)
        {
            PlanCard ochsCard = ObjectPool.GetObject("Card Pool").GetComponent<PlanCard>();
            ochsCard.Initialize(ochs_attack, targetField);
            targetField.PlaceCard(ochsCard.gameObject);
            ochsCard.CurrentDeployment = CardDeployment.Turned;

            ochsCard.DOKill();
            ochsCard.transform.DOLocalMoveZ(0.5f, 0.5f);
            ochsCard.transform.DOLocalRotate(new Vector3(0f, 90f, 0f), 0.5f);
        }
    }
}
