using UnityEngine;
using TMPro;
using System.Collections;

public class SwitchCellInfo : MonoBehaviour
{
    public TextMeshProUGUI cellInfoText;

    [SerializeField] private int _cellNumber;
    
    public void UpdateCellInformation()
    {
        switch (_cellNumber)
        {
            case 9:
                cellInfoText.text = "C3";
                break;
            case 8:
                cellInfoText.text = "B3";
                break;
            case 7:
                cellInfoText.text = "A3";
                break;
            case 6:
                cellInfoText.text = "C2";
                break;
            case 5:
                cellInfoText.text = "B2";
                break;
            case 4:
                cellInfoText.text = "A2";
                break;
            case 3:
                cellInfoText.text = "C1";
                break;
            case 2:
                cellInfoText.text = "B1";
                break;
            case 1:
                cellInfoText.text = "A1";
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CellInfo>())
        {
            _cellNumber = other.gameObject.GetComponent<CellInfo>().cellNumberInfo;
        }
    }
}
