using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHPVariable : MonoBehaviour
{
    public FloatReference maxHP;
    public FloatVariable hp;
    public IntVariable playerLevel;

    private void Start()
    {
        hp.SetValue(maxHP);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            hp.ApplyChange(-10);
        }
    }
}
