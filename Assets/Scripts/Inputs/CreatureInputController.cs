using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureInputController : MonoBehaviour
    {
        public bool Creature { get; private set; }

        void Update()
        {
            Creature = Input.GetAxis("Creature") > .5f ? true : false;
        }
    }
}
