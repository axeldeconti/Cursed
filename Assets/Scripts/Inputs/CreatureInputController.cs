using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureInputController : MonoBehaviour
    {
        public bool Creature { get; private set; }
        private bool hasCalled;

        void Update()
        {
            //Creature = Input.GetAxis("Creature") > .5f ? true : false;

            if(Input.GetAxis("Creature") > .5f && !Creature && !hasCalled)
            {
                Creature = true;
                hasCalled = true;
            }
            else 
                Creature = false;

            if(Input.GetAxis("Creature") < .5f)
                hasCalled = false;
        }
    }
}
