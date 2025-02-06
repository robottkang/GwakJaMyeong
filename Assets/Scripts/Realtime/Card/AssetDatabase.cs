using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Card;

public partial class AssetDatabase : SingletonMonoBehaviour<AssetDatabase>
{
    [SerializeField]
    private CardData[] cardsData;

    public CardData[] CardsData => cardsData;
    public Dictionary<CardData.CardCode, Sprite> CardSprites => cardsData.ToDictionary(x => x.ThisCardCode, x => x.CardSprite);
}
