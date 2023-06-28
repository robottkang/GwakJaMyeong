using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room
{
    public class PhaseManager : MonoBehaviour
    {
        public static PhaseManager Instance { get; private set; }

        private Phase currentPhase = Phase.WaitPlayer;
        private int turnCount = 1;

        public Phase CurrentPhase
        {
            get => currentPhase;
            set
            {
                currentPhase = value;

#if UNITY_EDITOR
                Debug.Log($"--- Current Page: {currentPhase} ---");
#endif
                PhaseEventBus.Publish(currentPhase);
            }
        }
        public int TurnCount => turnCount;

        private void Awake()
        {
            Instance = this;

            PhaseEventBus.Subscribe(Phase.End, () => turnCount++);
        }
    }
}
