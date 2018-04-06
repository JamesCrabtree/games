using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attribute { 

    public int cost;
    public string category;
    public string trigger;
    public int numTargets;
    public int value;
    public string targetType;
    public int remTargets;

    public string description;

    public Attribute(string _trigger, int _numTargets, string _category, int _value, int _cost, string _target)
    {
        trigger = _trigger;
        numTargets = _numTargets;
        remTargets = numTargets;
        value = _value;
        cost = _cost;
        category = _category;
        targetType = _target;
        description = setDescription();
    }

    public Attribute(Attribute copyAttribute)
    {
        trigger = copyAttribute.trigger;
        numTargets = copyAttribute.numTargets;
        value = copyAttribute.value;
        cost = copyAttribute.cost;
        category = copyAttribute.category;
        targetType = copyAttribute.targetType;
        description = setDescription();
    }

    public string setDescription()
    {
        description = trigger + ": ";

        switch (category)
        {
            case "Taunt":
                description += "Taunt\n";
                break;
            case "Damage":
                description += "Deal " + value + " damage to " + numTargets + " enemy unit(s)\n";
                break;
            case "Heal":
                description += "Heal " + value + " health to " + numTargets + " friendly unit(s)\n";
                break;
            case "BuffH":
                description += "Increase health of " + numTargets + " friendly unit(s) by " + value + "\n";
                break;
            case "BuffA":
                description += "Increase attack of " + numTargets + " friendly unit(s) by " + value + "\n";
                break;
            case "DebuffA":
                description += "Decrease attack of " + numTargets + " enemy unit(s) by " + value + "\n";
                break;
            default:
                description += "\n";
                break;
        }
        return description;
    }
}