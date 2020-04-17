using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellInfo : MonoBehaviour
{
    //public enum cellNumber {A1, A2, A3, B1, B2, B3, C1, C2, C3}
    //public cellNumber _cellNumberEnum;

    public int cellNumberInfo;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<SwitchCellInfo>())
        other.gameObject.GetComponent<SwitchCellInfo>()._cellNumber = this.cellNumberInfo;
    }
}
