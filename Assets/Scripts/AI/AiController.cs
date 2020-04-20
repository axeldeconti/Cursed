﻿using System.Collections;
using UnityEngine;

namespace Cursed.AI
{
    public class AiController : MonoBehaviour
    {
        private static Pathfinding _pathfindingMgr;
        private static GameObject _player;

        [SerializeField] private AIState _state = AIState.GroundPatrol;

        private PathfindingAgent _pathAgent;

        private bool _destroy = false;
        private bool _timerChangeTarget = false;

        private void Awake()
        {
            if (_player == null)
                _player = GameObject.FindGameObjectWithTag("Player");

            _pathAgent = GetComponent<PathfindingAgent>();

            if (_pathfindingMgr == null)
                _pathfindingMgr = Pathfinding.Instance;
        }

        /*Destroy object on lateupdate to avoid warning errors of objects not existing*/
        private void LateUpdate()
        {
            if (_destroy)
                Destroy(gameObject);
        }

        //Check player distance and do what told to wether or not player is in distance
        private bool PlayerInRange(float range, bool raycastOn)
        {
            if (_player && Vector3.Distance(_player.transform.position, transform.position) < range)
            {
                if (raycastOn && !Physics2D.Linecast(transform.position, _player.transform.position/*, _ray.collisionMask*/))
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

        public bool NeedsPathfinding()
        {
            if (_state == AIState.GroundPatrol || _state == AIState.Chase)
                return true;

            _pathAgent.CancelPathing();
            return false;
        }

        public void GetInput(ref Vector3 velocity, ref Vector2 input, ref bool jumpRequest)
        {
            switch (_state)
            {
                case AIState.None:
                    break;
                case AIState.GroundPatrol:
                    GroundPatrol(ref input);
                    break;
                case AIState.Chase:
                    Chase();
                    break;
                case AIState.Attack:
                    AttackOnRange();
                    break;
                default:
                    break;
            }

            if (_state == AIState.Chase || _state == AIState.GroundPatrol)
                _pathAgent.AiMovement(ref velocity, ref input, ref jumpRequest);
        }

        private void Chase()
        {
            if (!PlayerInRange(30f, false)) //Change boolean to true for OnSight aggro / 6f-30f
            {
                _state = AIState.GroundPatrol;
                return;
            }
            if (PlayerInRange(5f, true)) //Change boolean to true for OnSight aggro / 1f-5f
            {
                _state = AIState.Attack;
                return;
            }

            _pathAgent.Target = _player;
            _state = AIState.Chase;
        }

        private void GroundPatrol(ref Vector2 input)
        {
            //Switch to chase if player in range
            if (PlayerInRange(30f, true)) //Change boolean to true for OnSight aggro / 6f-30f
            {
                _state = AIState.Chase;
                return;
            }

            //Coroutine for changing targeted platform
            if (_timerChangeTarget == false)
            {
                StartCoroutine(TimerForSwitchTarget());
            }
        }

        //Coroutine to switch tile/node target
        private IEnumerator TimerForSwitchTarget()
        {
            _timerChangeTarget = true;
            yield return new WaitForSeconds(5f);
            _pathAgent.Target = _pathfindingMgr.GroundNodes[Random.Range(0, _pathfindingMgr.GroundNodes.Count)].gameObject;
            _pathAgent.RequestPath(_pathAgent.Target.transform.position + new Vector3(0, 3, 0));
            _timerChangeTarget = false;
        }

        private void AttackOnRange()
        {
            if (!PlayerInRange(5f, true)) //Change boolean to true for OnSight aggro / 1f-5f
            {
                _state = AIState.Chase;
                return;
            }
            //Insert attack behavior
        }

        #region Getters & Setters

        public AIState State => _state;

        #endregion
    }

    public enum AIState { None, GroundPatrol, Chase, Attack }
}