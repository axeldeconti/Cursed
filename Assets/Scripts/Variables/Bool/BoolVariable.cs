using UnityEngine;


[CreateAssetMenu(fileName = "New Bool", menuName = "Variables/Bool")]
public class BoolVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    public bool Value;

    public void SetValue(bool value)
    {
        Value = value;
    }

    public void SetValue(BoolVariable value)
    {
        Value = value.Value;
    }

    public bool Equal(bool value)
    {
        if (Value == value)
            return true;
        else return false;
    }
}
