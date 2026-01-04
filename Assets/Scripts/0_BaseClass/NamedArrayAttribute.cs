using UnityEngine;

public class NamedArrayAttribute : PropertyAttribute
{
    public System.Type TargetEnum;
    public string[] Names;

    public NamedArrayAttribute(System.Type targetEnum)
    {
        TargetEnum = targetEnum;
        Names = System.Enum.GetNames(targetEnum);
    }

    public NamedArrayAttribute(string[] names)
    {
        Names = names;
    }
}
