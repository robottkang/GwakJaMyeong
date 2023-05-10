using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Card;

namespace DeckBuilding
{
    public class SpawnerCard : MonoBehaviour
    {
        [SerializeField]
        private CardInfo cardInfo;
        [SerializeField]
        private GameObject card;

        private void Awake()
        {
            for (int i = 0; i < 3; i++)
            {
                var spawnedCard = Instantiate(card, transform);
                spawnedCard.GetComponent<DeckBuildingCard>().CardInfo = cardInfo;
            }
        }
    }
}
    