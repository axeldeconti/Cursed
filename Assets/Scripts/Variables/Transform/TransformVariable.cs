using UnityEngine;


[CreateAssetMenu(fileName = "New Transform", menuName = "Variables/Transform")]
public class TransformVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    public Transform Value;

    public Vector3 Position { get { return Value.position; } }
    public Vector3 LocalPosition { get { return Value.localPosition; } }
    public Quaternion Rotation { get { return Value.rotation; } }
    public Quaternion LocalRotation { get { return Value.localRotation; } }
    public Vector3 Scale { get { return Value.localScale; } }

    public void SetValue(Transform value)
    {
        Value = value;
    }

    public void SetValue(TransformVariable value)
    {
        Value = value.Value;
    }
}
