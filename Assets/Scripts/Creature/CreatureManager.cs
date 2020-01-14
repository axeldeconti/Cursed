using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Creature
{
    public enum CreatureState
    {
        OnCharacter,
        Moving,
        OnEnemy,
        OnComeBack,
        OnPausing
    }

    public class CreatureManager : MonoBehaviour
    {
        private CreatureState _creatureState;
        private CreatureMovement _movement;
        private CreatureInputController _input;

        public event System.Action OnChangingState;

        private void Start() => Initialize();

        private void Initialize()
        {
            //Init Creature State
            _creatureState = CreatureState.OnCharacter;
            if(OnChangingState != null) OnChangingState.Invoke();

            //Init referencies
            _movement = GetComponent<CreatureMovement>();
            _input = GetComponent<CreatureInputController>();
        }


        private void Update()
        {
            if(_input.Creature)
            {
                _movement.MoveToDirection(new Vector2(1,0));
            }
        }

        public CreatureState GetCurrentState()
        {
            return _creatureState;
        }
    }
}