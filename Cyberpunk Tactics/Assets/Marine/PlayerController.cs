using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
    public GameObject selectionCirclePrefab;
    private GameObject selectionCircle;
    public GameObject UIPrefab;
    private GameObject UI;

    private void Start()
    {
        isUnit = true;
        isAttachment = false;
        abilityOneRequiresSelect = true;
        usingAbilityOne = false;
        selectionCircle = null;
        UI = null;
    }

    public override void select()
    {
        if(selectionCircle == null)
        {
            selectionCircle = Instantiate(selectionCirclePrefab);
            selectionCircle.transform.SetParent(transform, false);
            selectionCircle.transform.eulerAngles = new Vector3(90, 0, 0);
        }
    }

    public override void unselect()
    {
        if(selectionCircle != null)
        {
            Destroy(selectionCircle.gameObject);
            selectionCircle = null;
        }
    }

    public override void abilityOne()
    {
        //if(gameObject.GetComponent<PlayerUnit>().timerRocket > gameObject.GetComponent<PlayerUnit>().rocketCooldown)
            gameObject.GetComponent<PlayerUnit>().firingRocket = true;
    }

    public override void displayUI()
    {
        if(UI == null)
        {
            UI = Instantiate(UIPrefab);
            UI.transform.SetParent(GameObject.Find("UI").transform);
        }
    }

    public override void destroyUI()
    {
        if(UI != null)
        {
            Destroy(UI.gameObject);
            UI = null;
        }
    }

    public override void abilityOneSelect(Vector3 target)
    {
        gameObject.GetComponent<PlayerUnit>().fireRocket(target);
    }

    private void OnGUI()
    {
        if (gameObject.GetComponent<PlayerUnit>().firingRocket)
        {

        }
    }
}
