using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public int startingCards;
    public int remMana = 1;
    public int totalMana = 1;
    public int health;
    public float yHand;
    public float yBoard;
    public float yHealth;
    public float yMana;
    public bool myTurn;
    public bool isAI;

    public CardManager cardManager;
    public GameObject cardPrefab;
    public GameObject cardImage;
    public GameObject enemy;

    public bool tooltip = false;
    public bool hasTaunt = false;
    public bool attacking = false;
    public int selecting = 0;
    public bool holdingCard = false;

    public Stack<GameObject> cardsInDeck;
    public List<GameObject> cardsInHand = new List<GameObject>();
    public List<float> openSpots = new List<float> { -5, -2.5f, 0.0f, 2.5f, 5 };
    public List<GameObject> cardsOnBoard;
    public Attribute currAtt;
    public GameObject cardSelecting;
    public List<GameObject> cardsSelected = new List<GameObject>();
    public GameObject cardAttacking;

    private void Start()
    {
        cardPrefab.GetComponent<Card>().ownerObject = gameObject;
        cardManager = ScriptableObject.CreateInstance("CardManager") as CardManager;
        cardManager.cardPrefab = cardPrefab;
        cardsInDeck = cardManager.generateDeck();
        for (int i = 0; i < startingCards; i++)
        {
            drawCard();
        }
        if (isAI)
            gameObject.GetComponent<PlayerAI>().setup();
        gameObject.transform.position = new Vector3(-8, yHealth, 0);
        gameObject.GetComponent<HealthSprite>().setup();
        gameObject.GetComponent<HealthSprite>().setHealth(health);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void playCard(GameObject cardObject)
    {
        if (cardObject.GetComponent<Card>().cost <= remMana)
        {
            remMana -= cardObject.GetComponent<Card>().cost;
            cardsInHand.Remove(cardObject);
            openSpots.Sort();

            float mySpot = openSpots[openSpots.Count / 2];
            openSpots.Remove(mySpot);
            foreach(GameObject card in cardsOnBoard)
            {
                if (card.transform.parent.position.x == mySpot)
                {
                    mySpot = openSpots[openSpots.Count / 2];
                    openSpots.Remove(mySpot);
                }

            }
            cardObject.transform.parent.position = new Vector3(mySpot, yBoard, 0);
            cardsOnBoard.Add(cardObject);
            foreach (GameObject card in cardsInHand)
            {
                if (card.gameObject.transform.parent.position.x < cardObject.GetComponent<Card>().origPos.x)
                    card.transform.parent.position = new Vector3(card.gameObject.transform.parent.position.x + 1.2f, card.gameObject.transform.parent.position.y, 0);
                if (card.gameObject.transform.parent.position.x > cardObject.GetComponent<Card>().origPos.x)
                    card.transform.parent.position = new Vector3(card.gameObject.transform.parent.position.x - 1.2f, card.gameObject.transform.parent.position.y, 0);
                card.GetComponent<Card>().origPos = card.transform.parent.position;

            }
            cardObject.GetComponent<Card>().locked = true;
            foreach(Attribute att in cardObject.GetComponent<Card>().attributes)
            {
                if (att.trigger.Equals("Battlecry"))
                {
                    if (att.category.Equals("Taunt"))
                    {
                        hasTaunt = true;
                        cardObject.GetComponent<Card>().taunt = true;
                    }
                    else
                    {
                        currAtt = att;
                        cardSelecting = cardObject;
                        if (currAtt.targetType.Equals("Ally"))
                            selecting = System.Math.Min(att.numTargets, cardsOnBoard.Count - 1);
                        else
                            selecting = System.Math.Min(att.numTargets, enemy.GetComponent<Player>().cardsOnBoard.Count);
                    }
                }
            }
        }
        else
            cardObject.transform.parent.position = cardObject.GetComponent<Card>().origPos;

    }

    public void drawCard()
    {
        GameObject cardObject = cardsInDeck.Pop();
        Card card = cardObject.GetComponent<Card>();
        card.instantiate();
        cardObject.transform.parent = Instantiate(cardImage, handPosition(), Quaternion.identity).transform;
        cardObject.transform.parent.gameObject.GetComponent<CardSprite>().setup(card.cost, card.damage, card.health, card.description);
        card.origPos = cardObject.transform.parent.position;
        cardsInHand.Add(cardObject);
    }

    public void startTurn()
    {
        if (!myTurn)
        {
            if (totalMana < 9)
                totalMana++;
            remMana = totalMana;
            drawCard();
            foreach (GameObject playedCard in cardsOnBoard)
            {
                playedCard.GetComponent<Card>().canAttack = true;
                playedCard.transform.parent.GetComponent<CardSprite>().ready();
            }
            myTurn = true;
            if (isAI)
            {
                int totalAttack = 0;
                foreach(GameObject card in cardsOnBoard)
                {
                    totalAttack += card.GetComponent<Card>().damage;
                }
                if (totalAttack > enemy.GetComponent<Player>().health)
                {
                    foreach (GameObject card in cardsOnBoard)
                        attack(card, enemy);
                }
                else
                    gameObject.GetComponent<PlayerAI>().calculateTurn();
            }
        }
    }

    public void endTurn()
    {
        if (myTurn)
        {
            for (int i = 0; i < cardsOnBoard.Count; i++)
            {
                GameObject card = cardsOnBoard[i];
                if (card.GetComponent<Card>().canAttack)
                {
                    attack(card, enemy);
                    if (!card)
                        i--;
                }
            }
            if (enemy.GetComponent<Player>().health > 0)
            {
                selecting = 0;
                myTurn = false;
                enemy.GetComponent<Player>().startTurn();
            }
        }
    }

    private Vector3 handPosition()
    {
        Vector3 pos = new Vector3(0, yHand, 0);
        foreach (GameObject cardObj in cardsInHand)
        {
            pos = new Vector3(cardObj.transform.parent.position.x + 1.2f, cardObj.transform.parent.position.y, 0);
            cardObj.transform.parent.position = new Vector3(cardObj.transform.parent.position.x - 1.2f,
                                                            cardObj.transform.parent.position.y, 0);
            cardObj.GetComponent<Card>().origPos = cardObj.transform.parent.position;
        }
        return pos;
    }

    private void OnGUI()
    {
        GUIStyle style1 = new GUIStyle();
        style1.normal.textColor = Color.black;
        style1.fontSize = 20;

        GUIStyle style2 = new GUIStyle();
        style2.fontSize = 50;
        style2.normal.textColor = Color.red;

        GUI.Label(new Rect(9.1f*Screen.width/10, yMana*Screen.height, 100, 400), "MANA: " + remMana + "/" + totalMana, style1);
        if(selecting > 0)
        {
            GUI.Label(new Rect(Screen.width/4,  Screen.height/2 - 1, 100, 400), currAtt.description, style2);
        }
    }

    public void useAbility()
    {
        for(int i = 0; i < cardsSelected.Count; i++)
        {
            GameObject card = cardsSelected[i];
            switch (currAtt.category)
            {
                case "Damage":
                    card.GetComponent<Card>().health = card.GetComponent<Card>().health - currAtt.value;
                    break;
                case "Heal":
                    card.GetComponent<Card>().health = 
                        System.Math.Min(card.GetComponent<Card>().health + currAtt.value, card.GetComponent<Card>().origHealth);
                    break;
                case "BuffH":
                    card.GetComponent<Card>().health = card.GetComponent<Card>().health + currAtt.value;
                    break;
                case "BuffA":
                    card.GetComponent<Card>().damage = card.GetComponent<Card>().damage + currAtt.value;
                    break;
                case "DebuffA":
                    card.GetComponent<Card>().damage =
                        System.Math.Max(card.GetComponent<Card>().damage - currAtt.value,0);
                    break;
                default:
                    break;
            }
            card.GetComponent<Card>().updateStats();
            if (!card)
                i--;
        }
        cardsSelected.Clear();
    }

    public void setHealth(int _health)
    {
        if (isAI && _health <= 0)
            SceneManager.LoadScene("Win Screen");
        else if (_health <= 0)
            SceneManager.LoadScene("Lose Screen");
        else
        {
            health = _health;
            gameObject.GetComponent<HealthSprite>().setHealth(health);
        }
    }

    public void attack(GameObject attacker, GameObject target)
    {
        int enemyTauntCount = 0;
        int playerTauntCount = 0;
        bool isPlayer = false;
        attacker.GetComponent<Card>().canAttack = false;
        if (target == enemy)
            isPlayer = true;
        foreach(GameObject card in enemy.GetComponent<Player>().cardsOnBoard)
        {
            if (card.GetComponent<Card>().taunt)
            {
                target = card;
                enemyTauntCount++;
                isPlayer = false;
            }
        }
        foreach (GameObject card in cardsOnBoard)
        {
            if (card.GetComponent<Card>().taunt)
                playerTauntCount++;
        }
        if (isPlayer)
        {
            enemy.GetComponent<Player>().setHealth(enemy.GetComponent<Player>().health - attacker.GetComponent<Card>().damage);
            attacker.GetComponent<Card>().updateStats();
            return;
        }
        if (isAI)
        {
            attacker.GetComponent<Card>().health = attacker.GetComponent<Card>().health - target.GetComponent<Card>().damage;
            if (attacker.GetComponent<Card>().health <= 0)
            {
                if (attacker.GetComponent<Card>().taunt)
                {
                    if (playerTauntCount <= 1)
                        hasTaunt = false;
                }
            }
        }
        target.GetComponent<Card>().health = target.GetComponent<Card>().health - attacker.GetComponent<Card>().damage;
        if (target.GetComponent<Card>().health <= 0)
        {
            if (target.GetComponent<Card>().taunt)
            {
                if(enemyTauntCount <= 1)
                    enemy.GetComponent<Player>().hasTaunt = false;
            }
        }
        if (!isAI)
        {
            attacker.GetComponent<Card>().health = attacker.GetComponent<Card>().health - target.GetComponent<Card>().damage;
            if (attacker.GetComponent<Card>().health <= 0)
            {
                if (attacker.GetComponent<Card>().taunt)
                {
                    if (playerTauntCount <= 1)
                        hasTaunt = false;
                }
            }
        }
        attacker.transform.parent.gameObject.GetComponent<CardSprite>().unselect();
        target.GetComponent<Card>().updateStats();
        attacker.GetComponent<Card>().updateStats();
        attacking = false;
    }

}
