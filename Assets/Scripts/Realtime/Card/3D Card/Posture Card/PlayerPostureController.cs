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
        [Header("- Posture Objects")]
        [SerializeField]
        private PostureInfo[] postureInfos = new PostureInfo[4];

        private void Start()
        {
            Initialize();
        }

        // change the posture's data and transform depending on the mouse position when on mouse up
        private void Update()
        {
            if (!(Input.GetMouseButtonUp(0) && isPostureChanging)) return;

            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit mouseRaycastHit, Mathf.Infinity, LayerMask.GetMask("Board"));

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

        public override void ChangePosture(PostureType posture)
        {
            SetPosture(posture);

            new RaiseEventData<PostureType>(UserType.Player, postureCard.CurrentPosture).RaiseDuelEvent(DuelEventCode.SendPosture);
            /*PhotonNetwork.RaiseEvent((byte)DuelEventCode.SendPosture,
                JsonUtility.ToJson(new RaiseEventData(UserType.Opponent, postureCard.CurrentPosture)),
                RaiseEventOptions.Default,
                SendOptions.SendReliable);*/

            if (PhaseManager.CurrentPhase != Phase.Duel) return;

            if (posture == PostureType.Ochs && PlayerController.Instance.CurrentCard.CurrentDeployment == CardDeployment.Turned)
            {
                PlaceOchs_acttack(PlayerController.Instance);
            }
        }

        private void SetPosture(PostureType posture)
        {
            prevPosture = PostureCard.CurrentPosture;
            postureCard.CurrentPosture = posture;
            isPostureChanging = false;
        }

        public override void SelectPosture(PostureType availablePosture = ~PostureType.None)
        {
            isPostureChanging = true;

            if (PostureCard.CurrentPosture == PostureType.Pflug)
            {
                availablePosture &= ~PostureType.Pflug;
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
            InactivatePostureInfos();
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.AreEventCodesEqual(DuelEventCode.SendPosture))
            {
                var data = photonEvent.ConvertEventData<PostureType>();

                if (data.TargetUser == UserType.Player)
                    SetPosture(data.content);
            }
        }

        [System.Serializable]
        private struct PostureInfo
        {
            public PostureType posture;
            public GameObject postureObject;
            [Tooltip("The foward direction is zero degrees")]
            [Range(-180f, 180f)]
            public float startAngle;
            [Range(-180f, 180f)]
            public float endAngle;
        }
    }
}
