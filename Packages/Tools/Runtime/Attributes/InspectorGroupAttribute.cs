using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class InspectorGroupAttribute : PropertyAttribute
{
    public string GroupName;
    public bool GroupAllFieldsUntilNextGroupAttribute;
    public int GroupColorIndex;
    public bool ClosedByDefault;

    public InspectorGroupAttribute(string groupName, bool groupAllFieldsUntilNextGroupAttribute = false, int groupColorIndex = 24, bool closedByDefault = false)
    {
        if (groupColorIndex > 139) { groupColorIndex = 139; }

        this.GroupName = groupName;
        this.GroupAllFieldsUntilNextGroupAttribute = groupAllFieldsUntilNextGroupAttribute;
        this.GroupColorIndex = groupColorIndex;
        this.ClosedByDefault = closedByDefault;
    }
}

