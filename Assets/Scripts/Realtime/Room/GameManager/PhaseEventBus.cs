using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Room
{
    public class PhaseEventBus : MonoBehaviour
    {
        private static readonly IDictionary<Phase, UnityEvent> Events = new Dictionary<Phase, UnityEvent>();

        public static void Subscribe(Phase eventType, UnityAction listener)
        {
            UnityEvent thisEvent;

            if (Events.TryGetValue(eventType, out thisEvent))
            {
                thisEvent.AddListener(listener);
            }
            else
            {
                thisEvent = new UnityEvent();
                thisEvent.AddListener(listener);
                Events.Add(eventType, thisEvent);
            }
        }

        public static void Unsubscribe(Phase type, UnityAction listener)
        {
            UnityEvent thisEvent;

            if (Events.TryGetValue(type, out thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
        }

        public static void UnsubscribeAll(Phase type)
        {
            UnityEvent thisEvent;

            if (Events.TryGetValue(type, out thisEvent))
            {
                thisEvent.RemoveAllListeners();
            }
        }

        public static void Clear()
        {
            Events.Clear();
        }

        public static void Publish(Phase type)
        {
            UnityEvent thisEvent;

            if (Events.TryGetValue(type, out thisEvent))
            {
                thisEvent.Invoke();
            }
        }
    }

    public enum Phase
    {
        WaitPlayer,
        Draw,
        StrategyPlan,
        Duel,
        DuelFirstStep,
        DuelSecondStep,
        DuelThirdStep,
        End,
    }
}
