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
        Chasing,
        OnPausing
    }

    public class CreatureManager : MonoBehaviour
    {
        public CharacterMovement _characterMovement;
        [SerializeField] private CreatureState _creatureState;
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
            transform.position = _characterMovement.transform.position + new Vector3(1f * _characterMovement.Side, 1.5f, 0f);

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
                        //_movement.MoveInTheAir = true;
                        break;
                
                    case CreatureState.OnCharacter:
                        ToggleChilds(false);
                        break;
                
                    case CreatureState.OnComeBack:
                        // Launch movement to player
                        ToggleChilds(true);
                        break;
                
                    case CreatureState.OnEnemy:
                        ToggleChilds(false);
                        break;

                    case CreatureState.Chasing:
                        break;
                    
                    case CreatureState.OnPausing:
                        break;
                }
            }
        }
    }
}