using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSprite : MonoBehaviour {

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
    public Dictionary<int, Texture2D> numberMap;

    public GameObject tensSprite;
    public GameObject onesSprite;

    public void setup()
    {
        numberMap = new Dictionary<int, Texture2D>
        {
            {0, zero }, {1, one }, {2, two }, {3, three}, {4, four }, {5, five },
            { 6, six}, {7, seven }, {8, eight }, {9, nine}

        };
    }

    public void setHealth(int health)
    {

        tensSprite.GetComponent<SpriteRenderer>().sprite =
            Sprite.Create(numberMap[health/10], new Rect(0,0,
            numberMap[health/10].width, numberMap[health/10].height), new Vector2(0.5f, 0.5f));
        onesSprite.GetComponent<SpriteRenderer>().sprite =
            Sprite.Create(numberMap[health%10], new Rect(0,0,
            numberMap[health % 10].width, numberMap[health % 10].height), new Vector2(-0.5f, 0.5f));
    }
}
