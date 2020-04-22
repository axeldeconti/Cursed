using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cursed.UI;

public class CellCameraDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<CellInfo>())
        {
            CellInfo cellTriggered = other.GetComponent<CellInfo>();
            cellTriggered._playerOnThisCell = true;

            foreach(MapCellUI cell in FindObjectsOfType<MapCellUI>())
            {
                if (cellTriggered == cell._myCell)
                    cell.PlayerOnMyCell();
            }
        }
    }
}
