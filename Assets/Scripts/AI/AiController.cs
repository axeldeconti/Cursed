using Cursed.Character;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.AI
{
    public class AiController : MonoBehaviour
    {
        private static Pathfinding _pathfindingMgr;

        private PathfindingAgent _pathAgent = null;
        private CollisionHandler _col = null;
        private CharacterMovement _move = null;
        private CharacterAttackManager _atk = null;
        private EnemyHealth _health = null;

        [SerializeField] private AiTarget _target = null;

        [Header("States")]
        [SerializeField] private string _state = "None";
        [SerializeField] private List<AiState> _allStates = null;

        [Header("Debug")]
        [SerializeField] private bool _debugLogs = false;
        [SerializeField] private bool _debugDraws = false;
        [SerializeField] private FloatReference _aggroRange = null;
        [SerializeField] private FloatReference _attackRange = null;

        private bool _isMoving = false;
        private int _nbOfDirties = 0;

        private bool _destroy = false;

        private void Awake()
        {
            _pathAgent = GetComponent<PathfindingAgent>();
            _col = GetComponent<CollisionHandler>();
            _move = GetComponent<CharacterMovement>();
            _atk = GetComponent<CharacterAttackManager>();
            _health = GetComponent<EnemyHealth>();

            _pathAgent.OnPathCompleted += OnPathCompleted;
            _pathAgent.OnPathDirty += OnPathDirty;
            _health.onAttack += OnAttackCallback;
        }

        private void Start()
        {
            if (_pathfindingMgr == null)
                _pathfindingMgr = Pathfinding.Instance;

            _isMoving = false;
            _nbOfDirties = 0;
            _destroy = false;

            if (_allStates.Count > 0)
                _state = _allStates[0].Name;
        }

        private void LateUpdate()
        {
            //Destroy object on lateupdate to avoid warning errors of objects not existing
            if (_destroy)
                Destroy(gameObject);
        }

        /// <summary>
        /// Retrieve the inputs
        /// </summary>
        /// <param name="input">Input like the joystick [-1, 0, 1]</param>
        /// <param name="jumpRequest"></param>
        /// <param name="dashRequest"></param>
        /// <param name="attack1"></param>
        /// <param name="attack2"></param>
        public void GetInputs(ref AIData data)
        {
            UpdateState(ref data);
        }

        /// <summary>
        /// Callback for the enemy health onAttack action
        /// </summary>
        /// <param name="attacker"></param>
        public void OnAttackCallback(GameObject attacker)
        {
            AiTarget target = attacker.GetComponent<AiTarget>();

            if (target)
            {
                if (HasState("Chase"))
                {
                    SetState(CurrentState(), "Chase");
                    _target = target;
                }
            }
        }

        #region Checks and Raycasts
        /// <summary>
        /// Check target distance with a check on line of sight
        /// </summary>
        public bool TargetInRange(float range, bool raycastOn)
        {
            if (_target && Vector3.Distance(_target.Position, transform.position) < range)
            {
                //Transform at character's feet so up the position a bit
                Vector3 pos = transform.position + Vector3.up * 2;
                if (raycastOn && !Physics2D.Linecast(pos, _target.Position, _pathfindingMgr.GroundLayer))
                {
                    return true;
                }
                else if (!raycastOn)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Create a raycast in front of the AI and returns the result
        /// </summary>
        /// <param name="distance">Distance of the raycast</param>
        /// <returns></returns>
        public RaycastHit2D RaycastInFront(float distance)
        {
            //Transform at character's feet so up the position a bit
            return RaycastInFront(distance, 2);
        }

        /// <summary>
        /// Create a raycast in front of the AI and returns the result
        /// </summary>
        /// <param name="distance">Distance of the raycast</param>
        /// <returns></returns>
        public RaycastHit2D RaycastInFront(float distance, float height)
        {
            //Transform at character's feet so up the position a bit
            Vector3 pos = transform.position + Vector3.up * height + Vector3.right * _move.Side * 2.5f;
            return Physics2D.Linecast(pos, pos + Vector3.right * _move.Side * distance);
        }
        #endregion

        #region States
        /// <summary>
        /// Changes state to a new one
        /// </summary>
        /// <param name="newState">New current state</param>
        public void SetState(AiState oldState, string newState)
        {
            if (!HasState(newState))
                throw new ArgumentException("[AI " + gameObject.name + "] : Can't go to " + newState + " from " + oldState.Name);

            oldState.OnStateExit(this);
            _state = newState;
            CurrentState().OnStateEnter(this);
        }

        private void UpdateState(ref AIData data)
        {
            if (_state.Equals("None") || !HasState(_state))
                return;

            CurrentState().OnStateUpdate(this, ref data);
        }

        private AiState CurrentState()
        {
            for (int i = 0; i < _allStates.Count; i++)
            {
                if (_allStates[i].Name.Equals(_state))
                    return _allStates[i];
            }
            return null;
        }

        private AiState GetState(string state)
        {
            for (int i = 0; i < _allStates.Count; i++)
            {
                if (_allStates[i].Name.Equals(state))
                    return _allStates[i];
            }
            return null;
        }

        public bool HasState(string state)
        {
            for (int i = 0; i < _allStates.Count; i++)
            {
                if (_allStates[i].Name.Equals(state))
                    return true;
            }
            return false;
        }
        #endregion

        #region Path
        /// <summary>
        /// Called by the Pathfinding Agent to check if a path is needed
        /// </summary>
        /// <returns></returns>
        public bool NeedsPathfinding()
        {
            if (_state == "GroundPatrol" || _state == "Chase")
                return true;

            _pathAgent.CancelPathing();
            return false;
        }

        /// <summary>
        /// Set the Path Agent target to a random tile
        /// </summary>
        public void FindRandomPathTarget()
        {
            _pathAgent.Target = _pathfindingMgr.GroundNodes[UnityEngine.Random.Range(0, _pathfindingMgr.GroundNodes.Count)].gameObject.transform.position;
            _pathAgent.RequestPath(_pathAgent.Target + new Vector3(0, 1, 0));
        }

        /// <summary>
        /// Called by the Path Agent when the path is completed
        /// </summary>
        private void OnPathCompleted()
        {
            _isMoving = false;
            _nbOfDirties = 0;

            switch (_state)
            {
                case "GroundPatrol":
                    break;
                case "Chase":
                    break;
                case "Attack":
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Called by the Path Agent when the path is dirty
        /// </summary>
        private void OnPathDirty()
        {
            _nbOfDirties++;

            if (_nbOfDirties > 20)
            {
                FindRandomPathTarget();
                _nbOfDirties = 0;
            }
        }
        #endregion

        private void Log(string log)
        {
            Debug.Log("[AIC] : " + log);
        }

        #region Getters & Setters
        public PathfindingAgent PathAgent => _pathAgent;
        public CharacterAttackManager Atk => _atk;
        public CharacterMovement Move => _move;
        public HealthManager Health => _health;
        public string State => _state;
        public AiTarget Target
        {
            get => _target;
            set => _target = value;
        }
        public bool IsMoving
        {
            get => _isMoving;
            set => _isMoving = value;
        }
        #endregion

        private void OnDrawGizmos()
        {
            if (!_debugDraws || _move == null)
                return;

            Vector3 pos = Vector3.zero;
            Vector3 pos2 = Vector3.zero;

            switch (_state)
            {
                case "GroundPatrol":
                    pos = transform.position + Vector3.up * 2 + Vector3.right * _move.Side * 2.5f;
                    pos2 = transform.position + Vector3.up * 2 + Vector3.right * _move.Side * 2.5f;

                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(pos, pos + Vector3.right * _move.Side * _aggroRange);
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(pos2, pos2 + Vector3.right * _move.Side * 5);
                    break;
                case "Chase":
                    pos = transform.position + Vector3.up * 2;
                    Vector3 dir = (_target.Position - pos).normalized;
                    Vector3 aggroRangePos = pos + dir * _aggroRange;
                    Vector3 attackRangePos = pos + dir * _attackRange;
                    pos2 = transform.position + Vector3.up * 2 + Vector3.right * _move.Side * 2.5f;

                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(pos, aggroRangePos);
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(pos, attackRangePos);
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(pos2, pos2 + Vector3.right * _move.Side * 5);
                    break;
                case "Attack":
                    break;
                default:
                    break;
            }
        }
    }
}