using Cursed.Character;
using System.Collections;
using UnityEngine;

namespace Cursed.AI
{
    public class AiController : MonoBehaviour
    {
        private static Pathfinding _pathfindingMgr;

        private PathfindingAgent _pathAgent = null;
        private CollisionHandler _col = null;

        [SerializeField] private AIState _state = AIState.GroundPatrol;

        [Header("Data")]
        [SerializeField] private FloatReference _aggroRange = null;
        [SerializeField] private FloatReference _timeToChangePlatformTarget = null;

        private Transform _target = null;
        private bool _isMoving = false;

        //Chase
        private float _currentTimeToChangePlatformTarget = 0f;

        private bool _destroy = false;

        private void Awake()
        {
            _pathAgent = GetComponent<PathfindingAgent>();
            _col = GetComponent<CollisionHandler>();

            if (_pathfindingMgr == null)
                _pathfindingMgr = Pathfinding.Instance;

            _pathAgent.OnPathCompleted += OnPathCompleted;
        }

        private void LateUpdate()
        {
            //Destroy object on lateupdate to avoid warning errors of objects not existing
            if (_destroy)
                Destroy(gameObject);
        }

        /// <summary>
        /// Check player distance with a check on line of sight
        /// </summary>
        private bool PlayerInRange(float range, bool raycastOn)
        {
            if (_target && Vector3.Distance(_target.transform.position, transform.position) < range)
            {
                if (raycastOn && !Physics2D.Linecast(transform.position, _target.transform.position, _pathfindingMgr.GroundLayer))
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
        public void GetInputs(ref Vector2 input, ref bool jumpRequest, ref bool dashRequest, ref bool attack1, ref bool attack2)
        {
            switch (_state)
            {
                case AIState.None:
                    break;
                case AIState.GroundPatrol:
                    GroundPatrol(ref input);
                    _pathAgent.AiMovement(ref input, ref jumpRequest);
                    break;
                case AIState.Chase:
                    Chase();
                    _pathAgent.AiMovement(ref input, ref jumpRequest);
                    break;
                case AIState.Attack:
                    AttackOnRange();
                    break;
                default:
                    break;
            }
        }

        #region Ground Patrol
        private void GroundPatrol(ref Vector2 input)
        {
            //Switch to chase if player in range
            if (PlayerInRange(_aggroRange, true))
            {
                State = AIState.Chase;
                return;
            }

            if(!_isMoving)
                _currentTimeToChangePlatformTarget -= Time.deltaTime;

            //It's time to change target
            if(_currentTimeToChangePlatformTarget <= 0)
            {
                //Change target
                _pathAgent.Target = _pathfindingMgr.GroundNodes[Random.Range(0, _pathfindingMgr.GroundNodes.Count)].gameObject.transform;
                _pathAgent.RequestPath(_pathAgent.Target.transform.position + new Vector3(0, 3, 0));

                //Reset timer
                _currentTimeToChangePlatformTarget = _timeToChangePlatformTarget;

                _isMoving = true;
            }
        }

        #endregion

        #region Chase
        private void Chase()
        {
            if (!PlayerInRange(30f, false)) //Change boolean to true for OnSight aggro / 6f-30f
            {
                State = AIState.GroundPatrol;
                return;
            }
            if (PlayerInRange(5f, true)) //Change boolean to true for OnSight aggro / 1f-5f
            {
                State = AIState.Attack;
                return;
            }

            _pathAgent.Target = _target;
            State = AIState.Chase;
        }

        #endregion

        #region Attack
        private void AttackOnRange()
        {
            if (!PlayerInRange(5f, true)) //Change boolean to true for OnSight aggro / 1f-5f
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
    }

    public enum AIState { None, GroundPatrol, Chase, Attack }
}