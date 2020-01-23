using System.Collections.Generic;
using UnityEngine;

public class PathfindingAgent : MonoBehaviour
{

    [System.NonSerialized]
    public static Pathfinding _pathfindingManagerScript;
    private Character _characterScript;
    private CharacterController2D _controller;
    private AiController _aiControllerScript;

    public GameObject pathfindingTarget; /*Following / Chasing - overrides Vector3 Pathfinding*/

    public float followDistance = 0.5f; //if follow target is this distance away, we start following
    public bool debugBool = false; /*Very expensive if multiple characters have this enabled*/
    public bool drawPath = false; /*Very expensive if multiple characters have this enabled*/
    public Color drawColor = Color.red;
    public bool endDrawComplete = true;

    //Ensures starting position is grounded at the correct location.
    private bool _useStored = false;
    private Vector3 _storePoint;
    private GameObject storeObject;

    //Pathfinding
    public bool repathOnFail = true;

    private LineRenderer _pathLineRenderer;
    private int _orderNum = -1;
    private List<instructions> _currentOrders = new List<instructions>();
    private List<instructions> _waitingOrders = null;
    private Vector3 _lastOrder;
    private bool _pathIsDirty = false;
    private float _oldDistance;
    private int _newPathAttempts = 3;
    private int _newPathAttemptCount = 0;
    private int _failAttempts = 3;
    private float _failAttemptCount = 0;

    //Timers
    public float followPathTimer = 0.5f;
    private float _fFollowPathTimer;
    public float pathFailTimer = 0.25f;
    private float _fPathFailTimer;
    private float _oneWayTimer = 0.1f;
    private float _fOneWayTimer;

    //AI
    [System.NonSerialized]
    public float lastPointRandomAccuracy = 0.2f;
    public static float pointAccuracy = 0.18f;

    [System.NonSerialized]
    public bool pathCompleted = true;
    [System.NonSerialized]
    public bool isPathFinding = false;

    private bool _stopPathing = true;
    private bool _hasLastOrder = false;
    private bool _aiJumped = false; //Is AI actually in jump
    private bool _onewayDropDown = false;
    private bool _onewayGrounded = false;

    //Get Components
    private void Awake()
    {

        if (_pathfindingManagerScript == null) { _pathfindingManagerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<Pathfinding>(); }
        _aiControllerScript = GetComponent<AiController>();
        _characterScript = GetComponent<Character>();
        _controller = GetComponent<CharacterController2D>();
        if (drawPath)
        {
            AddLineRenderer();
        }
    }

    private void Start()
    {
        CursedDebugger.Instance.Add("GoTarget", () => pathfindingTarget.ToString());
        CursedDebugger.Instance.Add("VectorTarget", () => _lastOrder.ToString());
    }

    public void CancelPathing()
    {
        if (debugBool) { Debug.Log("path canceled"); }
        if (endDrawComplete && _pathLineRenderer) { _pathLineRenderer.positionCount = (1); }
        //Remove orders && Prevent pathfinding
        _hasLastOrder = false;
        _currentOrders = null;
        isPathFinding = false;
        _stopPathing = true;
        //Your Code: OR keep custom AI in the AI-CONTROLLER
    }

    private void PathStarted()
    {
        if (debugBool) { Debug.Log("path started"); }

        if (drawPath)
        {
            if (!_pathLineRenderer) { AddLineRenderer(); }
            _pathLineRenderer.startColor = (drawColor);
            _pathLineRenderer.endColor = (drawColor);
            _pathLineRenderer.positionCount = (_currentOrders.Count);
            for (int i = 0; i < _currentOrders.Count; i++)
            {

                _pathLineRenderer.SetPosition(i, new Vector3(_currentOrders[i].pos.x, _currentOrders[i].pos.y, 0));
            }

        }
        if (!drawPath && _pathLineRenderer) { Destroy(gameObject.GetComponent<LineRenderer>()); }
        //Path has started
        //Your Code: OR keep custom AI in the AI-CONTROLLER

        _aiControllerScript.PathStarted();
    }


    private void PathCompleted()
    {
        if (debugBool) { Debug.Log("path completed"); }
        if (!drawPath && _pathLineRenderer) { Destroy(gameObject.GetComponent<LineRenderer>()); }
        CancelPathing(); //Reset Variables && Clears the debugging gizmos from drawing
        //Path was completed
        //Your Code: OR keep custom AI in the AI-CONTROLLER
        _aiControllerScript.PathCompleted();
    }

    private void PathNotFound()
    {
        if (debugBool) { Debug.Log("path not found"); }
        _newPathAttemptCount++;
        if (_newPathAttemptCount >= _newPathAttempts)
        {
            CancelPathing(); if (debugBool) { Debug.Log("newpath attempt limit reached. cancelling path."); }
        }
        //TODO: Attempt to get as close as possible to our goal even though no path was found?

        //No Path Found
        //Your Code: OR keep custom AI in the AI-CONTROLLER
    }

