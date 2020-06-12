using UnityEngine;
using System;

namespace Cursed.Managers
{
    public class ControlerManager : Singleton<ControlerManager>
    {
        public enum ControlerType { XBOX, PS4, None}

        private ControlerType _currentControler = ControlerType.None;

        public event Action<ControlerType> _onControlerChanged;

        [Header("Xbox Inputs")]
        public Sprite _buttonAXbox;
        public Sprite _buttonBXbox;
        public Sprite _buttonXXbox;
        public Sprite _buttonYXbox;
        public Sprite _buttonLBXbox;
        public Sprite _buttonLTXbox;
        public Sprite _buttonRBXbox;
        public Sprite _buttonRTXbox;
        public Sprite _buttonStartXbox;
        public Sprite _buttonSelectXbox;

        [Header("PS4 Inputs")]
        public Sprite _buttonAPlaystation;
        public Sprite _buttonBPlaystation;
        public Sprite _buttonXPlaystation;
        public Sprite _buttonYPlaystation;
        public Sprite _buttonLBPlaystation;
        public Sprite _buttonLTPlaystation;
        public Sprite _buttonRBPlaystation;
        public Sprite _buttonRTPlaystation;
        public Sprite _buttonStartPlaystation;
        public Sprite _buttonSelectPlaystation;


        void Update()
        {
            GetControler();
        }

        private ControlerType GetControler()
        {
            string[] names = Input.GetJoystickNames();
            for (int x = 0; x < names.Length; x++)
            {
                if (names[x].Length == 19)
                {
                    if (_currentControler != ControlerType.PS4)
                    {
                        Debug.Log("PS4 Controler is connected !");
                        _currentControler = ControlerType.PS4;
                        _onControlerChanged?.Invoke(_currentControler);
                    }
                }
                if (names[x].Length == 33)
                {
                    if (_currentControler != ControlerType.XBOX)
                    {
                        Debug.Log("XBOX ONE Controler is connected !");
                        _currentControler = ControlerType.XBOX;
                        _onControlerChanged?.Invoke(_currentControler);
                    }
                }
                else
                {
                    Debug.Log("No Controler connected");
                    _currentControler = ControlerType.None;
                }
            }
            return _currentControler;
        }

        #region GETTERS & SETTERS
        public ControlerType _ControlerType => _currentControler;

        #endregion

    }
}
