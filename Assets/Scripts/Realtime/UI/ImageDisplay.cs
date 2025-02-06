using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Card.Posture;
using UnityEngine.UI;

namespace UI
{
    public class ImageDisplay : MonoBehaviour, IInfoDisplay
    {
        [SerializeField]
        private Image guideImage = null;
        [SerializeField]
        private Image hightlighter = null;
        [SerializeField]
        private Sprite postureGuideSprite = null;

        private GuideType displayingGuideType = 0;

        private enum GuideType
        {
            posture,

        }

        public bool TrySetDisplayData(GameObject target)
        {
            if (target.TryGetComponent(out PostureCard _))
            {
                displayingGuideType = GuideType.posture;
                return true;
            }

            return false;
        }

        public void DisplayInfo()
        {
            switch (displayingGuideType)
            {
                case GuideType.posture:
                    guideImage.sprite = postureGuideSprite;
                    // 하이라이터 작동시키기
                    break;
                default:
                    throw new System.Exception("Unexpected Guide Type");
            }
        }

        public void SetActiveDisplay(bool value)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(value);
            }

        }
    }
}
