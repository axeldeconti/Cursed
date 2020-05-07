﻿using Cursed.Character;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.AI
{
    public class PathfindingAgent : MonoBehaviour
    {
        public static Pathfinding _pathfindingMgr;

        private AiController _aiController = null;
        private CollisionHandler _col = null;

        /// <summary>
        /// Target to follow and chase
        /// </summary>
        [SerializeField] private GameObject _target;

        /// <summary>
        /// If follow target is this distance away, we start following
        /// </summary>
        [SerializeField] private float _followDistance = 0.5f;
        [SerializeField] private bool _debugBool = false; /*Very expensive if multiple characters have this enabled*/
        [SerializeField] private bool _drawPath = false; /*Very expensive if multiple characters have this enabled*/
        [SerializeField] private Color _drawColor = Color.red;
        [SerializeField] private bool _endDrawComplete = true;

        /// <summary>
        /// Ensures starting position is grounded at the correct location.
        /// </summary>
        private bool _useStored = false;
        private Vector3 _storePoint;

        //Pathfinding
        private LineRenderer _pathLineRenderer;
        /// <summary>
        /// Index of the current order in the current orders list
        /// </summary>
        private int _orderNum = -1;
        /// <summary>
        /// Current orders that are beeing followed
        /// </summary>
        private List<instructions> _currentOrders = new List<instructions>();
        /// <summary>
        /// Storage for the path until we're ready to use it
        /// </summary>
        private List<instructions> _waitingOrders = null;
        /// <summary>
        /// Last order given from the last path request
        /// </summary>
        private Vector3 _lastOrder;
        /// <summary>
        /// Is the current path dirty or not
        /// </summary>
        private bool _pathIsDirty = false;
        /// <summary>
        /// Distance with the current order
        /// </summary>
        private float _oldDistance;
        private int _newPathAttempts = 3;
        private int _newPathAttemptCount = 0;
        /// <summary>
        /// Number of failed attempts before the path is considered dirty
        /// </summary>
        private int _failAttempts = 3;
        /// <summary>
        /// Current number of failed attempts
        /// </summary>
        private int _failAttemptCount = 0;

        //Timers
        [SerializeField] private float followPathTimer = 0.5f;
        private float _fFollowPathTimer;
        [SerializeField] private float pathFailTimer = 0.25f;
        private float _fPathFailTimer;

        //AI
        [System.NonSerialized]
        public float lastPointRandomAccuracy = 0.2f;
        public static float pointAccuracy = 0.18f;

        [System.NonSerialized]
        public bool pathCompleted = true;

        private bool _stopPathing = true;
        private bool _hasLastOrder = false;
        private bool _aiJumped = false; //Is AI actually in jump

        private void Awake()
        {
            if (_pathfindingMgr == null)
                _pathfindingMgr = Pathfinding.Instance;

            _aiController = GetComponent<AiController>();
            _col = GetComponent<CollisionHandler>();

            if (_drawPath)
                AddLineRenderer();
        }

        private void Start()
        {
            //CursedDebugger.Instance.Add("GoTarget", () => _target.ToString());
            //CursedDebugger.Instance.Add("VectorTarget", () => _lastOrder.ToString());
        }

        private void Update()
        {
            //Applying new paths when character is ready

            if (_useStored)
                RequestPath(_storePoint);

            //Only receive orders if we're grounded, so we don't accidentally fall off a ledge mid-jump.
            if (_waitingOrders != null && _col.OnGround)
            {
                //Check if the AI controller need a pathfinding
                if (_aiController.NeedsPathfinding())
                {
                    _currentOrders = _waitingOrders;
                    _waitingOrders = null;
                    pathCompleted = false;
                    _stopPathing = false;

                    if (!_target)
                        _hasLastOrder = true;

                    _newPathAttemptCount = 0;
                    _failAttemptCount = 0;
                    _orderNum = 0;

                    if (_currentOrders != null && _orderNum < _currentOrders.Count - 1) //used for DirtyPath
                        _oldDistance = Vector3.Distance(transform.position, _currentOrders[_orderNum].pos);

                    //If character is nowhere near starting node, we try to salvage the path by picking the nearest node and setting it as the start.
                    if (Vector3.Distance(transform.position, _currentOrders[0].pos) > 2f)
                    {
                        float closest = float.MaxValue;
                        for (int i = 0; i < _currentOrders.Count; i++)
                        {
                            float distance = Vector3.Distance(_currentOrders[i].pos, transform.position);
                            if ((_currentOrders[i].order == "walkable") && distance < closest)
                            {
                                closest = distance;
                                _orderNum = i;
                            }
                        }
                    }

                    //If possible, we skip the first node, this prevents that character from walking backwards to first node.
                    if (_currentOrders.Count > _orderNum + 1 && _currentOrders[_orderNum].order == _currentOrders[_orderNum + 1].order &&
                        (_currentOrders[_orderNum].order == "walkable"))
                    {
                        _orderNum++;
                    }

                    //Add Random deviation to last node position to stagger paths (Staggers character positions / Looks better.)
                    if (_currentOrders.Count - 1 > 0 && _currentOrders[_currentOrders.Count - 1].order == "walkable")
                    {
                        _currentOrders[_currentOrders.Count - 1].pos.x += Random.Range(-1, 1) * lastPointRandomAccuracy;
                    }

                    PathStarted();
                }
            }
        }

        private void FixedUpdate()
        {
            //Requesting new path timers

            //Update Follow/Chase Path
            if (_target)
            {
                _fFollowPathTimer += Time.deltaTime;
                if (_fFollowPathTimer >= followPathTimer)
                {
                    _fFollowPathTimer = 0f;
                    //If need a new path (target not close to last order || no orders)
                    if ((_currentOrders != null && _currentOrders.Count > 0 && Vector3.Distance(_currentOrders[_currentOrders.Count - 1].pos, _target.transform.position) > _followDistance)
                            || _currentOrders == null || _currentOrders.Count == 0)
                    {
                        //If not close enough from target
                        if (Vector3.Distance(transform.position, _target.transform.position) > _followDistance + 0.18f)
                            _pathIsDirty = true;
                    }
                }
            }

            //Unable to make progress on current path, Update Path
            if (!pathCompleted)
            {
                _fPathFailTimer += Time.deltaTime;

                //If it's time to check
                if (_fPathFailTimer > pathFailTimer)
                {
                    _fPathFailTimer = 0;

                    //If there are still orders left
                    if (_currentOrders != null && _currentOrders.Count > _orderNum)
                    {
                        float newDistance = Vector3.Distance(transform.position, _currentOrders[_orderNum].pos);
                        //If current distance is greater or equal to the old distance
                        if (_oldDistance <= newDistance)
                        {
                            _failAttemptCount++;
                            //If tried enough times
                            if (_failAttemptCount >= _failAttempts && _col.OnGround)
                            {
                                _failAttemptCount = 0;
                                _pathIsDirty = true;
                            }
                        }
                        else 
                            _failAttemptCount = 0;

                        _oldDistance = newDistance;
                    }
                }
            }

            //If path is dirty, request new path;
            if (_pathIsDirty)
            {
                _pathIsDirty = false;
                if (_target && _aiController.State == AIState.Chase)
                    RequestPath(_target);
                else if (_hasLastOrder && _aiController.State == AIState.GroundPatrol)
                    RequestPath(_lastOrder);

                if (_debugBool)
                    Log("Path is dirty");
            }
        }

        public void CancelPathing()
        {
            if (_debugBool)
                Log("path canceled");

            if (_endDrawComplete && _pathLineRenderer)
                _pathLineRenderer.positionCount = (1);

            //Remove orders && Prevent pathfinding
            _hasLastOrder = false;
            _currentOrders = null;
            _stopPathing = true;
        }

        /// <summary>
        /// Called to see pathing on screen (when path start)
        /// </summary>
        private void PathStarted()
        {
            if (_debugBool)
                Log("Path started");

            if (_drawPath)
            {
                if (!_pathLineRenderer)
                    AddLineRenderer();

                _pathLineRenderer.startColor = (_drawColor);
                _pathLineRenderer.endColor = (_drawColor);
                _pathLineRenderer.positionCount = (_currentOrders.Count);

                for (int i = 0; i < _currentOrders.Count; i++)
                {
                    _pathLineRenderer.SetPosition(i, new Vector3(_currentOrders[i].pos.x, _currentOrders[i].pos.y, 0));
                }

            }

            if (!_drawPath && _pathLineRenderer)
                Destroy(gameObject.GetComponent<LineRenderer>());
        }

        /// <summary>
        /// Called when the path has ended correctly (destination reached)
        /// </summary>
        private void PathCompleted()
        {
            if (_debugBool)
                Log("Path completed");

            if (!_drawPath && _pathLineRenderer)
                Destroy(gameObject.GetComponent<LineRenderer>());

            CancelPathing(); //Reset Variables && Clears the debugging gizmos from drawing
        }

        /// <summary>
        /// Called if destination is unreachable
        /// </summary>
        private void PathNotFound()
        {
            if (_debugBool)
                Log("Path not found");

            _newPathAttemptCount++;
            if (_newPathAttemptCount >= _newPathAttempts)
            {
                CancelPathing();

                if (_debugBool)
                    Log("Newpath attempt limit reached. cancelling path.");
            }
        }

        /// <summary>
        /// Used for refreshing paths, example : Chase behaviour 
        /// </summary>
        public int GetNodesFromCompletion()
        {
            if (_currentOrders == null)
                return 0;

            int r = _currentOrders.Count - _orderNum;
            return r;
        }

        /// <summary>
        /// Request path towards Vector3
        /// </summary>
        /// <param name="pathVector">Target</param>
        public void RequestPath(Vector3 pathVector)
        {
            if (_col.OnGround)
            {
                _useStored = false;
                if (_debugBool)
                    Log("Requeseting path vector");

                _lastOrder = pathVector;
                _pathfindingMgr.RequestPathInstructions(this, _lastOrder, 20f //JumpHeight
                    , true //Movement        //All booleans tells if AI can use the capacity
                    , true //Jump
                    , true //Fall
                    );
            }
            else
            {
                _useStored = true;
                _storePoint = pathVector;
            }
        }

        /// <summary>
        /// Request path towards GameObject
        /// </summary>
        /// <param name="go">Target</param>
        public void RequestPath(GameObject go)
        {
            _target = go;

            RequestPath(go.transform.position);

            //if (_col.OnGround)
            //{
            //    if (debugBool)
            //        Log("Requesting path target");

            //    _pathfindingMgr.RequestPathInstructions(this, _target.transform.position, 20f //JumpHeight
            //        , true //Same as RequestPath(Vector3 pathVector)
            //        , true
            //        , true
            //        );
            //}
        }

        /// <summary>
        /// Callback from Thread with path information
        /// </summary>
        public void ReceivePathInstructions(List<instructions> instr, bool passed)
        {
            //Passed == false means incompleted / failure to reach node destination
            if (!passed)
            {
                PathNotFound();
                return;
            }

            _waitingOrders = instr; //Storage for the path until we're ready to use it
        }

        public void AiMovement(ref Vector2 input, ref bool jumpRequest)
        {
            bool orderComplete = false;
            if (!_stopPathing && _currentOrders != null && _orderNum < _currentOrders.Count)
            {
                //Set input to 1 or -1 if no need to jump
                if (_currentOrders[_orderNum].order != "jump")
                    input.x = transform.position.x > _currentOrders[_orderNum].pos.x ? -1 : 1;

                //Prevent overshooting jumps and moving backwards & overcorrecting
                if (_orderNum - 1 > 0 
                    && (_currentOrders[_orderNum - 1].order == "jump" || _currentOrders[_orderNum - 1].order == "fall") 
                    && transform.position.x + 0.18f > _currentOrders[_orderNum].pos.x 
                    && transform.position.x - pointAccuracy < _currentOrders[_orderNum].pos.x)
                {
                    //velocity.x = 0f;
                    input.x = 0;
                    transform.position = new Vector3(Mathf.Lerp(transform.position.x, _currentOrders[_orderNum].pos.x, 0.2f), transform.position.y, transform.position.z);
                }

                //Match X position of node (Ground, Fall)
                if (_currentOrders[_orderNum].order != "jump"
                    && transform.position.x + pointAccuracy > _currentOrders[_orderNum].pos.x
                    && transform.position.x - pointAccuracy < _currentOrders[_orderNum].pos.x)
                {
                    input.x = 0f;
                    if (transform.position.y + 0.866f > _currentOrders[_orderNum].pos.y
                    && transform.position.y - 0.866f < _currentOrders[_orderNum].pos.y)
                    {
                        //If next node is a jump, remove velocity.x, and lerp position to point.
                        if (_orderNum + 1 < _currentOrders.Count && _currentOrders[_orderNum + 1].order == "jump")
                        {
                            //velocity.x *= 0.0f;
                            transform.position = new Vector3(Mathf.Lerp(transform.position.x, _currentOrders[_orderNum].pos.x, 0.2f), transform.position.y, transform.position.z);
                        }

                        //If last node was a jump, and next node is a fall, remove velocity.x, and lerp position to point
                        if (_orderNum + 1 < _currentOrders.Count && _orderNum - 1 > 0 && _currentOrders[_orderNum + 1].order == "fall" && _currentOrders[_orderNum + -1].order == "jump")
                        {
                            //velocity.x *= 0.0f;
                            transform.position = new Vector3(Mathf.Lerp(transform.position.x, _currentOrders[_orderNum].pos.x, 0.5f), transform.position.y, transform.position.z);
                        }

                        orderComplete = true;
                    }
                }

                //Jump
                if (_currentOrders[_orderNum].order == "jump" && !_aiJumped && _col.OnGround)
                {
                    jumpRequest = true;
                    _aiJumped = true;
                    //Pourquoi ? Why ? Maybe not to jump when the ai needs to go through a jump node without jumping
                    if (_orderNum + 1 < _currentOrders.Count && Mathf.Abs(_currentOrders[_orderNum + 1].pos.x - _currentOrders[_orderNum].pos.x) > 1f)
                    {
                        orderComplete = true;
                        _aiJumped = false;
                    }
                }
                else if (_aiJumped && transform.position.y + 1f > _currentOrders[_orderNum].pos.y && transform.position.y - 1f < _currentOrders[_orderNum].pos.y)
                {
                    //Ai has jumped, go to next order
                    orderComplete = true;
                    _aiJumped = false;
                }

                //Next order!
                if (orderComplete)
                {
                    _orderNum++;

                    //Used for DirtyPath
                    if (_orderNum < _currentOrders.Count - 1)
                        _oldDistance = Vector3.Distance(transform.position, _currentOrders[_orderNum].pos);

                    if (_orderNum >= _currentOrders.Count)
                    {
                        //velocity.x = 0;
                        input.x = 0;
                        //Carry out orders when the node is finally reached...
                        PathCompleted();
                    }
                }
            }
        }

        #region Debugging && visuals

        private void OnDrawGizmos()
        {
            if (_currentOrders != null && !pathCompleted && _debugBool)
            {
                for (int i = 0; i < _currentOrders.Count; i++)
                {

                    if (i == _orderNum)
                    {
                        Gizmos.color = Color.cyan;
                    }
                    else
                    {
                        if (i == 0) { Gizmos.color = Color.green; }
                        else if (i == _currentOrders.Count - 1) { Gizmos.color = Color.red; }
                        else
                        {
                            Gizmos.color = Color.gray;
                        }
                    }
                    Gizmos.DrawSphere(_currentOrders[i].pos, 0.11f);
                    if (i + 1 < _currentOrders.Count)
                    {
                        if (i - 1 == _orderNum || i == _orderNum) { Gizmos.color = Color.red; } else { Gizmos.color = Color.gray; }

                        Gizmos.DrawLine(_currentOrders[i].pos, _currentOrders[i + 1].pos);
                    }
                }
            }
        }

        private void AddLineRenderer()
        {
            if (!GetComponent<LineRenderer>())
            {
                _pathLineRenderer = gameObject.AddComponent<LineRenderer>();
                _pathLineRenderer.materials[0] = (Material)Resources.Load("Sprite/Default", typeof(Material));
                _pathLineRenderer.materials[0].shader = Shader.Find("Sprites/Default");
                _pathLineRenderer.positionCount = (1);
                _pathLineRenderer.startWidth = (0.5f);
                _pathLineRenderer.endWidth = (0.5f);
            }
        }

        private void Log(string log)
        {
            Debug.Log("[PFAgent] " + gameObject.name + " : " + log);
        }

        #endregion

        #region Getters & Setters

        public GameObject Target
        {
            get => _target;
            set => _target = value;
        }

        #endregion
    }
}