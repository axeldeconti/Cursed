using Cursed.Utilities;
using UnityEngine;
using XInputDotNetPure; // Required in C#

namespace Cursed.VisualEffect
{
    public class ControllerVibration : Singleton<ControllerVibration>
    {
        private bool _playerIndexSet = false;
        private PlayerIndex _playerIndex;
        private GamePadState _state;
        private GamePadState _prevState;
        private bool _isVibrating = false;
        private float _timer = -1;

        private void Update()
        {
            UpdateVibration();
        }

        private void CheckPlayerIndex()
        {
            // Find a PlayerIndex, for a single player game
            // Will find the first controller that is connected ans use it
            if (!_playerIndexSet || !_prevState.IsConnected)
            {
                for (int i = 0; i < 4; ++i)
                {
                    PlayerIndex testPlayerIndex = (PlayerIndex)i;
                    GamePadState testState = GamePad.GetState(testPlayerIndex);
                    if (testState.IsConnected)
                    {
                        _playerIndex = testPlayerIndex;
                        _playerIndexSet = true;
                    }
                }
            }

            _prevState = _state;
            _state = GamePad.GetState(_playerIndex);
        }

        private void UpdateVibration()
        {
            if (!_isVibrating)
                return;

            _timer -= Time.deltaTime;

            if(_timer <= 0 || Time.timeScale == 0)
            {
                GamePad.SetVibration(_playerIndex, 0, 0);
                _isVibrating = false;
            }
        }

        public void StartVibration(VibrationData_SO data)
        {
            CheckPlayerIndex();

            if (_isVibrating && !_playerIndexSet)
                return;

            _isVibrating = true;
            GamePad.SetVibration(_playerIndex, data.LeftMotorIntensity, data.RightMotorIntensity);
            _timer = data.VibrationTime;
        }

        private void OnDestroy()
        {
            GamePad.SetVibration(_playerIndex, 0, 0);
        }
    }
}