    //Used for refreshing paths, example: Flee behaviour 
    public int GetNodesFromCompletion()
    {
        if (_currentOrders == null) { return 0; }
        int r = _currentOrders.Count - _orderNum;
        return r;
    }

    //Request path towards Vector3
    public void RequestPath(Vector3 pathVector)
    {

        if (_controller.collisions.below)
        {
            _useStored = false;
            if (debugBool) { Debug.Log("requeseting path vector"); }
            _lastOrder = pathVector;
            _pathfindingManagerScript.RequestPathInstructions(gameObject, _lastOrder, _characterScript.jump.maxJumpHeight
                , _characterScript.movement.ability
                , _characterScript.jump.ability
                , _characterScript.FallNodes
                );
        }
        else
        {
            _useStored = true;
            _storePoint = pathVector;
        }
    }

    //Request path towards GameObject
    public void RequestPath(GameObject Go)
    {
        pathfindingTarget = Go;
        if (_controller.collisions.below)
        {
            if (debugBool) { Debug.Log("requeseting path target"); }
            _pathfindingManagerScript.RequestPathInstructions(gameObject, pathfindingTarget.transform.position, _characterScript.jump.maxJumpHeight
                , _characterScript.movement.ability
                , _characterScript.jump.ability
                , _characterScript.FallNodes
                );
        }
    }
    
    //Callback from Thread with path information
    public void ReceivePathInstructions(List<instructions> instr, bool passed)
    {

        //Passed == false means incompleted / failure to reach node destination
        if (!passed) { PathNotFound(); return; }
        _waitingOrders = instr; //Storage for the path until we're ready to use it
    }

    //AI Movement /*TODO: Cleanup!*/
    public void AiMovement(ref Vector3 velocity, ref Vector2 input, ref bool jumpRequest)
    {
        bool orderComplete = false;
        if (!_stopPathing && _currentOrders != null && _orderNum < _currentOrders.Count)
        {

            if (_characterScript.ledgegrab.ledgeGrabbed) { _characterScript.ledgegrab.StopLedgeGrab(); } //temporary disable ledgegrabbing for ai
                                                                                                         //find direction to travel
            if (_currentOrders[_orderNum].order != "jump") { input.x = transform.position.x > _currentOrders[_orderNum].pos.x ? -1 : 1; }
            if (_currentOrders[_orderNum].order == "climb") { input.y = transform.position.y > _currentOrders[_orderNum].pos.y ? -1 : 1; }

            //prevent overshooting jumps and moving backwards & overcorrecting
            if (_orderNum - 1 > 0 && (_currentOrders[_orderNum - 1].order == "jump" || _currentOrders[_orderNum - 1].order == "fall") && transform.position.x + 0.18f > _currentOrders[_orderNum].pos.x &&
                transform.position.x - pointAccuracy < _currentOrders[_orderNum].pos.x)
            {
                velocity.x = 0f;
                transform.position = new Vector3(Mathf.Lerp(transform.position.x, _currentOrders[_orderNum].pos.x, 0.2f), transform.position.y, transform.position.z);
            }

            //climbing
            if (_currentOrders[_orderNum].order == "climb" && transform.position.x + pointAccuracy > _currentOrders[_orderNum].pos.x && transform.position.x - pointAccuracy < _currentOrders[_orderNum].pos.x)
            {
                //if last node is ground node that was switched to climbing, path completed
                if (_orderNum == _currentOrders.Count - 1) { orderComplete = true; }
                if (transform.position.y + pointAccuracy > _currentOrders[_orderNum].pos.y && transform.position.y - pointAccuracy < _currentOrders[_orderNum].pos.y)
                {

                    if (_orderNum == _currentOrders.Count - 1) { orderComplete = true; }

                    if (_orderNum + 1 < _currentOrders.Count && _currentOrders[_orderNum + 1].order == "climb")
                    {
                        orderComplete = true;
                    }
                }
                if (_orderNum + 1 < _currentOrders.Count && _currentOrders[_orderNum + 1].order != "climb")
                {
                    input.y = _currentOrders[_orderNum + 1].pos.y < _currentOrders[_orderNum].pos.y ? -1 : 1;

                }
            }

            //match X position of node (Ground, Fall)
            if (_currentOrders[_orderNum].order != "jump" && _currentOrders[_orderNum].order != "climb"
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
                    //   ******TEMP FIX FOR fall nodes on oneways
                    if (_currentOrders[_orderNum].order == "fall")
                    {
                        input.y = -1;
                    }


                    if (_currentOrders[_orderNum].order != "jump")
                    {
                        orderComplete = true;
                    }

                }
            }
            //Jump
            if (_currentOrders[_orderNum].order == "jump" && !_aiJumped && _controller.collisions.below)
            {
                //velocity.y = characterScript.jumpVelocity;
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

            //oneway nodes
            if (_orderNum > 0 && (_currentOrders[_orderNum - 1].order == "fall" || _currentOrders[_orderNum - 1].order == "jump"))
            {
                if (_controller.collisions.below)
                {

                    _onewayGrounded = true;
                    if (_onewayGrounded && _onewayDropDown)
                    {
                        input.y = -1;
                        _onewayGrounded = false;
                    }

                }
            }

            //next order!
            if (orderComplete)
            {

                _orderNum++;

                _onewayGrounded = false; //oneway detection
                _onewayDropDown = false;
                _fOneWayTimer = 0;

                if (_orderNum < _currentOrders.Count - 1)
                { //used for DirtyPath
                    _oldDistance = Vector3.Distance(transform.position, _currentOrders[_orderNum].pos);
                }

                if (_orderNum >= _currentOrders.Count)
                {

                    velocity.x = 0;
                    //Carry out orders when the node is finally reached...
                    PathCompleted();
                }
            }
        }
    }

