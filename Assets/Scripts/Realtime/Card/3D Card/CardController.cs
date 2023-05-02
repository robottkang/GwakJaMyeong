using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer cardSprite;
    [SerializeField]
    private CardInfo cardInfo;

    public bool CanMove { get; set; }
    public CardInfo CardInfo
    {
        get => cardInfo;
        set => cardInfo = value;
    }

    private void Start()
    {
        cardSprite.sprite = cardInfo != null ? cardInfo.CardSprite : null;
    }
}
