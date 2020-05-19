﻿using Cursed.Character;
using UnityEngine;

namespace Cursed.AI
{
    public class AiController : MonoBehaviour
    {
        private static Pathfinding _pathfindingMgr;

        private PathfindingAgent _pathAgent = null;
        private CollisionHandler _col = null;
        private CharacterMovement _move = null;

        [SerializeField] private AIState _state = AIState.GroundPatrol;
        [SerializeField] private AiTarget _target = null;

        [Header("Data")]
        [SerializeField] private FloatReference _aggroRange = null;
        [SerializeField] private FloatReference _timeToChangePlatformTarget = null;

        [Header("Debug")]
        [SerializeField] private bool _debugLogs = false;
        [SerializeField] private bool _debugDraws = false;

        private bool _isMoving = false;
        private int _nbOfDirties = 0;

        //Chase
        private float _currentTimeToChangePlatformTarget = 0f;

        private bool _destroy = false;

        private void Awake()
        {
            _pathAgent = GetComponent<PathfindingAgent>();
            _col = GetComponent<CollisionHandler>();
            _move = GetComponent<CharacterMovement>();

            if (_pathfindingMgr == null)
                _pathfindingMgr = Pathfinding.Instance;

            _pathAgent.OnPathCompleted += OnPathCompleted;
            _pathAgent.OnPathDirty += OnPathDirty;
        }

        private void Start()
        {
            _isMoving = false;
            _nbOfDirties = 0;
            _currentTimeToChangePlatformTarget = 0;
            _destroy = false;
        }

        private void LateUpdate()
        {
            //Destroy object on lateupdate to avoid warning errors of objects not existing
            if (_destroy)
                Destroy(gameObject);
        }

        /// <summary>
        /// Check target distance with a check on line of sight
        /// </summary>
        private bool TargetInRange(float range, bool raycastOn)
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
        private RaycastHit2D RaycastInFront(float distance)
        {
            //Transform at character's feet so up the position a bit
            Vector3 pos = transform.position + Vector3.up * 2 + Vector3.right * _move.Side * 2.5f;
            return Physics2D.Linecast(pos, pos + Vector3.right * _move.Side * distance);
        }

        /// <summary>
        /// Called by the Pathfinding Agent to check if a path is needed
        /// </summary>
        /// <returns></returns>
        public bool NeedsPathfinding()
        {
            if (_state == AIState.GroundPatrol || _state == AIState.Chase)
                return true;

            _pathAgent.CancelPathing();
            return false;
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
            switch (_state)
            {
                case AIState.None:
                    break;
                case AIState.GroundPatrol:
                    GroundPatrol(ref data);
                    _pathAgent.AiMovement(ref data);
                    break;
                case AIState.Chase:
                    Chase();
                    _pathAgent.AiMovement(ref data);
                    break;
                case AIState.Attack:
                    AttackOnRange();
                    break;
                default:
                    break;
            }
        }

        #region Ground Patrol
        private void GroundPatrol(ref AIData data)
        {
            //Check for a target if I have none
            if (!_target)
            {
                RaycastHit2D hit = RaycastInFront(_aggroRange);
                if (hit)
                {
                    //Set target to the AiTarget if one is hit
                    _target = hit.collider.GetComponent<AiTarget>();
                }
            }

            //Switch to chase if player in range
            if (TargetInRange(_aggroRange, true))
            {
                State = AIState.Chase;
                return;
            }

            if (!_isMoving)
                _currentTimeToChangePlatformTarget -= Time.deltaTime;

            //It's time to change target
            if (_currentTimeToChangePlatformTarget <= 0)
            {
                //Change target
                FindRandomPathTarget();

                //Reset timer
                _currentTimeToChangePlatformTarget = _timeToChangePlatformTarget;

                _isMoving = true;
            }
        }

        /// <summary>
        /// Set the Path Agent target to a random tile
        /// </summary>
        private void FindRandomPathTarget()
        {
            _pathAgent.Target = _pathfindingMgr.GroundNodes[Random.Range(0, _pathfindingMgr.GroundNodes.Count)].gameObject.transform.position;
            _pathAgent.RequestPath(_pathAgent.Target + new Vector3(0, 1, 0));
        }

        #endregion

        #region Chase
        private void Chase()
        {
            if (!TargetInRange(30f, false))
            {
                State = AIState.GroundPatrol;
                return;
            }
            if (TargetInRange(5f, true))
            {
                State = AIState.Attack;
                return;
            }

            int side = _target.Position.x > transform.position.x ? -1 : 1;
            _pathAgent.Target = _target.Position;
            _pathAgent.Target = _pathAgent.Target + Vector3.right * side * 4;
        }

        #endregion

        #region Attack
        private void AttackOnRange()
        {
            if (!TargetInRange(5f, true)) //Change boolean to true for OnSight aggro / 1f-5f
            {
                State = AIState.Chase;
                return;
            }
            //Insert attack behavior
        }

        #endregion

        /// <summary>
        /// Called by the Path Agent when the path is completed
        /// </summary>
        private void OnPathCompleted()
        {
            _isMoving = false;
            _nbOfDirties = 0;

            switch (_state)
            {
                case AIState.None:
                    break;
                case AIState.GroundPatrol:
                    break;
                case AIState.Chase:
                    break;
                case AIState.Attack:
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

        private void ExitState(AIState exitState)
        {
            switch (exitState)
            {
                case AIState.None:
                    break;
                case AIState.GroundPatrol:
                    break;
                case AIState.Chase:
                    break;
                case AIState.Attack:
                    break;
                default:
                    break;
            }
        }

        private void Log(string log)
        {
            Debug.Log("[AIC] : " + log);
        }

        #region Getters & Setters

        public AIState State
        {
            get => _state;
            set
            {
                ExitState(_state);
                _state = value;
            }
        }

        #endregion

        private void OnDrawGizmos()
        {
            if (!_debugDraws || _move == null)
                return;

            switch (_state)
            {
                case AIState.None:
                    break;
                case AIState.GroundPatrol:
                    Gizmos.color = Color.red;
                    int i = _move.Side;
                    Vector3 pos = transform.position + Vector3.up * 2 + Vector3.right * _move.Side * 2.5f;
                    Gizmos.DrawLine(pos, pos + Vector3.right * _move.Side * _aggroRange);
                    break;
                case AIState.Chase:
                    break;
                case AIState.Attack:
                    break;
                default:
                    break;
            }
        }
    }

    public enum AIState { None, GroundPatrol, Chase, Attack }
}