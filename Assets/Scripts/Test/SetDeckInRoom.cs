using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using Room;
using Card;

public class SetDeckInRoom : MonoBehaviour
{
    [SerializeField]
    private List<CardInfo> deck;
    [Button]
    private void SetDeck()
    {
        if (Application.isPlaying && Application.isEditor)
        {
            DeckController.deckList = deck;
            GameManager.gameManager.DeckObject.SpawnTestCard();
        }
    }
}