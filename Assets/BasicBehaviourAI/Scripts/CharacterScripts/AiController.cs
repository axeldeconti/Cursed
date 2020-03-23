using System.Collections;
using UnityEngine;
using Cursed.Character;


    public class AiController : MonoBehaviour
    {

        public enum ai_state { none, groundpatrol, chase, attack } /*Add custom AI states here!*/

        public ai_state state = ai_state.groundpatrol;

        private RaycastController _ray;
        private PathfindingAgent _pathAgent;
        public static Pathfinding _pathScript;
        [System.NonSerialized]

        public static GameObject player;
        private bool _destroy = false;
        private bool _timerChangeTarget = false;


        private void Awake()
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
            _ray = GetComponent<CharacterController2D>();
            _pathAgent = GetComponent<PathfindingAgent>();

            if (_pathScript == null) { _pathScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<Pathfinding>(); }
        }

        //Check player distance and do what told to wether or not player is in distance
        private bool PlayerInRange(float range, bool raycastOn)
        {
            if (player && Vector3.Distance(player.transform.position, transform.position) < range)
            {
                if (raycastOn && !Physics2D.Linecast(transform.position, player.transform.position, _ray.collisionMask))
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
            if (state == ai_state.groundpatrol || state == ai_state.chase) { return true; }
            _pathAgent.CancelPathing();
            return false;
        }

        public void GetInput(ref Vector3 velocity, ref Vector2 input, ref bool jumpRequest)
        {

            switch (state)
            {
                case ai_state.none: break;
                case ai_state.groundpatrol: GroundPatrol(ref input); break;
                case ai_state.chase: Chase(); break; //add this line in to the GetInput method
                case ai_state.attack: AttackOnRange(); break;
                default: break;
            }

            if (state == ai_state.chase || state == ai_state.groundpatrol)
            {
                _pathAgent.AiMovement(ref velocity, ref input, ref jumpRequest);
            }
        }

        /*Destroy object on lateupdate to avoid warning errors of objects not existing*/
        void LateUpdate()
        {
            if (_destroy) { Destroy(gameObject); }
        }

        private void Chase()
        {
            if (!PlayerInRange(30f, false)) //Change boolean to true for OnSight aggro / 6f-30f
        {
                state = ai_state.groundpatrol;
                return;
            }
            if (PlayerInRange(5f, true)) //Change boolean to true for OnSight aggro / 1f-5f
            {
                state = ai_state.attack;
                return;
            }

            _pathAgent.pathfindingTarget = player;
            state = ai_state.chase;
        }

        private void GroundPatrol(ref Vector2 input)
        {
            //Switch to chase if player in range
            if (PlayerInRange(30f, true)) //Change boolean to true for OnSight aggro / 6f-30f
            {
                state = ai_state.chase;
                return;
            }

            //Coroutine for changing targeted platform
            if (_timerChangeTarget == false)
            {
                StartCoroutine(TimerForSwitchTarget());
            }
        }

        //Coroutine to switch tile/node target
        IEnumerator TimerForSwitchTarget()
        {
            _timerChangeTarget = true;
            yield return new WaitForSeconds(5f);
            _pathAgent.pathfindingTarget = _pathScript.GroundNodes[Random.Range(0, _pathScript.GroundNodes.Count)].gameObject;
            _pathAgent.RequestPath(_pathAgent.pathfindingTarget.transform.position + new Vector3(0,3,0));
            //Debug.Log(_pathAgent.pathfindingTarget.transform.position + new Vector3(0,3,0));
            _timerChangeTarget = false;
        }

        private void AttackOnRange()
        {
            if (!PlayerInRange(5f, true)) //Change boolean to true for OnSight aggro / 1f-5f
        {
                state = ai_state.chase;
                return;
            }
            //Insert attack behavior
        }
    }