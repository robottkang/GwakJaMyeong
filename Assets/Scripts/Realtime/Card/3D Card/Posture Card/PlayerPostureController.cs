using Card;
using Card.Posture;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Room.Opponent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room
{
    public class PlayerPostureController : PostureController, IOnEventCallback
    {
        private Camera mainCamera;
        [Header("- Posture Objects")]
        [SerializeField]
        private PostureInfo[] postureInfos = new PostureInfo[4];

        private void Awake()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        // change the posture's data and transform depending on the mouse position when on mouse up
        private void Update()
        {
            if (!(Input.GetMouseButtonUp(0) && isPostureChanging)) return;

            Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit mouseRaycastHit, Mathf.Infinity, LayerMask.GetMask("Board"));

            Vector3 v = mouseRaycastHit.point - transform.position;
            const float minDistanceToChangePosture = 1f;

            if (v.magnitude > minDistanceToChangePosture)
            {
                float angle = Vector3.SignedAngle(Vector3.forward, v, Vector3.up);
                ChangePosture(angle);
            }
        }

        private void ChangePosture(float angle)
        {
            foreach (var postureInfo in postureInfos)
            {
                // if point is in the range of the angle
                if ((postureInfo.startAngle < postureInfo.endAngle && postureInfo.startAngle <= angle && angle <= postureInfo.endAngle) ||
                    (postureInfo.startAngle > postureInfo.endAngle && (postureInfo.startAngle <= angle || angle <= postureInfo.endAngle)))
                {
                    ChangePosture(postureInfo.posture);
                    InactivatePostureInfos();
                    return;
                }
            }
        }

        public override void ChangePosture(Posture posture)
        {
            SetPosture(posture);

            PhotonNetwork.RaiseEvent(
                (byte)DuelEventCode.SendMyPosture,
                postureCard.CurrentPosture,
                RaiseEventOptions.Default,
                SendOptions.SendReliable);

            if (PhaseManager.CurrentPhase != Phase.Duel) return;

            if (posture == Posture.Ochs && PlayerController.Instance.CurrentCard.CurrentCardDeployment == CardDeployment.Turned)
            {
                GameObject ochsCard = ObjectPool.GetObject("Card Pool");
                ochsCard.GetComponent<PlanCard>().Initialize(ochs_attack, PlayerController.Instance);
                PlayerController.Instance.PlaceCard(ochsCard);
                ochsCard.GetComponent<PlanCard>().CurrentCardDeployment = CardDeployment.Turned;
            }
        }

        private void SetPosture(Posture posture)
        {
            prevPosture = PostureCard.CurrentPosture;
            postureCard.CurrentPosture = posture;
            isPostureChanging = false;
        }

        public override void SelectPosture(Posture availablePosture = ~Posture.None)
        {
            isPostureChanging = true;

            if (PostureCard.CurrentPosture == Posture.Pflug)
            {
                availablePosture &= ~Posture.Pflug;
            }

            foreach (var postureInfo in postureInfos)
            {
                if (availablePosture.HasFlag(postureInfo.posture))
                    postureInfo.postureObject.SetActive(true);
            }
        }

        private void InactivatePostureInfos()
        {
            foreach (var postureInfo in postureInfos)
            {
                postureInfo.postureObject.SetActive(false);
            }
        }

        public override void UndoPosture()
        {
            ChangePosture(prevPosture);
            prevPosture = PostureCard.CurrentPosture;
        }

        private void Initialize()
        {
            mainCamera = Camera.main;

            InactivatePostureInfos();
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (byte)DuelEventCode.SendOpponentPosture)
            {
                SetPosture((Posture)photonEvent.CustomData);
            }
        }

        [System.Serializable]
        private struct PostureInfo
        {
            public Posture posture;
            public GameObject postureObject;
            [Tooltip("The foward direction is zero degrees")]
            [Range(-180f, 180f)]
            public float startAngle;
            [Range(-180f, 180f)]
            public float endAngle;
        }
    }
}
