using System.Collections;
using UnityEngine;
using XInputDotNetPure; // Required in C#

namespace Cursed.VisualEffect
{
    public class ControllerVibrate : MonoBehaviour
    {
        bool playerIndexSet = false;
        PlayerIndex playerIndex;
        GamePadState state;
        GamePadState prevState;

        void Update()
        {
            // Find a PlayerIndex, for a single player game
            // Will find the first controller that is connected ans use it
            if (!playerIndexSet || !prevState.IsConnected)
            {
                for (int i = 0; i < 4; ++i)
                {
                    PlayerIndex testPlayerIndex = (PlayerIndex)i;
                    GamePadState testState = GamePad.GetState(testPlayerIndex);
                    if (testState.IsConnected)
                    {
                        Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                        playerIndex = testPlayerIndex;
                        playerIndexSet = true;
                    }
                }
            }

            prevState = state;
            state = GamePad.GetState(playerIndex);

            if (Input.GetKeyDown(KeyCode.Joystick1Button2))
            {
                StartCoroutine(MakeVibration(0.2f, 1f, 1f));
            }
        }

        private IEnumerator MakeVibration(float _time, float _leftMotor, float _rightMotor)
        {
            // Make Vibration
            GamePad.SetVibration(playerIndex, _leftMotor, _rightMotor);
            yield return new WaitForSecondsRealtime(_time);
            GamePad.SetVibration(playerIndex, 0, 0);
        }

    }
}