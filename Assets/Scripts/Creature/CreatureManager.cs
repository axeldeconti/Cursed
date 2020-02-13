using Cursed.Character;
using Cursed.Utilities;
using UnityEngine;
using System.Collections;

namespace Cursed.Creature
{
    public enum CreatureState
    {
        OnCharacter,
        Moving,
        OnEnemy,
        OnComeBack,
        Chasing,
        OnPausing,
        OnWall
    }

    public class CreatureManager : MonoBehaviour
    {
        private CharacterMovement _characterMovement;
        [SerializeField] private CreatureState _creatureState;
        private CreatureMovement _movement;
        private CreatureInputController _input;
        private CreatureJoystickDirection _joystick;
        private Animator _animator;

        public event System.Action OnChangingState;

        [SerializeField] private FloatReference _triggeredTimer;
        private float _currentTriggeredTimer;

        private bool _canRecall = false;
        [SerializeField] private bool _recall = false;

        private void Start() => Initialize();

        private void Initialize()
        {
            //Init referencies
            _characterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
            _movement = GetComponent<CreatureMovement>();
            _input = GetComponent<CreatureInputController>();
            _animator = GetComponent<Animator>();
            _joystick = GetComponent<CreatureJoystickDirection>();

            //Init Creature State
            CurrentState = CreatureState.OnComeBack;

            CursedDebugger.Instance.Add("State : ", () => CurrentState.ToString());
        }


        private void Update()
        {
            UpdateInput();
            
        }

        private void UpdateInput()
        {
            #region LAUNCH & RECALL
            if (_input.Up)
            {
                if (_recall)
                    _recall = false;
                else
                    DeAttachFromPlayer();
            }

            if (_input.Down && _creatureState != CreatureState.OnCharacter && _canRecall)
            {
                CurrentState = CreatureState.OnComeBack;
                _recall = true;
            }
            #endregion

            #region HOLDING CAMERA ZOOM

            if (_input.Holding)
            {
                if (_creatureState == CreatureState.OnCharacter)
                {
                    if (!_recall)
                    {
                        CameraZoomController.Instance.Zoom(false);
                        _currentTriggeredTimer += Time.deltaTime;
                        if (_currentTriggeredTimer >= _triggeredTimer.Value)
                        {
                            _currentTriggeredTimer = 0f;
                            DeAttachFromPlayer();
                        }
                    }
                }
                else
                    CameraZoomController.Instance.Zoom(true);
            }
            else
                CameraZoomController.Instance.Zoom(true);
            #endregion
        }

        private void DeAttachFromPlayer()
        {
            if (_creatureState != CreatureState.OnCharacter)
                return;

            StartCoroutine(WaitForRecallReady());

            if (_characterMovement.Side != 0)
                _movement.Direction = _characterMovement.Side;
            else
                _movement.Direction = 1;

            if (_joystick.Direction != Vector3.zero)
            {
                if (_joystick.Target != null)
                    transform.position = _joystick.Target.GetChild(0).position;
            }
            else
                transform.position = _characterMovement.transform.GetChild(0).position + new Vector3(4f * _movement.Direction, 0f);

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

        IEnumerator WaitForRecallReady()
        {
            _canRecall = false;
            yield return new WaitForSeconds(.75f);
            _canRecall = true;
        }

        public CreatureState CurrentState
        {
            get => _creatureState;
            set
            {
                _creatureState = value;

                switch (_creatureState)
                {
                    case CreatureState.Moving:
                        //ToggleChilds(true);
                        _animator.SetBool("GoToCharacter", false);
                        _animator.SetBool("OnWall", false);
                        _animator.SetBool("Moving", true);
                        //_movement.MoveInTheAir = true;
                        break;

                    case CreatureState.OnCharacter:
                        //ToggleChilds(false);
                        _animator.SetBool("GoToCharacter", true);
                        _animator.SetBool("OnWall", false);
                        _animator.SetBool("Moving", false);
                        break;

                    case CreatureState.OnComeBack:
                        ToggleChilds(true);
                        //_animator.SetTrigger("Wall");
                        _animator.SetBool("OnWall", false);
                        _animator.SetBool("Moving", true);
                        break;

                    case CreatureState.OnEnemy:
                        //ToggleChilds(false);
                        _animator.SetBool("GoToCharacter", true);
                        _animator.SetBool("Moving", false);
                        _animator.SetBool("OnWall", false);
                        _animator.SetBool("Chasing", false);
                        break;

                    case CreatureState.Chasing:
                        _animator.SetBool("GoToCharacter", false);
                        _animator.SetBool("OnWall", false);
                        _animator.SetBool("Chasing", true);
                        break;

                    case CreatureState.OnPausing:
                        break;

                    case CreatureState.OnWall:
                        _animator.SetBool("OnWall", true);
                        _animator.SetBool("Moving", false);
                        _animator.SetBool("Chasing", false);
                        break;
                }
            }
        }

        #region GETTERS & SETTERS

        public Vector2 DirectionVector => _joystick.Direction;
        public int DirectionInt => _movement.Direction;
        public bool Recall => _recall;

        #endregion
    }
}