    //Applying new paths when character is ready
    void Update()
    {

        if (_useStored)
        {
            RequestPath(_storePoint);
        }

        //Only recieve orders if we're grounded, so we don't accidentally fall off a ledge mid-jump.
        if (_waitingOrders != null && (_controller.collisions.below))
        {
            if (_aiControllerScript.NeedsPathfinding())
            {
                isPathFinding = true;
                _currentOrders = _waitingOrders;
                _waitingOrders = null;
                pathCompleted = false;
                _stopPathing = false;
                if (!pathfindingTarget)
                {
                    _hasLastOrder = true;
                }

                _newPathAttemptCount = 0;
                _orderNum = 0;

                _failAttemptCount = 0;
                if (_currentOrders != null && _orderNum < _currentOrders.Count - 1)
                { //used for DirtyPath
                    _oldDistance = Vector3.Distance(transform.position, _currentOrders[_orderNum].pos);
                }
                //If character is nowhere near starting node, we try to salvage the path by picking the nearest node and setting it as the start.
                if (Vector3.Distance(transform.position, _currentOrders[0].pos) > 2f)
                {
                    float closest = float.MaxValue;
                    for (int i = 0; i < _currentOrders.Count; i++)
                    {
                        float distance = Vector3.Distance(_currentOrders[i].pos, transform.position);
                        if ((_currentOrders[i].order == "walkable" || _currentOrders[i].order == "climb") && distance < closest)
                        {
                            closest = distance;
                            _orderNum = i;
                        }
                    }
                }
                //If possible, we skip the first node, this prevents that character from walking backwards to first node.
                if (_currentOrders.Count > _orderNum + 1 && _currentOrders[_orderNum].order == _currentOrders[_orderNum + 1].order &&
                    (_currentOrders[_orderNum].order == "walkable" || _currentOrders[_orderNum].order == "climb"))
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

    //Requesting new path timers
    private void FixedUpdate()
    {

        if (_onewayGrounded)
        {
            _fOneWayTimer += Time.deltaTime;
            if (_fOneWayTimer >= _oneWayTimer)
            {
                _onewayDropDown = true;
                _fOneWayTimer = 0;
            }
        }

        //Update Follow/Chase Path
        if (pathfindingTarget)
        {
            _fFollowPathTimer += Time.deltaTime;
            if (_fFollowPathTimer >= followPathTimer)
            {
                _fFollowPathTimer = 0f;
                if ((_currentOrders != null && _currentOrders.Count > 0 && Vector3.Distance(_currentOrders[_currentOrders.Count - 1].pos, pathfindingTarget.transform.position) > followDistance)
                    || ((_currentOrders == null || _currentOrders.Count == 0)))
                {
                    if (Vector3.Distance(transform.position, pathfindingTarget.transform.position) > followDistance + 0.18f)
                        _pathIsDirty = true;
                }
            }
        }
        //Unable to make progress on current path, Update Path

        if (repathOnFail && !pathCompleted)
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
                        if (_failAttemptCount >= _failAttempts && _controller.collisions.below)
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
            if (pathfindingTarget && _aiControllerScript.state == AiController.ai_state.chase ) 
            { RequestPath(pathfindingTarget); } 
            else if (_hasLastOrder && _aiControllerScript.state == AiController.ai_state.groundpatrol) 
            { RequestPath(_lastOrder); }

            if (debugBool)
            {
                Debug.Log("path is dirty");
            }
        }
    }

    //Debugging visuals
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
            _pathLineRenderer.startWidth = (0.1f);
            _pathLineRenderer.endWidth = (0.1f);
        }
    }
}