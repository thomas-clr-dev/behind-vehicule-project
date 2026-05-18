using System;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class EnumConditionAttribute : PropertyAttribute
{
    public string ConditionEnum = "";
    public bool Hidden = false;

    BitArray bitArray = new BitArray(32);
    public bool ContainsBitFlag(int enumValue)
    {
        return bitArray.Get(enumValue);
    }

    public EnumConditionAttribute(string conditionBoolean, params int[] enumValues)
    {
        this.ConditionEnum = conditionBoolean;
        this.Hidden = true;

        for (int i = 0; i < enumValues.Length; i++)
        {
            bitArray.Set(enumValues[i], true);
        }
    }
}
