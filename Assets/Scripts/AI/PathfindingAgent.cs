using Cursed.Character;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.AI
{
    public class PathfindingAgent : MonoBehaviour
    {
        public static Pathfinding _pathfindingMgr;

        private AiController _aiController = null;
        private CollisionHandler _col = null;

        [SerializeField] private GameObject _target; //Following / Chasing

        [SerializeField] private float followDistance = 0.5f; //if follow target is this distance away, we start following
        [SerializeField] private bool debugBool = false; /*Very expensive if multiple characters have this enabled*/
        [SerializeField] private bool drawPath = false; /*Very expensive if multiple characters have this enabled*/
        [SerializeField] private Color drawColor = Color.red;
        [SerializeField] private bool endDrawComplete = true;

        //Ensures starting position is grounded at the correct location.
        private bool _useStored = false;
        private Vector3 _storePoint;

        //Pathfinding
        private LineRenderer _pathLineRenderer;
        private int _orderNum = -1;
        private List<instructions> _currentOrders = new List<instructions>();
        private List<instructions> _waitingOrders = null; //Storage for the path until we're ready to use it
        private Vector3 _lastOrder;
        private bool _pathIsDirty = false;
        private float _oldDistance;
        private int _newPathAttempts = 3;
        private int _newPathAttemptCount = 0;
        private int _failAttempts = 3;
        private float _failAttemptCount = 0;

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

            if (drawPath)
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
                if (_aiController.NeedsPathfinding())
                {
                    _currentOrders = _waitingOrders;
                    _waitingOrders = null;
                    pathCompleted = false;
                    _stopPathing = false;

                    if (!_target)
                        _hasLastOrder = true;

                    _newPathAttemptCount = 0;
                    _orderNum = 0;

                    _failAttemptCount = 0;
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
                        _orderNum += 1;
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
                    if ((_currentOrders != null && _currentOrders.Count > 0 && Vector3.Distance(_currentOrders[_currentOrders.Count - 1].pos, _target.transform.position) > followDistance)
                            || ((_currentOrders == null || _currentOrders.Count == 0)))
                    {
                        if (Vector3.Distance(transform.position, _target.transform.position) > followDistance + 0.18f)
                            _pathIsDirty = true;
                    }
                }
            }

            //Unable to make progress on current path, Update Path
            if (!pathCompleted)
            {
                _fPathFailTimer += Time.deltaTime;

                if (_fPathFailTimer > pathFailTimer)
                {
                    _fPathFailTimer = 0;

                    if (_currentOrders != null && _currentOrders.Count > _orderNum)
                    {
                        float newDistance = Vector3.Distance(transform.position, _currentOrders[_orderNum].pos);
                        if (_oldDistance <= newDistance)
                        {
                            _failAttemptCount++;
                            if (_failAttemptCount >= _failAttempts && _col.OnGround)
                            {
                                _failAttemptCount = 0;
                                _pathIsDirty = true;
                            }
                        }
                        else { _failAttemptCount = 0; }
                        _oldDistance = newDistance;
                    }
                }
            }

            //If path is dirty, request new path;
            if (_pathIsDirty)
            {
                _pathIsDirty = false;
                if (_target && _aiController.State == AIState.Chase)
                { RequestPath(_target); }
                else if (_hasLastOrder && _aiController.State == AIState.GroundPatrol)
                { RequestPath(_lastOrder); }

                if (debugBool)
                    Log("Path is dirty");
            }
        }

        public void CancelPathing()
        {
            if (debugBool)
                Log("path canceled");

            if (endDrawComplete && _pathLineRenderer)
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
            if (debugBool)
                Log("Path started");

            if (drawPath)
            {
                if (!_pathLineRenderer)
                    AddLineRenderer();

                _pathLineRenderer.startColor = (drawColor);
                _pathLineRenderer.endColor = (drawColor);
                _pathLineRenderer.positionCount = (_currentOrders.Count);

                for (int i = 0; i < _currentOrders.Count; i++)
                {
                    _pathLineRenderer.SetPosition(i, new Vector3(_currentOrders[i].pos.x, _currentOrders[i].pos.y, 0));
                }

            }
            if (!drawPath && _pathLineRenderer)
                Destroy(gameObject.GetComponent<LineRenderer>());
        }

        /// <summary>
        /// Called when the path has ended correctly (destination reached)
        /// </summary>
        private void PathCompleted()
        {
            if (debugBool)
                Log("Path completed");

            if (!drawPath && _pathLineRenderer)
                Destroy(gameObject.GetComponent<LineRenderer>());

            CancelPathing(); //Reset Variables && Clears the debugging gizmos from drawing
        }

        /// <summary>
        /// Called if destination is unreachable
        /// </summary>
        private void PathNotFound()
        {
            if (debugBool)
                Log("Path not found");

            _newPathAttemptCount++;
            if (_newPathAttemptCount >= _newPathAttempts)
            {
                CancelPathing();

                if (debugBool)
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
                if (debugBool)
                    Log("Requeseting path vector");

                _lastOrder = pathVector;
                _pathfindingMgr.RequestPathInstructions(gameObject, _lastOrder, 20f //JumpHeight
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

            if (_col.OnGround)
            {
                if (debugBool)
                    Log("Requesting path target");

                _pathfindingMgr.RequestPathInstructions(gameObject, _target.transform.position, 20f //JumpHeight
                    , true //Same as RequestPath(Vector3 pathVector)
                    , true
                    , true
                    );
            }
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

        public void AiMovement(ref Vector3 velocity, ref Vector2 input, ref bool jumpRequest)
        {
            bool orderComplete = false;
            if (!_stopPathing && _currentOrders != null && _orderNum < _currentOrders.Count)
            {

                if (_currentOrders[_orderNum].order != "jump")
                    input.x = transform.position.x > _currentOrders[_orderNum].pos.x ? -1 : 1;

                //prevent overshooting jumps and moving backwards & overcorrecting
                if (_orderNum - 1 > 0 && (_currentOrders[_orderNum - 1].order == "jump" || _currentOrders[_orderNum - 1].order == "fall") && transform.position.x + 0.18f > _currentOrders[_orderNum].pos.x &&
                    transform.position.x - pointAccuracy < _currentOrders[_orderNum].pos.x)
                {
                    velocity.x = 0f;
                    transform.position = new Vector3(Mathf.Lerp(transform.position.x, _currentOrders[_orderNum].pos.x, 0.2f), transform.position.y, transform.position.z);
                }

                //match X position of node (Ground, Fall)
                if (_currentOrders[_orderNum].order != "jump"
                    && transform.position.x + pointAccuracy > _currentOrders[_orderNum].pos.x
                    && transform.position.x - pointAccuracy < _currentOrders[_orderNum].pos.x)
                {
                    input.x = 0f;
                    if (transform.position.y + 0.866f > _currentOrders[_orderNum].pos.y
                    && transform.position.y - 0.866f < _currentOrders[_orderNum].pos.y)
                    {
                        //if next node is a jump, remove velocity.x, and lerp position to point.
                        if (_orderNum + 1 < _currentOrders.Count && _currentOrders[_orderNum + 1].order == "jump")
                        {
                            velocity.x *= 0.0f;
                            transform.position = new Vector3(Mathf.Lerp(transform.position.x, _currentOrders[_orderNum].pos.x, 0.2f), transform.position.y, transform.position.z);
                        }
                        //if last node was a jump, and next node is a fall, remove velocity.x, and lerp position to point
                        if (_orderNum + 1 < _currentOrders.Count && _orderNum - 1 > 0 && _currentOrders[_orderNum + 1].order == "fall" && _currentOrders[_orderNum + -1].order == "jump")
                        {
                            velocity.x *= 0.0f;
                            transform.position = new Vector3(Mathf.Lerp(transform.position.x, _currentOrders[_orderNum].pos.x, 0.5f), transform.position.y, transform.position.z);
                        }
                        if (_currentOrders[_orderNum].order != "jump")
                        {
                            orderComplete = true;
                        }
                    }
                }

                //Jump
                if (_currentOrders[_orderNum].order == "jump" && !_aiJumped && _col.OnGround)
                {
                    jumpRequest = true;
                    _aiJumped = true;
                    if (_orderNum + 1 < _currentOrders.Count && Mathf.Abs(_currentOrders[_orderNum + 1].pos.x - _currentOrders[_orderNum].pos.x) > 1f)
                    {
                        orderComplete = true;
                        _aiJumped = false;
                    }
                }
                else if (_aiJumped && transform.position.y + 1f > _currentOrders[_orderNum].pos.y && transform.position.y - 1f < _currentOrders[_orderNum].pos.y)
                {
                    orderComplete = true;
                    _aiJumped = false;
                }

                //next order!
                if (orderComplete)
                {
                    _orderNum++;

                    if (_orderNum < _currentOrders.Count - 1)//used for DirtyPath
                        _oldDistance = Vector3.Distance(transform.position, _currentOrders[_orderNum].pos);

                    if (_orderNum >= _currentOrders.Count)
                    {
                        velocity.x = 0;
                        //Carry out orders when the node is finally reached...
                        PathCompleted();
                    }
                }
            }
        }

        #region Debugging && visuals

        private void OnDrawGizmos()
        {
            if (_currentOrders != null && !pathCompleted && debugBool)
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