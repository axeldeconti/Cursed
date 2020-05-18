using Cursed.Character;
using Cursed.Utilities;
using System.Collections;
using UnityEngine;
using Cursed.VisualEffect;
using Cursed.Managers;

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
        OnWall,
        OnDoorSwitch,
        OnLaser
    }

    public class CreatureManager : MonoBehaviour
    {
        private CharacterMovement _characterMovement;
        [SerializeField] private CreatureState _creatureState;
        private CreatureMovement _movement;
        private CreatureInputController _input;
        private CreatureJoystickDirection _joystick;
        private Animator _animator;
        private CreatureVfxHandler _vfx;
        private CreatureCollision _collision;

        public event System.Action OnChangingState;

        [SerializeField] private FloatReference _timeBeforeZoom;
        private float _currentTimerBeforeZoom;

        private bool _canRecall = false;

        [SerializeField] private bool _showDebug = false;
        [SerializeField] private bool _recallOffScreen = true;

        private Vector2 _launchDirection;

        [Space]
        [Header("Stats Camera Shake")]
        [SerializeField] private ShakeData _shakeLaunchCreature = null;
        [SerializeField] private ShakeData _shakeCreatureOnCharacter = null;
        [SerializeField] private ShakeDataEvent _onCamShake = null;

        private void Start() => Initialize();

        private GameObject _refCreatureTrailEffect;
        private Transform _colliderTransform;

        private void Initialize()
        {
            //Init referencies
            _characterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
            _movement = GetComponent<CreatureMovement>();
            _input = GetComponent<CreatureInputController>();
            _animator = GetComponent<Animator>();
            _joystick = GetComponent<CreatureJoystickDirection>();
            _vfx = GetComponent<CreatureVfxHandler>();
            _collision = GetComponentInChildren<CreatureCollision>();
            _colliderTransform = GetComponentInChildren<Collider2D>().transform;

            //Init Creature State
            CurrentState = CreatureState.OnComeBack;

            if (_showDebug)
            {
                CursedDebugger.Instance.Add("State : ", () => CurrentState.ToString());
            }
        }


        private void Update()
        {
            #region GET PLAYER MOVEMENT

            if(_characterMovement == null)
                _characterMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();

            #endregion

            UpdateInput();
            CameraZoom();

            if (_recallOffScreen)
                CheckDistanceFromPlayer();
        }

        private void UpdateInput()
        {
            if (GameManager.Instance.State == GameManager.GameState.Pause)
                return;

            #region LAUNCH & RECALL
            if (_input.Down && _creatureState == CreatureState.OnCharacter)
            {
                DeAttachFromPlayer();
            }
            if (_input.Down && _creatureState != CreatureState.OnCharacter && _canRecall)
            {
                if (CreatureOnCharacter.Instance != null)
                    Destroy(CreatureOnCharacter.Instance.gameObject);

                CurrentState = CreatureState.OnComeBack;
                AkSoundEngine.PostEvent("Play_Creature_Call", gameObject);
            }


            #endregion
        }

        private void CameraZoom()
        {
            if (_creatureState == CreatureState.OnCharacter)
            {

                if (_characterMovement.State == CharacterMovementState.Idle)
                {
                    if (_joystick.Direction != Vector3.zero)
                    {
                        _currentTimerBeforeZoom += Time.deltaTime;
                        if (_currentTimerBeforeZoom >= _timeBeforeZoom)
                        {
                            CameraZoomController.Instance.Zoom(CameraZoomController.Instance._maxZoomCreature, CameraZoomController.Instance._zoomOutCreatureSpeed);
                        }
                    }
                    else
                        ResetZoom();
                }
                else
                    ResetZoom();
            }
            else
                ResetZoom();
        }

        private void ResetZoom()
        {
            _currentTimerBeforeZoom = 0f;
            CameraZoomController.Instance.Zoom(CameraZoomController.Instance._initialZoom, CameraZoomController.Instance._zoomInCreatureSpeed);
        }

        private void CheckDistanceFromPlayer()
        {
            if (CurrentState == CreatureState.Moving || CurrentState == CreatureState.OnWall)
            {
                float borderSize = 0f;
                Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(_colliderTransform.position);
                bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;
                if (isOffScreen)
                    LaunchComeBack();
            }

        }

        private void LaunchComeBack()
        {
            CurrentState = CreatureState.OnComeBack;
        }

        private void DeAttachFromPlayer()
        {
            if (_creatureState != CreatureState.OnCharacter)
                return;

            //Play SFX
            AkSoundEngine.PostEvent("Play_Creature_Launch", gameObject);


            StartCoroutine(WaitForRecallReady());

            if (_characterMovement.Side != 0)
                _movement.Direction = _characterMovement.Side;
            else
                _movement.Direction = 1;

            if (_joystick.Direction != Vector3.zero)
            {
                if (_joystick.Target != null)
                {
                    //transform.position = _joystick.Target.GetChild(0).position;
                    transform.position = _characterMovement.transform.GetChild(0).position + new Vector3(1f * _movement.Direction, 0f);
                    _launchDirection = _joystick.Direction;
                }
            }
            else
            {
                _launchDirection = new Vector2(_movement.Direction, 0f);
                transform.position = _characterMovement.transform.GetChild(0).position + new Vector3(1f * _movement.Direction, 0f);
            }

            //Play VFX
            _vfx.CreatureLauchParticle(_launchDirection);
            _onCamShake?.Raise(_shakeLaunchCreature);

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
                        _vfx.CreatureAberration(_vfx._lowerChromacticAberrationValue);
                        _animator.SetBool("GoToCharacter", false);
                        _animator.SetBool("OnWall", false);
                        _animator.SetBool("Moving", true);
                        //_movement.MoveInTheAir = true;
                        if (_refCreatureTrailEffect == null)
                            _refCreatureTrailEffect = _vfx.CreatureTrailParticle();
                        break;

                    case CreatureState.OnCharacter:
                        //ToggleChilds(false);
                        _vfx.CreatureTouchImpactParticle(_collision.HitTransform.GetChild(0));
                        _vfx.CreatureAberration(_vfx._higherChromacticAberrationValue);
                        _onCamShake?.Raise(_shakeCreatureOnCharacter);
                        _animator.SetBool("GoToCharacter", true);
                        _animator.SetBool("OnWall", false);
                        _animator.SetBool("Moving", false);
                        Destroy(_refCreatureTrailEffect);
                        break;

                    case CreatureState.OnComeBack:
                        //ToggleChilds(true);
                        //_animator.SetTrigger("Wall");
                        _animator.SetBool("GoToCharacter", false);
                        _animator.SetBool("OnWall", false);
                        _animator.SetBool("Moving", true);
                        _collision.CheckCreatureOnObject();
                        if (_refCreatureTrailEffect == null)
                            _refCreatureTrailEffect = _vfx.CreatureTrailParticle();
                        break;

                    case CreatureState.OnEnemy:
                        //ToggleChilds(false);
                        _vfx.CreatureTouchImpactParticle(_collision.HitTransform.GetChild(0));
                        _animator.SetBool("GoToCharacter", true);
                        _animator.SetBool("Moving", false);
                        _animator.SetBool("OnWall", false);
                        _animator.SetBool("Chasing", false);
                        Destroy(_refCreatureTrailEffect);
                        break;

                    case CreatureState.Chasing:
                        _animator.SetBool("GoToCharacter", false);
                        _animator.SetBool("OnWall", false);
                        _animator.SetBool("Chasing", true);
                        if (_refCreatureTrailEffect == null)
                            _refCreatureTrailEffect = _vfx.CreatureTrailParticle();
                        break;

                    case CreatureState.OnPausing:
                        break;

                    case CreatureState.OnWall:
                        _animator.SetBool("OnWall", true);
                        _animator.SetBool("Moving", false);
                        _animator.SetBool("Chasing", false);
                        Destroy(_refCreatureTrailEffect);
                        break;

                    case CreatureState.OnDoorSwitch:
                        //_vfx.CreatureTouchImpactParticle(_collision.HitTransform.GetChild(0));
                        _animator.SetBool("GoToCharacter", true);
                        _animator.SetBool("Moving", false);
                        _animator.SetBool("OnWall", false);
                        _animator.SetBool("Chasing", false);
                        Destroy(_refCreatureTrailEffect);
                        break;

                    case CreatureState.OnLaser:
                        _animator.SetBool("GoToCharacter", true);
                        _animator.SetBool("OnWall", false);
                        _animator.SetBool("Moving", false);
                        _animator.SetBool("Chasing", false);
                        Destroy(_refCreatureTrailEffect);
                        break;
                }
            }
        }

        #region GETTERS & SETTERS

        public Vector2 LaunchDirection => _launchDirection;
        public int DirectionInt => _movement.Direction;


        #endregion
    }
}