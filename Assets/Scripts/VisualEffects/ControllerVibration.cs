using Cursed.Character;
using System.Collections;
using UnityEngine;
using XInputDotNetPure; // Required in C#

namespace Cursed.VisualEffect
{
    public class ControllerVibration : MonoBehaviour
    {
        private bool playerIndexSet = false;
        private PlayerIndex playerIndex;
        private GamePadState state;
        private GamePadState prevState;

        private void Update()
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
        }

        public IEnumerator MakeVibration(float _time, float _leftMotor, float _rightMotor)
        {
            // Make Vibration
            Debug.Log("aaa");
            GamePad.SetVibration(playerIndex, _leftMotor, _rightMotor);
            yield return new WaitForSecondsRealtime(_time);
            GamePad.SetVibration(playerIndex, 0, 0);
        }

    }
}