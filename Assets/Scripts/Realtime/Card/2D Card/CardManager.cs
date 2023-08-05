using System.Collection;
using System.Collection.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Card.UI
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject displayerPrefab;

        public CardManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == nul)
            {
                Instance = this;
                DontDestroyOnLoad(thi);
            }
            else Destroy(gameObject);
        }
    }
}