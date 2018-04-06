using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour {

    AudioSource bang;
    public string enemyTag;
    bool exploded = false;
    bool finished = false;
    private float wait = 0;

    void Start()
    {
        bang = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (exploded)
            wait += Time.deltaTime;
        if (exploded && wait > 0.3f && !finished)
        {
            ParticleSystem exp = GetComponent<ParticleSystem>();
            exp.Play();
            bang.Play();
            GetComponent<MeshRenderer>().enabled = false;
            GameObject[] targets = GameObject.FindGameObjectsWithTag(enemyTag);
            foreach (GameObject target in targets)
            {
                if (Vector3.Distance(transform.position, target.transform.position) <= 3)
                    target.GetComponent<PlayerUnit>().reduceHealth(3);
            }
            finished = true;
            Destroy(gameObject, exp.main.duration);
        }
    }

    void Explode()
    {
        exploded = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "bullet")
            Physics.IgnoreCollision(collision.collider, gameObject.GetComponent<Collider>());
        if (!exploded)
            Explode();
    }
}
