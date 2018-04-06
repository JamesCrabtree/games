using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : ScriptableObject{
    public Attribute[] attributes;
    public GameObject cardPrefab;
    private System.Random rnd = new System.Random();

    public CardManager()
    {
        attributes = new Attribute[11];
        generateAttributes();
    }

    public Stack<GameObject> generateDeck()
    {
        Stack<GameObject> cardStack = new Stack<GameObject>();
        List<GameObject> cards = new List<GameObject>();
        int count = 0;
        int backCount = 2;
        int cost = 1;
        for (int i = 0; i < 25; i++)
        {
            if (cost <= 4 && ((cost % 2 == 0 && count == cost + 2) || count == cost+1))
            {
                //Debug.Log(cost + " " + count);
                cost++;
                count = 0;
            }
            if(cost == 5 && count == 3)
            {
                //Debug.Log(cost + " " + count);
                cost++;
                count = 0;
            }
            else if (cost > 5 && count == cost - backCount-1)
            {
                //Debug.Log(cost + " " + count);
                backCount += 2;
                cost++;
                count = 0;
            }
            count++;
            cards.Add(generateCard(cost));
        }
        for (int i = 0; i < 25; i++)
        {
            int r = rnd.Next(cards.Count);
            cardStack.Push(cards[r]);
            cards.Remove(cards[r]);
        }

        return cardStack;
    }

    public GameObject generateCard(int cost)
    {
        int max = 11;
        GameObject newCard = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Card card = newCard.GetComponent<Card>();

        card.cost = cost;
        int boost = cost/2;
        card.damage = card.damage + boost;
        cost -= boost;
        boost = rnd.Next(cost/2, cost);
        card.health = card.health + boost;
        card.origHealth = card.health;
        cost -= boost;
        while (cost > 0)
        {
            Attribute attribute = new Attribute(attributes[rnd.Next(max)]);
            //cost -= attribute.cost;
            if (!attribute.category.Equals("Taunt"))
            {
                if(cost > 0)
                {
                    boost = rnd.Next(cost);
                    attribute.numTargets = attribute.numTargets + boost;
                    cost -= boost;
                }
                if (cost > 0)
                {
                    //cost = rnd.Next(cost);
                    attribute.value = attribute.value + cost;
                    cost -= cost;
                }
            }
            else
                max = 10;
            attribute.setDescription();
            card.attributes.Add(attribute);
            card.description = card.description + attribute.description;
        }
        return newCard;
    }

    public void generateAttributes()
    {
        string[] triggers = { "Battlecry", "Deathrattle" };
        string[] categories = { "Damage", "Heal", "BuffH", "BuffA", "DebuffA" };
        for(int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if(categories[j].Equals("Damage") || categories[j].Equals("DebuffA"))
                    attributes[5 * i + j] = new Attribute(triggers[i], 1, categories[j], 1, 1, "Enemy");
                else
                    attributes[5 * i + j] = new Attribute(triggers[i], 1, categories[j], 1, 1, "Ally");
            }
        }
        attributes[10] = new Attribute("Battlecry", 0, "Taunt", 0, 1, "Player");
    }
}
