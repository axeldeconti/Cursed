using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureInputController : MonoBehaviour
    {
        public bool CreatureOnCharacter { get; private set; }
        public bool CreatureInAir { get; private set; }
        private bool hasCalled;
        private bool _canRecall;

        void Update()
        {

            if (Input.GetAxis("Creature") > .5f && !CreatureOnCharacter && !hasCalled)
            {
                hasCalled = true;
                CreatureInAir = true;
            }

            else
            {
                CreatureOnCharacter = false;
                CreatureInAir = false;
            }


            if (Input.GetAxis("Creature") < .5f && hasCalled)
            {
                CreatureOnCharacter = true;
                hasCalled = false;
                CreatureInAir = false;
            }
        }

        public bool ButtonTriggered => hasCalled;
    }
}
