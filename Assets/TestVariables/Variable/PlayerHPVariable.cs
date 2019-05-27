using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHPVariable : MonoBehaviour
{
    public FloatReference maxHP;
    public FloatVariable hp;

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
