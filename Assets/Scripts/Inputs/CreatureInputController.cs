using Cursed.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureInputController : MonoBehaviour
    {
        private bool _hasCalled;
        public bool Down { get; private set; }
        public bool Up { get; private set; }
        public bool Holding { get; private set; }
        public bool Sonar { get; private set; }

        private ControlerManager _controlerManager;

        private void Start()
        {
            _controlerManager = ControlerManager.Instance;
        }

        void Update()
        {
            if (_hasCalled)
            {
                Down = false;
            }

            #region XBOX CONTROLS

            if (_controlerManager._ControlerType == ControlerManager.ControlerType.XBOX || _controlerManager._ControlerType == ControlerManager.ControlerType.None)
            {
                if (Input.GetAxis("Creature") > .5f && !_hasCalled)
                {
                    Down = true;
                    Holding = true;
                    _hasCalled = true;
                    Up = false;
                }
                else
                {
                    Up = false;
                    Down = false;
                }

                if (Input.GetAxis("Creature") < .5f && _hasCalled)
                {
                    Down = false;
                    Holding = false;
                    Up = true;
                    _hasCalled = false;
                }

                if (Input.GetButton("CreatureSonar"))
                    Sonar = true;
                if (Input.GetButtonUp("CreatureSonar"))
                    Sonar = false;
            }

            #endregion

            #region PS4 CONTROLS

            else if (_controlerManager._ControlerType == ControlerManager.ControlerType.PS4)
            {
                if (Input.GetAxis("Creature_PS4") > .5f && !_hasCalled)
                {
                    Down = true;
                    Holding = true;
                    _hasCalled = true;
                    Up = false;
                }
                else
                {
                    Up = false;
                    Down = false;
                }

                if (Input.GetAxis("Creature_PS4") < .5f && _hasCalled)
                {
                    Down = false;
                    Holding = false;
                    Up = true;
                    _hasCalled = false;
                }

                if (Input.GetButton("CreatureSonar_PS4"))
                    Sonar = true;
                if (Input.GetButtonUp("CreatureSonar_PS4"))
                    Sonar = false;
            }

            #endregion

        }
    }
}
