using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestAxel))]
public class TestAxel_Editor : Editor
{
    private TestAxel _testAxel = null;

    private void OnEnable()
    {
        _testAxel = (TestAxel)target;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Test"))
            _testAxel.Test();

        base.OnInspectorGUI();
    }
}
