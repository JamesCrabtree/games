using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public bool isUnit;
    public bool isAttachment;

    public bool abilityOneRequiresSelect;
    public bool usingAbilityOne;

    public virtual GameObject getMasterObject()
    {
        return gameObject;
    }

	public virtual void select()
    {
        
    }

    public virtual void unselect()
    {
        
    }

    public virtual void displayUI()
    {

    }

    public virtual void destroyUI()
    {

    }

    public virtual void abilityOne()
    {

    }

    public virtual void abilityOneSelect(Transform target)
    {

    }

    public virtual void abilityOneSelect(Vector3 target)
    {

    }
}
