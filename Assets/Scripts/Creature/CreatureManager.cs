using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cursed.Character;

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
        public CharacterMovement _characterMovement;
        private CreatureState _creatureState;
        private CreatureMovement _movement;
        private CreatureInputController _input;

        public event System.Action OnChangingState;

        private void Start() => Initialize();

        private void Initialize()
        {
            //Init referencies
            _movement = GetComponent<CreatureMovement>();
            _input = GetComponent<CreatureInputController>();

            //Init Creature State
            CurrentState = CreatureState.OnComeBack;
        }


        private void Update()
        {
            if(_input.Creature || Input.GetButtonDown("Creature"))
            {
                if(_creatureState == CreatureState.OnCharacter) DeAttachFromPlayer();
                else CurrentState = CreatureState.OnComeBack;
            }
        }

        private void OnCharacter()
        {
            _movement.MoveToPlayer = false;
            ToggleChilds(false);
        }

        private void DeAttachFromPlayer()
        {
            transform.position = _characterMovement.transform.position + new Vector3(0f, 1f, 0f);

            _movement.Direction = _characterMovement.Side;
            CurrentState = CreatureState.Moving;
        }

        private void ToggleChilds(bool active)
        {
            // Activate-Deactivate renderers and colliders
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(active);
            }
        }

        public CreatureState CurrentState 
        {
            get => _creatureState;
            set 
            {
                _creatureState = value;

                switch(_creatureState)
                {
                    case CreatureState.Moving:
                        ToggleChilds(true);
                        _movement.MoveInTheAir = true;
                        break;
                
                    case CreatureState.OnCharacter:
                        OnCharacter();
                        break;
                
                    case CreatureState.OnComeBack:
                        // Launch movement to player
                        _movement.MoveInTheAir = false;
                        _movement.MoveToPlayer = true;
                        break;
                
                    case CreatureState.OnEnemy:
                        break;
                    
                    case CreatureState.OnPausing:
                        break;
                }
            }
        }
    }
}