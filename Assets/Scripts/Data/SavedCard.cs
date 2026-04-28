using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class SavedCardHelper
{
    public static SavedCard save(this Card card)
    {
        return new()
        {
            cardId = card.cardTag
        };
    }
}
[System.Serializable]
public class SavedCard
{
    public string cardId;
    public Card load()
    {
        return CardManager.getCardByTag(cardId);
    }
}
