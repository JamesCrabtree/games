using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play {
    public List<simCard> cardsPlayed;
    public Dictionary<Attribute, List<simCard>> abilitiesUsed;
    public Dictionary<simCard, simCard> attacks;

    public Play(List<simCard> cp, Dictionary<Attribute, List<simCard>> abilities, Dictionary<simCard, simCard> att)
    {
        cardsPlayed = cp;
        abilitiesUsed = abilities;
        attacks = att;
    }

    public Play(Play play)
    {
        cardsPlayed = new List<simCard>();
        abilitiesUsed = new Dictionary<Attribute, List<simCard>>();
        attacks = new Dictionary<simCard, simCard>();
        foreach (simCard card in play.cardsPlayed)
            cardsPlayed.Add(card);
        foreach(KeyValuePair<Attribute, List<simCard>> pair in play.abilitiesUsed)
        {
            abilitiesUsed[pair.Key] = new List<simCard>();
            foreach(simCard card in pair.Value)
            {
                abilitiesUsed[pair.Key].Add(new simCard(card));
            }
        }
        foreach(KeyValuePair<simCard, simCard> pair in play.attacks)
        {
            attacks[new simCard(pair.Key)] = new simCard(pair.Value);
        }
    }
}
