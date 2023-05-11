using Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostureChanger : MonoBehaviour
{
    [Header("- Posture Objects")]
    [SerializeField]
    private PostureInfo[] postureInfos = new PostureInfo[4];

    [Space]
    [SerializeField]
    private PostureCard postureCard;
    //[HideInInspector]
    public Posture availablePosture;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        Initialize();
    }

    private void OnMouseDown()
    {
        foreach (var postureInfo in postureInfos)
        {
            if (availablePosture.HasFlag(postureInfo.posture))
                postureInfo.postureObject.SetActive(true);
        }
    }

    private void OnMouseUp()
    {
        Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit mouseRaycastHit, Mathf.Infinity, LayerMask.GetMask("Board"));
        
        Vector3 v = mouseRaycastHit.point - transform.position;
        const float minDistanceToChangePosture = 1f;

        if (v.magnitude > minDistanceToChangePosture)
        {
            float angle = Vector3.SignedAngle(Vector3.forward, v, Vector3.up);
            SetPosture(angle);
        }

        foreach (var postureInfo in postureInfos)
        {
            if (availablePosture.HasFlag(postureInfo.posture))
                postureInfo.postureObject.SetActive(false);
        }
    }

    private void SetPosture(float angle)
    {
        foreach (var postureInfo in postureInfos)
        {
            if (postureInfo.startAngle < postureInfo.endAngle && postureInfo.startAngle <= angle && angle <= postureInfo.endAngle)
            {
                postureCard.CurrentPosture = postureInfo.posture;
                return;
            }
            else if (postureInfo.startAngle > postureInfo.endAngle && (postureInfo.startAngle <= angle || angle <= postureInfo.endAngle))
            {
                postureCard.CurrentPosture = postureInfo.posture;
                return;
            }
        }
    }

    public void Initialize()
    {
        foreach (var postureInfo in postureInfos)
        {
            postureInfo.postureObject.SetActive(false);
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
