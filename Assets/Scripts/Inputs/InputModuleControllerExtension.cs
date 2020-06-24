using Cursed.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputModuleControllerExtension : MonoBehaviour
{
    private StandaloneInputModule _standaloneInputModule;

    private ControlerManager _controlerManager;

    private void Awake()
    {
        _standaloneInputModule = GetComponent<StandaloneInputModule>();
        _controlerManager = ControlerManager.Instance;
    }

    private void Update()
    {
        #region XBOX CONTROLS
        if (_controlerManager._ControlerType == ControlerManager.ControlerType.XBOX || _controlerManager._ControlerType == ControlerManager.ControlerType.None)
        {
            _standaloneInputModule.horizontalAxis = "Horizontal";
            _standaloneInputModule.verticalAxis = "Vertical";
            _standaloneInputModule.submitButton = "Jump";
        }

        #endregion

        #region PS4 CONTROLS
        if (_controlerManager._ControlerType == ControlerManager.ControlerType.PS4)
        {
            _standaloneInputModule.horizontalAxis = "Horizontal_PS4";
            _standaloneInputModule.verticalAxis = "Vertical_PS4";
            _standaloneInputModule.submitButton = "Jump_PS4";
        }

        #endregion
    }
}
