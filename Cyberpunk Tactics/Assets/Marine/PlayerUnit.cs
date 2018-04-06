using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerUnit : MonoBehaviour
{
    public int health;
    public float range;
    public GameObject Bullet;
    public GameObject Grenade;
    public Transform spawn;
    private GameObject bullet;

    public float timer = 0;
    public float timerRocket = 1;
    public float wait = 1;
    public float rocketCooldown = 0.2f;

    public bool isSelected;
    public bool firingRocket;

    AudioSource myAudio;

    private void Start()
    {
        myAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        timerRocket += Time.deltaTime;

        if (firingRocket)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                transform.GetChild(2).GetComponent<LineRenderer>().enabled = true;
                transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
                Vector3 velocity = calculateBestThrowSpeed(transform.GetChild(2).position, hit.point, 3);
                transform.GetChild(2).GetComponent<LaunchArcRenderer>().simulatePath(velocity);
            }
        }
    }

    public void attack()
    {
        if (timer > wait)
        {
            if (bullet)
                Destroy(bullet);
            bullet = Instantiate(Bullet, spawn.position, transform.rotation);
            bullet.GetComponent<Rigidbody>().velocity = (bullet.transform.forward * 75);
            myAudio.Play();
            timer = 0.0f;
        }
    }

    public void fireRocket(Vector3 endLocation)
    {
        //if (timerRocket > rocketCooldown)
        //{
            Quaternion rotation = transform.rotation;
            transform.LookAt(new Vector3(endLocation.x, transform.position.y, endLocation.z));
            bullet = Instantiate(Grenade, spawn.position, transform.rotation);
            bullet.GetComponent<Rigidbody>().velocity = calculateBestThrowSpeed(transform.GetChild(2).position, endLocation, 3);
            transform.rotation = rotation;
            myAudio.Play();
            firingRocket = false;
            transform.GetChild(2).GetComponent<LineRenderer>().enabled = false;
            timerRocket = 0.0f;
        //}
    }


    public void reduceHealth(int val)
    {
        health -= val;
        if (gameObject && health <= 0)
            die();
    }

    private Vector3 calculateBestThrowSpeed(Vector3 origin, Vector3 target, float timeToTarget)
    {
        Vector3 toTarget = target - origin;
        Vector3 toTargetXZ = toTarget;
        toTargetXZ.y = 0;

        float y = toTarget.y;
        float xz = toTargetXZ.magnitude;

        float t = timeToTarget;
        float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
        float v0xz = xz / t;

        Vector3 result = toTargetXZ.normalized;
        result *= v0xz;
        result.y = v0y;

        return result;
    }

    private void die()
    {
        Destroy(gameObject);
    }
}