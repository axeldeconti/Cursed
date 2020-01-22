using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDebugger : MonoBehaviour
{
    public int intValue = 0;
    public float floatValue = 0;
    public bool boolValue = false;
    public Vector3 vector3value;
    public TestEnum enumValue = TestEnum.Test_1;

    private void Start()
    {
        CursedDebugger.Instance.Add("int", () => intValue.ToString());
        CursedDebugger.Instance.Add("float", () => floatValue.ToString(), Color.blue);
        CursedDebugger.Instance.Add("bool", () => boolValue.ToString());
        CursedDebugger.Instance.Add("vector3", () => vector3value.ToString(), Color.green);
        CursedDebugger.Instance.Add("enum", () => enumValue.ToString());

        CursedDebugger.Instance.Add("Test", () => intValue.ToString() + " !! " + floatValue.ToString());
    }
}

public enum TestEnum { Test_1, Test_2, Test_3}