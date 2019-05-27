using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public float maxHp = 100, hp = 100;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            hp -= 10;
            UIManager.instance.SetHealthSliderValue(hp / maxHp);
        }
            
    }
}
