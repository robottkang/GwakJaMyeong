using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Room.UI
{
    public class Timer : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI timeText;
        public UnityEvent OnTimeout { private set; get; } = new();

        private float duration = 60f;
        private float startTime = 0f;
        private bool startsTimer = false;

        private void Update()
        {
            if (startsTimer)
            {
                timeText.text = Mathf.Ceil(startTime + duration - Time.time).ToString();

                if (startTime + duration < Time.time)
                {
                    OnTimeout.Invoke();
                    startsTimer = false;
                    Debug.Log("Timer end");
                }
            }
        }

        public void StartTimer(float duration)
        {
            if (!startsTimer)
            {
                this.duration = duration;
                startsTimer = true;
                startTime = Time.time;
            }
            else
            {
                Debug.Log("Started");
            }
        }

        public void StopTimer()
        {
            startsTimer = false;
        }
    }
}
