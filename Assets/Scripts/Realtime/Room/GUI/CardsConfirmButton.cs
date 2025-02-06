using DG.Tweening;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Room.UI
{
    public class CardsConfirmButton : MonoBehaviour
    {
        private Image image;
        private CanvasGroup canvasGroup;
        private Vector3 originPosition;

        private void Awake()
        {
            gameObject.SetActive(false);
            image = GetComponent<Image>();
            (canvasGroup = GetComponent<CanvasGroup>()).alpha = 0f;

            PhaseEventBus.Subscribe(Phase.StrategyPlan, () =>
            {
                gameObject.SetActive(true);
                canvasGroup.DOFade(1f, 0.5f);
            });
        }

        private void Start()
        {
            originPosition = transform.position;
        }

        public void TrySendPlanCards()
        {
            for (int i = 0; i < 3; i++)
            {
                Card.PlanCard planCard;
                if ((planCard = PlayerController.Instance.GetCard(i)) == null)
                {
                    transform.position = originPosition;
                    transform.DOShakePosition(0.4f, 3f, 10, 90f, false, true);
                    image.DOColor(Color.red, 0.2f)
                        .OnComplete(() => image.DOColor(Color.white, 0.5f));
                    return;
                }
            }

            PlayerController.Instance.SendPlanCards();

            canvasGroup.DOFade(0f, 0.5f)
                .OnComplete(() => gameObject.SetActive(false));
        }
    }
}
