using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {
    public int cost;
    public int damage;
    public int health;
    public int origHealth;

    public GameObject ownerObject;
    public string description;
    public List<Attribute> attributes = new List<Attribute>();
    public Vector3 origPos;

    public Texture2D cardImage;

    public bool locked = false;
    public bool taunt = false;
    private float timeOverSprite = 0.0f;

    public Player owner;
    private bool held = false;
    private bool canClick = true;
    private bool instantiated = false;
    private bool tooltip = false;
    private bool selected = false;
    public bool canAttack = false;

    // Use this for initialization
    void Start()
    {
        owner = ownerObject.GetComponent<Player>();
    }
	
	// Update is called once per frame
	void Update () {
        if (instantiated)
        {     
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            bool overSprite = this.transform.parent.gameObject.GetComponent<SpriteRenderer>().bounds.Contains(mousePosition);

            if (overSprite && !locked && owner.myTurn && owner.selecting == 0)
            {
                if (Input.GetButton("Fire1") && (!owner.holdingCard || held) && canClick)
                {
                    held = true;
                    owner.holdingCard = true;
                    float xPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                    xPos = System.Math.Abs(xPos) > Screen.width / 2 ? Screen.width / 2 * (xPos / System.Math.Abs(xPos)) : xPos;

                    float yPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
                    yPos = System.Math.Abs(yPos) > Screen.height / 2 ? Screen.height / 2 * (yPos / System.Math.Abs(yPos)) : yPos;

                    GameObject cardSprite = gameObject.transform.parent.gameObject;
                    cardSprite.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    cardSprite.GetComponent<CardSprite>().damSprite.GetComponent<SpriteRenderer>().sortingOrder = 4;
                    cardSprite.GetComponent<CardSprite>().healthSprite.GetComponent<SpriteRenderer>().sortingOrder = 4;
                    cardSprite.GetComponent<CardSprite>().costSprite.GetComponent<SpriteRenderer>().sortingOrder = 4;


                    //Set the position to the mouse position
                    this.transform.parent.position = new Vector3(xPos, yPos, 0.0f);
                }
            }
            else if (overSprite && locked && owner.myTurn && owner.selecting > 0 && owner.cardSelecting != gameObject && !owner.holdingCard)
            {
                if (Input.GetMouseButton(0) && canClick)
                {
                    canClick = false;
                    if (!selected)
                    {
                        gameObject.transform.parent.gameObject.GetComponent<CardSprite>().select();
                        selected = true;
                        owner.cardsSelected.Add(gameObject);
                        owner.selecting--;
                        if (owner.selecting == 0)
                            owner.useAbility();
                        if (canAttack)
                            gameObject.transform.parent.gameObject.GetComponent<CardSprite>().ready();
                        else
                            gameObject.transform.parent.gameObject.GetComponent<CardSprite>().unselect();
                        selected = false;
                    }
                    else
                    {
                        if(canAttack)
                            gameObject.transform.parent.gameObject.GetComponent<CardSprite>().ready();
                        else
                            gameObject.transform.parent.gameObject.GetComponent<CardSprite>().unselect();
                        owner.selecting++;
                        selected = false;
                    }
                }
            }
            else if (overSprite && locked && owner.myTurn && owner.selecting == 0 && canAttack && !owner.attacking && !owner.holdingCard)
            {
                if (Input.GetMouseButton(0) && canClick)
                {
                    canClick = false;
                    owner.attacking = true;
                    transform.parent.gameObject.GetComponent<CardSprite>().select();
                    //canAttack = false;
                    owner.cardAttacking = gameObject;
                }
            }
            else if (overSprite && locked && owner.myTurn && owner.attacking && !owner.holdingCard)
            {
                if (Input.GetMouseButton(0) && canClick)
                {
                    canClick = false;
                    owner.attacking = false;
                    transform.parent.gameObject.GetComponent<CardSprite>().ready();
                    //canAttack = true;
                    owner.cardAttacking = null;
                }
            }
            else if (overSprite && locked && !owner.myTurn && owner.enemy.GetComponent<Player>().attacking 
                && !owner.enemy.GetComponent<Player>().holdingCard)
            {
                if (Input.GetMouseButton(0) && canClick)
                {
                    canClick = false;
                    owner.enemy.GetComponent<Player>().attack(owner.enemy.GetComponent<Player>().cardAttacking, gameObject);
                    owner.enemy.GetComponent<Player>().attacking = false;
                    owner.enemy.GetComponent<Player>().cardAttacking = null;
                }
            }
            else if (overSprite && locked && !owner.myTurn && owner.enemy.GetComponent<Player>().selecting > 0
                && !owner.enemy.GetComponent<Player>().holdingCard)
            {
                if (Input.GetMouseButton(0) && canClick)
                {
                    canClick = false;
                    if (!selected)
                    {
                        gameObject.transform.parent.gameObject.GetComponent<CardSprite>().select();
                        selected = true;
                        owner.enemy.GetComponent<Player>().cardsSelected.Add(gameObject);
                        owner.enemy.GetComponent<Player>().selecting--;
                        if (owner.enemy.GetComponent<Player>().selecting == 0)
                            owner.enemy.GetComponent<Player>().useAbility();
                        gameObject.transform.parent.gameObject.GetComponent<CardSprite>().unselect();
                        selected = false;
                    }
                    else
                    {
                        gameObject.transform.parent.gameObject.GetComponent<CardSprite>().unselect();
                        owner.enemy.GetComponent<Player>().selecting++;
                        selected = false;
                    }
                }
            }
            if (Input.GetButtonUp("Fire1"))
            {
                canClick = true;
                if (held)
                {
                    held = false;
                    owner.holdingCard = false;

                    GameObject cardSprite = gameObject.transform.parent.gameObject;
                    cardSprite.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    cardSprite.GetComponent<CardSprite>().damSprite.GetComponent<SpriteRenderer>().sortingOrder = 2;
                    cardSprite.GetComponent<CardSprite>().healthSprite.GetComponent<SpriteRenderer>().sortingOrder = 2;
                    cardSprite.GetComponent<CardSprite>().costSprite.GetComponent<SpriteRenderer>().sortingOrder = 2;
                }
            }
            if (!held && !locked && owner.myTurn && owner.selecting == 0)
            {
                if (this.transform.parent.position.y > -3 && owner.cardsOnBoard.Count < 5)
                    owner.playCard(gameObject);
                else
                    this.transform.parent.position = origPos;
            }
        }
    }


    private void OnGUI()
    {
        if (instantiated)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            bool overSprite = this.transform.parent.gameObject.GetComponent<SpriteRenderer>().bounds.Contains(mousePosition);

            if (overSprite && !held)
            {
                timeOverSprite += Time.deltaTime;
                if ((!owner.tooltip || tooltip)  && timeOverSprite > 1.0f)
                {
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.black;
                    style.fontSize = 50;
                    GUI.Label(new Rect(Screen.width/4, 2*Screen.height/3, 100, 400), description, style);
                    owner.tooltip = true;
                }
            }
            else
            {
                timeOverSprite = 0.0f;
                owner.tooltip = false;
            }
        }
    }

    public void instantiate()
    {
        instantiated = true;
    }

    public void updateStats()
    {
        if (health <= 0)
        {
            owner.cardsOnBoard.Remove(gameObject);
            owner.openSpots.Add(gameObject.transform.parent.position.x);
            foreach (Attribute att in attributes)
            {
                if (att.trigger.Equals("Deathrattle"))
                {
                    owner.currAtt = att;
                    owner.cardSelecting = gameObject;
                    if (owner.currAtt.targetType.Equals("Ally"))
                        owner.selecting = System.Math.Min(att.numTargets, owner.cardsOnBoard.Count);
                    else
                        owner.selecting =
                            System.Math.Min(att.numTargets, owner.enemy.GetComponent<Player>().cardsOnBoard.Count);
                    if (owner.isAI)
                    {
                        ownerObject.GetComponent<PlayerAI>().selectTargets();
                    }
                }
            }
            GameObject.Destroy(gameObject.transform.parent.gameObject);
        }
        else
        {
            gameObject.transform.parent.gameObject.GetComponent<CardSprite>().setup(cost, damage, health, description);
            if (canAttack)
                gameObject.transform.parent.gameObject.GetComponent<CardSprite>().ready();
        }
    }
    
    public simCard copyCard()
    {
        return new simCard(this);
    }

}

public class simCard
{
    public static int uniqueNum = 0;
    public int cardID;
    public int cost;
    public int damage;
    public int health;
    public int origHealth;
    public bool isPlayer;
    public List<Attribute> attributes;

    public simCard(bool _isPlayer)
    {
        isPlayer = _isPlayer;
        cardID = uniqueNum++;
        cost = 0;
        damage = 0;
        health = 20;
        origHealth = 20;
        attributes = new List<Attribute>();
    }

    public simCard(Card card)
    {
        isPlayer = false;
        cardID = uniqueNum++;
        cost = card.cost;
        damage = card.damage;
        health = card.health;
        origHealth = card.origHealth;
        attributes = card.attributes;
    }

    public simCard(simCard card)
    {
        isPlayer = card.isPlayer;
        cardID = card.cardID;
        cost = card.cost;
        damage = card.damage;
        health = card.health;
        origHealth = card.origHealth;
        attributes = card.attributes;
    }

    public override bool Equals(object obj)
    {
        simCard card = obj as simCard;
        if (card == null)
            return false;
        else
            return cardID.Equals(card.cardID);
    }

    public override int GetHashCode()
    {
        return this.cardID.GetHashCode();
    }
}
