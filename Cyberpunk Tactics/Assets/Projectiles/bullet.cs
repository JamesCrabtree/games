using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour {

    public string friendTag;
    public string enemyTag;

	void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "bullet" || collision.gameObject.tag == friendTag) Physics.IgnoreCollision(collision.collider, gameObject.GetComponent<Collider>());
        if(collision.gameObject.tag == enemyTag) collision.gameObject.GetComponent<PlayerUnit>().reduceHealth(1);
        Destroy(gameObject);
    }
}
