using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSprite : MonoBehaviour {

    public Texture2D zero;
    public Texture2D one;
    public Texture2D two;
    public Texture2D three;
    public Texture2D four;
    public Texture2D five;
    public Texture2D six;
    public Texture2D seven;
    public Texture2D eight;
    public Texture2D nine;
    public Texture2D background;
    public Texture2D yellow;
    public Texture2D red;
    public Dictionary<int, Texture2D> numberMap;

    public GameObject damSprite;
    public GameObject costSprite;
    public GameObject healthSprite;
    public GameObject selectSprite;

    public void setup(int cost, int damage, int health, string description)
    {
        numberMap = new Dictionary<int, Texture2D>
        {
            {0, zero }, {1, one }, {2, two }, {3, three}, {4, four }, {5, five },
            { 6, six}, {7, seven }, {8, eight }, {9, nine}

        };
        gameObject.GetComponent<SpriteRenderer>().sprite =
            Sprite.Create(background, new Rect(0, 0, background.width, background.height), new Vector2(0.5f, 0.5f));
        damSprite.GetComponent<SpriteRenderer>().sprite = 
            Sprite.Create(numberMap[damage], new Rect(0, 0, numberMap[damage].width, numberMap[damage].height), new Vector2(2.2f, 2));
        healthSprite.GetComponent<SpriteRenderer>().sprite =
            Sprite.Create(numberMap[health], new Rect(0, 0, numberMap[health].width, numberMap[health].height), new Vector2(-1.3f, 2));
        costSprite.GetComponent<SpriteRenderer>().sprite =
            Sprite.Create(numberMap[cost], new Rect(0, 0, numberMap[cost].width, numberMap[cost].height), new Vector2(2.2f, -1));
    }

    public void select()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite =
            Sprite.Create(yellow, new Rect(0, 0, yellow.width, yellow.height), new Vector2(0.5f, 0.5f));
    }

    public void ready()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite =
            Sprite.Create(red, new Rect(0, 0, red.width, red.height), new Vector2(0.5f, 0.5f));
    }

    public void unselect()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite =
            Sprite.Create(background, new Rect(0, 0, background.width, background.height), new Vector2(0.5f, 0.5f));
    }
}
