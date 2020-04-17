using UnityEngine;
using TMPro;
using System.Collections;

public class SwitchCellInfo : MonoBehaviour
{
    private TextMeshProUGUI _cellInfoText;

    [SerializeField] public int _cellNumber;
    
    // Start is called before the first frame update
    void Start()
    {
        _cellInfoText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCellInformation();
    }

    public void UpdateCellInformation()
    {
        switch (_cellNumber)
        {
            case 9:
                print("C3");
                break;
            case 8:
                print("C2");
                break;
            case 7:
                print("C1");
                break;
            case 6:
                print("B3");
                break;
            case 5:
                print("B2");
                break;
            case 4:
                print("B1");
                break;
            case 3:
                print("A3");
                break;
            case 2:
                print("A2");
                break;
            case 1:
                print("A1");
                break;
            default:
                break;
        }
    }
}
