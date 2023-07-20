using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Room
{
    public class DuelManager : MonoBehaviour
    {
        public static DuelManager Instance { get; private set; }
        public bool setPosture = false;
        public bool hasActionToken = false;
        public int StrategyPlanOrder = 0;
        public Card.PlanCardInfo currentPlanCard;


        private void Awake()
        {
            Instance = GetComponent<DuelManager>();
        }
    }
}
