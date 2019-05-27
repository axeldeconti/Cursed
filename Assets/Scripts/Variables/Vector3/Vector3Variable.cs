using UnityEngine;

[CreateAssetMenu(fileName = "New Vector3", menuName = "Variables/Vector3")]
public class Vector3Variable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    public Vector3 Value;

    public float X { get { return Value.x; } }
    public float Y { get { return Value.y; } }
    public float Z { get { return Value.z; } }

    public void SetValue(Vector3 value)
    {
        Value = value;
    }

    public void SetValue(Vector3Variable value)
    {
        Value = value.Value;
    }
}
