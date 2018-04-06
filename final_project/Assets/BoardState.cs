using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardState{
    public System.Random rnd = new System.Random();
    private Player player;
    public int playerHealth;
    public int enemyHealth;
    public List<simCard> playerBoardCards = new List<simCard>();
    public List<simCard> enemyBoardCards = new List<simCard>();
    public List<simCard> playerHandCards = new List<simCard>();
    public List<simCard> enemyHandCards = new List<simCard>();
    public int[] playerDeckCards = { 2, 3, 4, 5, 3, 3, 2, 1, 1 };
    public int[] enemyDeckCards = { 2, 3, 4, 5, 3, 3, 2, 1, 1 };

    public BoardState(Player player, List<GameObject> pbc, List<GameObject> ebc, List<GameObject> phc)
    {
        playerBoardCards = new List<simCard>();
        enemyBoardCards = new List<simCard>();
        playerHandCards = new List<simCard>();
        enemyHandCards = new List<simCard>();
        foreach (GameObject card in pbc)
            playerBoardCards.Add(card.GetComponent<Card>().copyCard());
        foreach (GameObject card in ebc)
            enemyBoardCards.Add(card.GetComponent<Card>().copyCard());
        foreach (GameObject card in phc)
            playerHandCards.Add(card.GetComponent<Card>().copyCard());
        for (int i = 0; i < 5; i++)
        {
            int cost = rnd.Next(0,9);
            while (enemyDeckCards[cost] == 0)
                cost = rnd.Next(0,9);
            enemyHandCards.Add(player.cardManager.generateCard(cost).GetComponent<Card>().copyCard());
            enemyDeckCards[cost]--;
        }
        foreach(simCard card in playerHandCards)
        {
            playerDeckCards[card.cost-1]--;
        }
        playerHealth = 20;
        enemyHealth = 20;
    }

    public void update(Player player, List<GameObject> pbc, List<GameObject> ebc, List<GameObject> phc,
        int _playerHealth, int _enemyHealth)
    {
        playerBoardCards = new List<simCard>();
        enemyBoardCards = new List<simCard>();
        playerHandCards = new List<simCard>();
        foreach (GameObject card in pbc)
            playerBoardCards.Add(card.GetComponent<Card>().copyCard());
        foreach (GameObject card in ebc)
            enemyBoardCards.Add(card.GetComponent<Card>().copyCard());
        foreach (GameObject card in phc)
            playerHandCards.Add(card.GetComponent<Card>().copyCard());
        int cost = rnd.Next(0, 9);
        while (enemyDeckCards[cost] == 0)
            cost = rnd.Next(0, 9);
        enemyHandCards.Add(player.cardManager.generateCard(cost).GetComponent<Card>().copyCard());
        enemyDeckCards[cost]--;
        playerHealth = _playerHealth;
        enemyHealth = _enemyHealth;
    }

    public BoardState(BoardState board)
    {
        playerBoardCards = new List<simCard>();
        enemyBoardCards = new List<simCard>();
        playerHandCards = new List<simCard>();
        enemyHandCards = new List<simCard>();
        player = board.player;
        foreach (simCard card in board.playerBoardCards)
            playerBoardCards.Add(new simCard(card));
        foreach (simCard card in board.enemyBoardCards)
            enemyBoardCards.Add(new simCard(card));
        foreach (simCard card in board.playerHandCards)
            playerHandCards.Add(card);
        foreach (simCard card in board.playerBoardCards)
            enemyHandCards.Add(card);
        playerHealth = board.playerHealth;
        enemyHealth = board.enemyHealth;
        for (int i = 0; i < 9; i++)
        {
            playerDeckCards[i] = board.enemyDeckCards[i];
            enemyDeckCards[i] = board.enemyDeckCards[i];
        }
    }
}
