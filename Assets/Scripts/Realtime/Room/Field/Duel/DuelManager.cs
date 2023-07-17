using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Room
{
    public class DuelManager : MonoBehaviour
    {
        public static DuelManager Instance { get; private set; }


        private void Awake()
        {
            Instance = GetComponent<DuelManager>();
        }

        public static void Invaildate(bool condition)
        {

        }
    }
}
