using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour {

    private Player player1;
    float time = 0.0f;
    private Player player2;
    private BoardState boardState; 

	// Use this for initialization
	public void setup ()
    {
        player1 = gameObject.GetComponent<Player>();
        player2 = player1.enemy.GetComponent<Player>();
        boardState = new BoardState(player1, player1.cardsOnBoard, player2.cardsOnBoard, 
            player1.cardsInHand);
	}

    private void Update()
    {
        time += Time.deltaTime;
    }

    public void calculateTurn()
    {
        boardState.update(player1, player1.cardsOnBoard, player2.cardsOnBoard,
            player1.cardsInHand, player1.health, player2.health);
        Play play = makeTree(boardState);
        foreach(simCard card in play.cardsPlayed)
        {
            if (player1.cardsOnBoard.Count > 5)
                break;
            player1.playCard(simToReal(card, player1.cardsInHand));
            foreach(Attribute att in card.attributes)
            {
                player1.currAtt = att;
                if (att.trigger.Equals("Battlecry") && !att.category.Equals("Taunt"))
                {
                    foreach(simCard target in play.abilitiesUsed[att])
                    {
                        GameObject attTarget = null;
                        if (att.targetType.Equals("Ally"))
                            attTarget = simToReal(target, player1.cardsOnBoard);
                        else
                            attTarget = simToReal(target, player2.cardsOnBoard);
                        if(attTarget)
                            player1.cardsSelected.Add(attTarget);
                    }
                }
                player1.useAbility();
            }
        }
        foreach (KeyValuePair<simCard,simCard> pair in play.attacks)
        {
            GameObject attacker = simToReal(pair.Key, player1.cardsOnBoard);
            GameObject target = simToReal(pair.Value, player2.cardsOnBoard);
            if (attacker && target)
                player1.attack(attacker, target);
            else if (attacker)
                player1.attack(attacker, player1.enemy);
        }
        player1.endTurn();
    }

    public GameObject simToReal(simCard sim, List<GameObject> cards)
    {
        foreach (GameObject myCard in cards)
        {
            Card handCard = myCard.GetComponent<Card>();
            if (sim.cost == handCard.cost &&
                sim.damage == handCard.damage &&
                sim.health == handCard.health &&
                sim.attributes.Equals(handCard.attributes))
            {
                return myCard;
            }
        }
        return null;
    }

    public void selectTargets()
    {
        if (player1.currAtt.category.Equals("Heal") ||
            player1.currAtt.category.Equals("BuffA") ||
            player1.currAtt.category.Equals("BuffH"))
        {
            foreach (GameObject card in player1.cardsOnBoard)
            {
                if (card != player1.cardSelecting)
                {
                    player1.cardsSelected.Add(card);
                    player1.selecting--;
                }
                if (player1.selecting == 0)
                {
                    player1.useAbility();
                    break;
                }
            }
        }
        else
        {
            foreach (GameObject card in player1.enemy.GetComponent<Player>().cardsOnBoard)
            {
                if (card != player1.cardSelecting)
                {
                    player1.cardsSelected.Add(card);
                    player1.selecting--;
                }
                if (player1.selecting == 0)
                {
                    player1.useAbility();
                    break;
                }
            }
        }
    }

    private Play makeTree(BoardState bState)
    {
        Dictionary<Play, int> possPlays = new Dictionary<Play, int>();

        List<Play> plays = determinePlays(bState);
        int total = 0;
        foreach(Play play in plays)
        {
            total++;
            possPlays[play] = runSimulation(play, bState, 1);
            if (total > 10)
                break;

        }
        Play bestPlay  = new Play(new List<simCard>(), new Dictionary<Attribute, List<simCard>>(), new Dictionary<simCard, simCard>());
        int bestVal = -4000000;
        foreach (KeyValuePair<Play, int> item in possPlays)
        {
            if(item.Value > bestVal)
            {
                bestPlay = item.Key;
                bestVal = item.Value;
            }

        }
        return bestPlay;
    }

    private List<Play> determinePlays(BoardState bState)
    {
        List<Play> cardPlays = pickCards(new List<simCard>(), bState.playerHandCards, player1.totalMana);

        List<Play> attrPlays = new List<Play>();
        List<Play> attackPlays = new List<Play>();
        foreach (Play play in cardPlays)
        {
            List<Attribute> attributes = new List<Attribute>();
            foreach (simCard card in play.cardsPlayed)
            {
                foreach (Attribute att in card.attributes)
                {
                    if (att.trigger.Equals("Battlecry") && !att.category.Equals("Taunt"))
                    {
                        attributes.Add(att);
                        play.abilitiesUsed[att] = new List<simCard>();
                    }
                }
            }
            attrPlays.AddRange(selectTargets(play, attributes, bState.playerBoardCards, bState.enemyBoardCards));
        }
        foreach (Play play in attrPlays)
        {
            attackPlays.AddRange(attack(play, bState.playerBoardCards, bState.enemyBoardCards));
        }
        return attackPlays;
    }

    private List<Play> pickCards(List<simCard> soFar, List<simCard> hand, int mana)
    {
        bool pickedCard = false;
        List<Play> plays = new List<Play>();
        foreach (simCard card in hand)
        {
            if (card.cost < mana)
            {
                pickedCard = true;
                int curr_mana = mana - card.cost;
                List<simCard> newHand = new List<simCard>();
                newHand.AddRange(hand);
                newHand.Remove(card);
                List<simCard> curr = new List<simCard>();
                curr.AddRange(soFar);
                curr.Add(card);
                plays.AddRange(pickCards(curr, newHand, curr_mana));
            }
        }
        if (!pickedCard)
            plays.Add(new Play(soFar, new Dictionary<Attribute, List<simCard>>(), new Dictionary<simCard, simCard>()));
        return plays;
    }

    private List<Play> selectTargets(Play play, List<Attribute> remAttributes, List<simCard> playerCards, List<simCard> enemyCards)
    {
        List<Play> plays = new List<Play>();
        if (remAttributes.Count > 0)
        {
            if (remAttributes[0].targetType.Equals("Ally"))
            {
                if (playerCards.Count <= 0)
                {
                    List<Attribute> newAttr = new List<Attribute>();
                    newAttr.AddRange(remAttributes);
                    newAttr.Remove(remAttributes[0]);
                    plays.AddRange(selectTargets(play, newAttr, playerCards, enemyCards));
                }
                else
                {
                    foreach (simCard card in playerCards)
                    {
                        Play newPlay = new Play(play);
                        newPlay.abilitiesUsed[remAttributes[0]].Add(new simCard(card));
                        List<Attribute> newAttr = new List<Attribute>();
                        newAttr.AddRange(remAttributes);
                        if (--new Attribute(remAttributes[0]).remTargets <= 0)
                            newAttr.Remove(remAttributes[0]);
                        plays.AddRange(selectTargets(newPlay, newAttr, playerCards, enemyCards));
                    }
                }
            }
            else if(remAttributes[0].targetType.Equals("Enemy"))
            {
                if (enemyCards.Count <= 0)
                {
                    List<Attribute> newAttr = new List<Attribute>();
                    newAttr.AddRange(remAttributes);
                    newAttr.Remove(remAttributes[0]);
                    plays.AddRange(selectTargets(play, newAttr, playerCards, enemyCards));
                }
                else
                {
                    foreach (simCard card in enemyCards)
                    {
                        Play newPlay = new Play(play);
                        newPlay.abilitiesUsed[remAttributes[0]].Add(new simCard(card));
                        List<Attribute> newAttr = new List<Attribute>();
                        newAttr.AddRange(remAttributes);
                        if (--new Attribute(remAttributes[0]).remTargets <= 0)
                            newAttr.Remove(remAttributes[0]);
                        plays.AddRange(selectTargets(newPlay, newAttr, playerCards, enemyCards));
                    }
                }
            }
            else
            {
                List<Attribute> newAttr = new List<Attribute>();
                newAttr.AddRange(remAttributes);
                newAttr.Remove(remAttributes[0]);
                plays.AddRange(selectTargets(play, newAttr, playerCards, enemyCards));
            }
        }
        else
        {
            plays.Add(play);
        }
        return plays;
    }

    private List<Play> attack(Play play, List<simCard> playerBoard, List<simCard> enemyBoard)
    {
        List<Play> plays = new List<Play>();
        if(playerBoard.Count > 0)
        {
            foreach (simCard playerCard in playerBoard)
            {
                foreach (simCard enemyCard in enemyBoard)
                {
                    Play newPlay = new Play(play);
                    newPlay.attacks[new simCard(playerCard)] = new simCard(enemyCard);
                    List<simCard> newHand = new List<simCard>();
                    newHand.AddRange(playerBoard);
                    newHand.Remove(playerCard);
                    if (playerCard.damage > enemyCard.health)
                        newHand.Remove(enemyCard);
                    plays.AddRange(attack(newPlay, newHand, enemyBoard));
                }
                Play attackEnemy = new Play(play);
                attackEnemy.attacks[new simCard(playerCard)] = new simCard(true);
                List<simCard> nextHand = new List<simCard>();
                nextHand.AddRange(playerBoard);
                nextHand.Remove(playerCard);
                plays.AddRange(attack(attackEnemy, nextHand, enemyBoard));
            }
        }
        else
        {
            plays.Add(play);
        }
        return plays;
    }

    public int runSimulation(Play myPlay, BoardState bState, int depth)
    {
        int totalCalls = 0;
        int value = 0;
        BoardState currState = new BoardState(bState);
        Play play = new Play(myPlay);
        while(play.cardsPlayed.Count > 0)
        {
            simCard card = play.cardsPlayed[0];
            if (currState.playerBoardCards.Count > 5)
                break;
            play.cardsPlayed.Remove(card);
            currState.playerHandCards.Remove(card);
            currState.playerBoardCards.Add(card);
            foreach (Attribute att in card.attributes)
            {
                if (att.trigger.Equals("Battlecry") && !att.category.Equals("Taunt"))
                {
                    foreach (simCard target in play.abilitiesUsed[att])
                    {
                        if (bState.enemyBoardCards.Contains(target))
                        {
                            useAbility(att, target);
                            if (target.health <= 0)
                            {
                                currState.enemyBoardCards.Remove(target);
                                //value += 1;
                            }
                        }
                    }
                }
            }
        }
        foreach (KeyValuePair<simCard, simCard> attackPair in play.attacks)
        {
            if (bState.enemyBoardCards.Contains(attackPair.Value))
            {
                attackPair.Value.health = attackPair.Value.health - attackPair.Key.damage;
                if (attackPair.Value.isPlayer)
                {
                    currState.enemyHealth = currState.enemyHealth - attackPair.Key.damage;
                    value += 7 * attackPair.Key.damage / 10;
                }
                else
                {
                    if (attackPair.Value.health <= 0)
                    {
                        currState.enemyBoardCards.Remove(attackPair.Value);
                        //value += 1;
                    }
                    if (attackPair.Key.health <= 0)
                    {
                        currState.playerBoardCards.Remove(attackPair.Key);
                        //value -= 1;
                    }
                }
            }
        }
        if (currState.enemyHealth > 0)
        {
            bool empty = true;
            for (int i = 0; i < 9; i++)
            {
                if (currState.enemyDeckCards[i] != 0)
                    empty = false;
            }
            if (empty)
                return 0;
            int cost = currState.rnd.Next(0, 9);
            while (currState.enemyDeckCards[cost] == 0)
                cost = currState.rnd.Next(0, 9);
            GameObject drawnCard = player2.cardManager.generateCard(cost + 1);
            currState.enemyHandCards.Add(new simCard(drawnCard));
            GameObject.Destroy(drawnCard);
            currState.enemyDeckCards[cost]--;
            foreach (Play currPlay in determinePlays(currState))
            {
                totalCalls++;
                if (totalCalls > 5)
                    return value;
                value += enemySimulation(currPlay, currState, depth);
            }
        }
        else
            value += 100;
        return value;
    }

    private int enemySimulation(Play myPlay, BoardState bState, int depth)
    {
        int totalCalls = 0;
        int value = 0;
        BoardState currState = new BoardState(bState);
        Play play = new Play(myPlay);
        while (play.cardsPlayed.Count > 0)
        {
            simCard card = play.cardsPlayed[0];
            if (currState.enemyBoardCards.Count > 5)
                break;
            play.cardsPlayed.Remove(card);
            currState.enemyHandCards.Remove(card);
            currState.enemyBoardCards.Add(card);
            foreach (Attribute att in card.attributes)
            {
                if (att.trigger.Equals("Battlecry") && !att.category.Equals("Taunt"))
                {
                    foreach (simCard target in play.abilitiesUsed[att])
                    {
                        if (bState.playerBoardCards.Contains(target))
                        {
                            useAbility(att, target);
                            if (target.health <= 0)
                            {
                                currState.playerBoardCards.Remove(target);
                                //value -= 1;
                            }
                        }
                    }
                }
            }
        }
        foreach (KeyValuePair<simCard, simCard> attackPair in play.attacks)
        {
            if (bState.enemyBoardCards.Contains(attackPair.Value))
            {
                attackPair.Value.health = attackPair.Value.health - attackPair.Key.damage;
                if (attackPair.Value.isPlayer)
                {
                    currState.playerHealth = currState.playerHealth - attackPair.Key.damage;
                    value -= attackPair.Key.damage;
                }
                else
                {
                    if (attackPair.Value.health <= 0)
                    {
                        currState.playerBoardCards.Remove(attackPair.Value);
                        //value -= 1;
                    }
                    if (attackPair.Key.health <= 0)
                    {
                        currState.enemyBoardCards.Remove(attackPair.Key);
                        //value += 1;
                    }
                }
            }
        }
        if (currState.playerHealth > 0)
        {
            if(--depth <= 0)
            {
                return value;
            }
            bool empty = true;
            for (int i = 0; i < 9; i++)
            {
                if (currState.enemyDeckCards[i] != 0)
                    empty = false;
            }
            if (empty)
                return 0;
            int cost = currState.rnd.Next(0, 9);
            while (currState.playerDeckCards[cost] == 0)
                cost = currState.rnd.Next(0, 9);
            GameObject drawnCard = player2.cardManager.generateCard(cost + 1);
            currState.enemyHandCards.Add(new simCard(drawnCard));
            GameObject.Destroy(drawnCard);
            currState.playerDeckCards[cost]--;
            foreach (Play currPlay in determinePlays(currState))
            {
                totalCalls++;
                if (totalCalls > 5)
                    return value;
                value += runSimulation(currPlay, currState, depth);
            }
        }
        else
            value -= 100;
        return value;
    }

    public void useAbility(Attribute att, simCard card)
    {
        switch (att.category)
        {
            case "Damage":
                card.health = card.health - att.value;
                break;
            case "Heal":
                card.health = System.Math.Min(card.health + att.value, card.origHealth);
                break;
            case "BuffH":
                card.health = card.health + att.value;
                break;
            case "BuffA":
                card.damage = card.damage + att.value;
                break;
            case "DebuffA":
                card.damage = System.Math.Max(card.damage - att.value, 0);
                break;
            default:
                break;
        }
    }

    private List<Play> deathRattle(simCard card, BoardState bState)
    {
        List<Attribute> deathAttributes = new List<Attribute>();
        foreach (Attribute deathAtt in card.attributes)
        {
            if (deathAtt.trigger.Equals("Deathrattle"))
            {
                deathAttributes.Add(deathAtt);
            }
        }
        return selectTargets(new Play(new List<simCard>(), new Dictionary<Attribute, List<simCard>>(),
                   new Dictionary<simCard, simCard>()), deathAttributes, bState.enemyBoardCards,
                   bState.playerBoardCards);
    }
}

