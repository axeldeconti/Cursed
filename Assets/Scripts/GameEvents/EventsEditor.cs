using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VoidEvent))]
public class VoidEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = Application.isPlaying;

        VoidEvent e = target as VoidEvent;
        if (GUILayout.Button("Raise"))
            e.Raise(new Void());
    }
}

[CustomEditor(typeof(IntEvent))]
public class IntEventEditor : Editor
{
    public int value;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = Application.isPlaying;

        IntEvent e = target as IntEvent;
        if (GUILayout.Button("Raise"))
            e.Raise(value);
    }
}

[CustomEditor(typeof(FloatEvent))]
public class FloatEventEditor : Editor
{
    public float value;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = Application.isPlaying;

        FloatEvent e = target as FloatEvent;
        if (GUILayout.Button("Raise"))
            e.Raise(value);
    }
}

[CustomEditor(typeof(Vector3Event))]
public class Vector3EventEditor : Editor
{
    public Vector3 value;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = Application.isPlaying;

        Vector3Event e = target as Vector3Event;
        if (GUILayout.Button("Raise"))
            e.Raise(value);
    }
}

[CustomEditor(typeof(TransformEvent))]
public class TransformEventEditor : Editor
{
    public Transform value;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = Application.isPlaying;

        TransformEvent e = target as TransformEvent;
        if (GUILayout.Button("Raise"))
            e.Raise(value);
    }
}
