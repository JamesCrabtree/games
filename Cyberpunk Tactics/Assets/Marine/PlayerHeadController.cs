using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadController : Controller {

    GameObject parentObj;

    void Start()
    {
        isUnit = true;
        isAttachment = true;
        parentObj = gameObject.transform.parent.gameObject;
        usingAbilityOne = false;
    }

    public override GameObject getMasterObject()
    {
        return parentObj;
    }

    public override void select()
    {
        parentObj.GetComponent<PlayerController>().select();
    }

    public override void unselect()
    {
        parentObj.GetComponent<PlayerController>().unselect();
    }

    public override void abilityOne()
    {
        parentObj.GetComponent<PlayerController>().abilityOne();
    }

    public override void abilityOneSelect(Vector3 target)
    {
        parentObj.GetComponent<PlayerController>().abilityOneSelect(target);
    }
}
