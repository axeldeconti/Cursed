using UnityEngine;
using Cursed.Managers;
using UnityEngine.UI;

public class InputImageUpdate : MonoBehaviour
{
    public enum InputType { FaceDown, FaceRight, FaceUp, FaceLeft, LeftBumper, RightBumper, LeftTrigger, RightTrigger, Start, Select}

    [SerializeField] private InputType _inputType;
    private ControlerManager _controlerManager;

    private Image _image;

    private void Awake()
    {
        _controlerManager = ControlerManager.Instance;
        _controlerManager._onControlerChanged += CheckControlerType;
        _image = GetComponent<Image>();

        CheckControlerType(_controlerManager._ControlerType);
    }

    private void CheckControlerType(ControlerManager.ControlerType type)
    {
        switch(type)
        {
            case ControlerManager.ControlerType.XBOX:
                SetToXboxInput();
                break;

            case ControlerManager.ControlerType.PS4:
                SetToPlaystationInput();
                break;
        }
    }

    private void SetToXboxInput()
    {
        switch (_inputType)
        {
            case InputType.FaceDown:
                _image.sprite = _controlerManager._buttonAXbox;
                break;
            case InputType.FaceRight:
                _image.sprite = _controlerManager._buttonBXbox;
                break;
            case InputType.FaceUp:
                _image.sprite = _controlerManager._buttonYXbox;
                break;
            case InputType.FaceLeft:
                _image.sprite = _controlerManager._buttonXXbox;
                break;
            case InputType.LeftBumper:
                _image.sprite = _controlerManager._buttonLBXbox;
                break;
            case InputType.RightBumper:
                _image.sprite = _controlerManager._buttonRBXbox;
                break;
            case InputType.LeftTrigger:
                _image.sprite = _controlerManager._buttonLTXbox;
                break;
            case InputType.RightTrigger:
                _image.sprite = _controlerManager._buttonRTXbox;
                break;
            case InputType.Start:
                _image.sprite = _controlerManager._buttonStartXbox;
                break;
            case InputType.Select:
                _image.sprite = _controlerManager._buttonSelectXbox;
                break;
            default:
                break;
        }
    }

    private void SetToPlaystationInput()
    {
        switch (_inputType)
        {
            case InputType.FaceDown:
                _image.sprite = _controlerManager._buttonAPlaystation;
                break;
            case InputType.FaceRight:
                _image.sprite = _controlerManager._buttonBPlaystation;
                break;
            case InputType.FaceUp:
                _image.sprite = _controlerManager._buttonYPlaystation;
                break;
            case InputType.FaceLeft:
                _image.sprite = _controlerManager._buttonXPlaystation;
                break;
            case InputType.LeftBumper:
                _image.sprite = _controlerManager._buttonLBPlaystation;
                break;
            case InputType.RightBumper:
                _image.sprite = _controlerManager._buttonRBPlaystation;
                break;
            case InputType.LeftTrigger:
                _image.sprite = _controlerManager._buttonLTPlaystation;
                break;
            case InputType.RightTrigger:
                _image.sprite = _controlerManager._buttonRTPlaystation;
                break;
            case InputType.Start:
                _image.sprite = _controlerManager._buttonStartPlaystation;
                break;
            case InputType.Select:
                _image.sprite = _controlerManager._buttonSelectPlaystation;
                break;
            default:
                break;
        }
    }
}
