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

        void Update()
        {
            if (_hasCalled)
            {
                Down = false;
            }

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

            if(Input.GetAxis("Creature") < .5f && _hasCalled)
            {
                Down = false;
                Holding = false;
                Up = true;
                _hasCalled = false;
            }

        }
    }
}
