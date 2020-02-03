﻿using UnityEngine;
using Cursed.Character;

namespace Cursed.Creature
{
    public enum CreatureState
    {
        OnCharacter,
        Moving,
        OnEnemy,
        OnComeBack,
        Chasing,
        OnPausing
    }

    public class CreatureManager : MonoBehaviour
    {
        public CharacterMovement _characterMovement;
        [SerializeField] private CreatureState _creatureState;
        private CreatureMovement _movement;
        private CreatureInputController _input;
        private CreatureJoystickDirection _joystick;
        private Animator _animator;

        public event System.Action OnChangingState;

        private void Start() => Initialize();

        private void Initialize()
        {
            //Init referencies
            _movement = GetComponent<CreatureMovement>();
            _input = GetComponent<CreatureInputController>();
            _animator = GetComponent<Animator>();
            _joystick = GetComponent<CreatureJoystickDirection>();

            //Init Creature State
            CurrentState = CreatureState.OnComeBack;

            CursedDebugger.Instance.Add("State : ",() => CurrentState.ToString());
        }


        private void Update()
        {
            if(_input.Creature || Input.GetButtonDown("Creature"))
            {
                if(_creatureState == CreatureState.OnCharacter) 
                    DeAttachFromPlayer();
                else
                {
                    CurrentState = CreatureState.OnComeBack;
                }
            }
        }

        private void DeAttachFromPlayer()
        {
            _movement.Direction = _characterMovement.Side;

            if (_joystick.Direction != Vector2.zero)
                transform.position = _joystick.Target.position;
            else
                transform.position = _characterMovement.transform.GetChild(0).position + new Vector3(5f * _movement.Direction, 0f);

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
                        //ToggleChilds(true);
                        _animator.SetBool("GoToCharacter", false);
                        _animator.SetBool("Moving", true);
                        //_movement.MoveInTheAir = true;
                        break;
                
                    case CreatureState.OnCharacter:
                        //ToggleChilds(false);
                        _animator.SetBool("GoToCharacter", true);
                        _animator.SetBool("Moving", false);
                        break;
                
                    case CreatureState.OnComeBack:
                        // Launch movement to player
                        ToggleChilds(true);
                        _animator.SetTrigger("Wall");
                        _animator.SetBool("Moving", true);
                        break;
                
                    case CreatureState.OnEnemy:
                        //ToggleChilds(false);
                        _animator.SetBool("GoToCharacter", true);
                        _animator.SetBool("Moving", false);
                        _animator.SetBool("Chasing", false);
                        break;

                    case CreatureState.Chasing:
                        _animator.SetBool("GoToCharacter", false);
                        _animator.SetBool("Chasing", true);
                        break;
                    
                    case CreatureState.OnPausing:
                        break;
                }
            }
        }
    }
}