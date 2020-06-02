using Cursed.Character;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Cursed.AI
{
    public class Pathfinding : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private LayerMask _collisionLayer;

        [SerializeField] private GameObject[] _maps;

        [Space]
        /// <summary>
        /// Each block is square. This should probably match your square 2dCollider on a tile.
        /// </summary>
        [SerializeField] private float _blockSize = 1f;
        /// <summary>
        /// The maximum jump height of a character
        /// </summary>
        //[SerializeField] private float _jumpHeight = 3.8f;
        /// <summary>
        /// The furthest a character can jump without momentum
        /// </summary>
        //[SerializeField] private float _maxJumpBlocksX = 3f;
        /// <summary>
        /// Normal jump data to place jump nodes
        /// </summary>
        [SerializeField] private JumpData _normalJump = null;
        [SerializeField] private float _jumpHeightIncrement;
        [SerializeField] private float _minimumJump;

        /// <summary>
        /// Percentage of blockSize (Determines height off ground level for a groundNode)
        /// </summary>
        [SerializeField] private float _groundNodeHeight = 0.01f;

        /// <summary>
        /// Percentage of blockSize (Determines max spacing allowed between two groundNodes)
        /// </summary>
        private float _groundMaxWidth = 0.35f;
        /// <summary>
        /// Percentage of blockSize (Determines space away from groundNode's side to place the fallNode)
        /// </summary>
        private float _fall_X_Spacing = 0.25f;
        /// <summary>
        /// Percentage of blockSize (Determines space away from groundNode's top to place the fallNode)
        /// </summary>
        private float _fall_Y_GrndDist = 0.02f;

        private Thread _t;

        private List<pathNode> _nodes = new List<pathNode>();
        private List<pathNode> _groundNodes = new List<pathNode>();

        [SerializeField] private List<ThreadLock> _orders = new List<ThreadLock>();
        [SerializeField] private List<ThreadLock> _readyOrders = new List<ThreadLock>();

        [SerializeField] private NodeWeight _nodeWeights;

        /// <summary>
        /// Pauses game on runtime and displays pathnode connections
        /// </summary>
        [SerializeField] private bool _debugTools = false;
        [SerializeField] private bool _debugLogs = false;

        private void Start()
        {
            _minimumJump = 1.8f;
            _jumpHeightIncrement = 2;

            CreateNodeMap();
        }

        private void Update()
        {
            DeliverPathfindingInstructions();
            MakeThreadDoWork();
        }

        private void CreateNodeMap()
        {
            _nodes = new List<pathNode>();
            _groundNodes = new List<pathNode>();

            List<GameObject> groundObjects = new List<GameObject>();

            //Find all children of tile parent
            GameObject currentMap;
            for (int i = 0; i < _maps.Length; i++)
            {
                currentMap = _maps[i];

                foreach (Transform child in currentMap.transform)
                {
                    if (1 << child.gameObject.layer == _groundLayer.value)
                    {
                        groundObjects.Add(child.gameObject);
                    }
                }
            }

            FindGroundNodes(groundObjects);
            FindFallNodes(_groundNodes);
            FindJumpNodes(_groundNodes);

            GroundNeighbors(_groundNodes, _groundNodes);
            JumpNeighbors(AttachedJumpNodes(_groundNodes), _groundNodes); //CHANGE this function to find all jump nodes attached to ground nodes **********TODO
            FallNeighbors(AttachedFallNodes(_groundNodes), _groundNodes);  //CHANGE this function to find all fall nodes attached to ground nodes **********TODO

            if (_debugTools)
                Debug.Break();
        }

        /// <summary>
        /// Request a new path
        /// </summary>
        /// <param name="agent">Agent asking for a path</param>
        /// <param name="target"></param>
        /// <param name="jumpH"></param>
        /// <param name="movement"></param>
        /// <param name="jump"></param>
        /// <param name="fall"></param>
        public void RequestPathInstructions(PathfindingAgent agent, Vector3 target, float jumpH, bool movement, bool jump, bool fall)
        {
            bool replaced = false;
            ThreadLock newLocker = new ThreadLock(agent, target, jumpH, movement, jump, fall);

            //Check if one order is present for this agent, if so, change it
            for (int i = 1; i < _orders.Count; i++)
            {
                if (_orders[i].agent == agent)
                {
                    _orders[i] = newLocker;
                    replaced = true;
                    break;
                }
            }

            if (!replaced)
                _orders.Add(newLocker);
        }

        /// <summary>
        /// Find a path
        /// </summary>
        /// <param name="threadLocker"></param>
        public void FindPath(object threadLocker)
        {
            ThreadLock a = (ThreadLock)threadLocker;
            Vector3 character = a.agentPos;
            Vector3 location = a.end;
            float characterJump = a.jump;

            List<Instructions> instr = new List<Instructions>();

            List<pathNode> openNodes = new List<pathNode>();
            List<pathNode> closedNodes = new List<pathNode>();
            List<pathNode> pathNodes = new List<pathNode>();

            ResetLists();

            pathNode startNode = new pathNode(OrderType.None, Vector3.zero);
            startNode = GetNearestGroundNode(character);

            pathNode endNode = GetNearestNode(location);

            //If a point couldnt be found or if character can't move cancel path
            if (endNode == null || startNode == null || !a.canMove)
            {
                a.passed = false;
                a.instr = instr;
                _readyOrders.Add(a);

                if (_debugLogs)
                {
                    string sn = startNode == null ? "null" : startNode.ToString();
                    string en = endNode == null ? "null" : endNode.ToString();
                    Log("Path canceled : start node = " + sn + " | end node = " + en + " | can move = " + a.canMove);
                }
                return;
            }

            startNode.g = 0;
            startNode.f = Vector3.Distance(startNode.pos, endNode.pos);

            openNodes.Add(startNode);


            pathNode currentNode = new pathNode(OrderType.None, Vector3.zero);
            while (openNodes.Count > 0)
            {
                float lowestScore = float.MaxValue;
                for (int i = 0; i < openNodes.Count; i++)
                {
                    if (openNodes[i].f < lowestScore)
                    {
                        currentNode = openNodes[i];
                        lowestScore = currentNode.f;
                    }
                }
                if (currentNode == endNode)
                {
                    closedNodes.Add(currentNode);
                    break;
                }
                else
                {
                    closedNodes.Add(currentNode);
                    openNodes.Remove(currentNode);

                    //Check if character can use this node
                    if (!currentNode.type.Equals(OrderType.Jump) || (currentNode.type.Equals(OrderType.Jump) &&
                        Mathf.Abs(currentNode.realHeight - characterJump) < _jumpHeightIncrement * 0.92) &&
                        characterJump <= currentNode.realHeight + _jumpHeightIncrement * 0.08)
                    {
                        for (int i = 0; i < currentNode.neighbours.Count; i++)
                        {
                            //Check if node can be used by character
                            if (!a.canJump && currentNode.neighbours[i].type.Equals(OrderType.Jump))
                                continue;
                            if (!a.canFall && currentNode.neighbours[i].type == OrderType.Fall)
                                continue;

                            if (currentNode.neighbours[i].parent == null)
                            {
                                currentNode.neighbours[i].g = currentNode.neighbours[i].c + currentNode.g;
                                currentNode.neighbours[i].h = Vector3.Distance(currentNode.neighbours[i].pos, endNode.pos);

                                if (currentNode.neighbours[i].type.Equals(OrderType.Jump))
                                    currentNode.neighbours[i].h += currentNode.neighbours[i].realHeight;

                                currentNode.neighbours[i].f = currentNode.neighbours[i].g + currentNode.neighbours[i].h;
                                currentNode.neighbours[i].parent = currentNode;
                                openNodes.Add(currentNode.neighbours[i]);
                            }
                            else
                            {
                                if (currentNode.g + currentNode.neighbours[i].c < currentNode.neighbours[i].g)
                                {
                                    currentNode.neighbours[i].g = currentNode.neighbours[i].c + currentNode.g;
                                    currentNode.neighbours[i].f = currentNode.neighbours[i].g + currentNode.neighbours[i].h;
                                    currentNode.neighbours[i].parent = currentNode;
                                }
                            }
                        }
                    }
                }
            }

            //Makes sure the path doesn't have a loop or something wrong and can return to the start or to a node without parent
            for (int i = 0; i < 1000; i++)
            {
                if (i > 800 && _debugLogs)
                    Log("Something's wrong");

                pathNodes.Add(currentNode);

                if (currentNode.parent != null)
                    currentNode = currentNode.parent;
                else
                    break;

                if (currentNode == startNode)
                {
                    pathNodes.Add(startNode);
                    break;
                }
            }

            //Mark the thread as passed
            if (pathNodes[0] != endNode)
                a.passed = false;
            else
                a.passed = true;

            //Reverse the pathNodes list to start at the begining
            pathNodes.Reverse();

            //Create all instructions
            for (int i = 0; i < pathNodes.Count; i++)
            {
                Instructions temp = new Instructions(pathNodes[i].pos, pathNodes[i].type);
                instr.Add(temp);
            }

            a.instr = instr;
            _readyOrders.Add(a);

            if (_debugLogs)
                Log("Path found");
        }

        public void DeliverPathfindingInstructions()
        {
            for (int i = 0; i < _readyOrders.Count; i++)
            {
                if (_readyOrders[i].agent)
                {
                    _readyOrders[i].agent.ReceivePathInstructions(_readyOrders[i].instr, _readyOrders[i].passed);
                }
            }
            _readyOrders = new List<ThreadLock>();
        }

        /// <summary>
        /// Set all node's parent to null
        /// </summary>
        private void ResetLists()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].parent = null;
            }
        }

        /// <summary>
        /// Used for runtime adding terrain block to pathfinding nodes
        /// </summary>
        public void CreateBlockCalled(Vector3 position)
        {

            GameObject newGameObject = (GameObject)Instantiate(Resources.Load("Tiles/GroundTile"), new Vector3(position.x, position.y, 0.0f), Quaternion.identity) as GameObject;
            newGameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/ground2", typeof(Sprite)) as Sprite;
            //add the ground node, 

            Vector3 ground = newGameObject.transform.position; ground.y += _blockSize * 0.5f + _blockSize * _groundNodeHeight;
            pathNode newGroundNode = new pathNode(OrderType.Walkable, ground);
            _groundNodes.Add(newGroundNode);

            newGroundNode.c = _nodeWeights.GetNodeWeightByOrder(newGroundNode.type);
            _nodes.Add(newGroundNode);

            newGroundNode.gameObject = newGameObject;

            //run it through the list

            RefreshAreaAroundBlock(newGameObject, false);
        }

        /// <summary>
        /// Used by creating and Removing pathfinding nodes
        /// </summary>
        public void RefreshAreaAroundBlock(GameObject go, bool blockRemoved)
        {
            List<pathNode> collect = new List<pathNode>();
            List<pathNode> largerCollect = new List<pathNode>();
            float searchSize = 4.2f;
            float largerSearchSize = 9f;

            //we remove all node connections related to the destroyed block... if a block needs to be removed...
            if (blockRemoved)
            {
                for (int i = 0; i < _groundNodes.Count; i++)
                {
                    if (_groundNodes[i].gameObject == go)
                    {

                        while (_groundNodes[i].createdJumpNodes.Count > 0)
                        {

                            _nodes.Remove(_groundNodes[i].createdJumpNodes[0]);
                            _groundNodes[i].createdJumpNodes.RemoveAt(0);
                        }
                        while (_groundNodes[i].createdFallNodes.Count > 0)
                        {

                            _nodes.Remove(_groundNodes[i].createdFallNodes[0]);
                            _groundNodes[i].createdFallNodes.RemoveAt(0);
                        }

                        _nodes.Remove(_groundNodes[i]);
                        _groundNodes.Remove(_groundNodes[i]);
                        break;
                    }
                }
            }

            //find all nearby blocks based on searchSizes
            for (int i = 0; i < _groundNodes.Count; i++)
            {

                if ((_groundNodes[i].pos - go.transform.position).magnitude < searchSize)
                {

                    _groundNodes[i].neighbours = new List<pathNode>();
                    collect.Add(_groundNodes[i]);
                }
                if ((_groundNodes[i].pos - go.transform.position).magnitude < largerSearchSize ||
                    (Mathf.Abs(_groundNodes[i].pos.x - go.transform.position.x) < searchSize && Mathf.Abs(_groundNodes[i].pos.y - go.transform.position.y) < 8f))
                {
                    largerCollect.Add(_groundNodes[i]);
                }
            }

            UpdateNodes(collect, largerCollect);
        }

        public void UpdateNodes(List<pathNode> collection, List<pathNode> largerCollection)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                while (collection[i].createdJumpNodes.Count > 0)
                {
                    _nodes.Remove(collection[i].createdJumpNodes[0]);
                    collection[i].createdJumpNodes.RemoveAt(0);
                }
                while (collection[i].createdFallNodes.Count > 0)
                {
                    _nodes.Remove(collection[i].createdFallNodes[0]);
                    collection[i].createdFallNodes.RemoveAt(0);
                }
            }

            FindFallNodes(collection);
            FindJumpNodes(collection);

            GroundNeighbors(collection, largerCollection);

            JumpNeighbors(AttachedJumpNodes(collection), largerCollection);
            FallNeighbors(AttachedFallNodes(collection), largerCollection);

            //Make node neighbor mesh visible
            if (Input.GetKey(KeyCode.LeftShift))
                Debug.Break();
        }

        private void FindGroundNodes(List<GameObject> objects)
        {
            _nodes = new List<pathNode>();

            for (int i = 0; i < objects.Count; i++)
            {
                Vector3 ground = objects[i].transform.position;
                ground.y += _blockSize * 0.5f + _blockSize * _groundNodeHeight;
                pathNode newGroundNode = new pathNode(OrderType.Walkable, ground);
                _groundNodes.Add(newGroundNode);

                newGroundNode.c = _nodeWeights.GetNodeWeightByOrder(newGroundNode.type);
                _nodes.Add(newGroundNode);

                newGroundNode.gameObject = objects[i];
            }
        }

        private void FindFallNodes(List<pathNode> searchList)
        {
            float spacing = _blockSize * 0.5f + _blockSize * _fall_X_Spacing;

            for (int i = 0; i < searchList.Count; i++)
            {
                Vector3 leftNode = searchList[i].pos; leftNode.x -= spacing;
                Vector3 rightNode = searchList[i].pos; rightNode.x += spacing;

                //raycheck left
                if (!Physics2D.Linecast(searchList[i].pos, leftNode, _collisionLayer))
                {
                    Vector3 colliderCheck = leftNode;
                    colliderCheck.y -= _fall_Y_GrndDist;

                    //raycheck down
                    if (!Physics2D.Linecast(leftNode, colliderCheck, _collisionLayer))
                    {
                        pathNode newFallNode = new pathNode(OrderType.Fall, leftNode);

                        newFallNode.spawnedFrom = searchList[i]; //this node has been spawned from a groundNode
                                                                 //fallNodes.Add(newFallNode);

                        newFallNode.c = _nodeWeights.GetNodeWeightByOrder(newFallNode.type);
                        _nodes.Add(newFallNode);

                        newFallNode.spawnedFrom.createdFallNodes.Add(newFallNode);
                    }
                }

                //raycheck right
                if (!Physics2D.Linecast(searchList[i].pos, rightNode, _collisionLayer))
                {
                    Vector3 colliderCheck = rightNode;
                    colliderCheck.y -= _fall_Y_GrndDist;

                    //raycheck down
                    if (!Physics2D.Linecast(rightNode, colliderCheck, _collisionLayer))
                    {
                        pathNode newFallNode = new pathNode(OrderType.Fall, rightNode);

                        newFallNode.spawnedFrom = searchList[i]; //this node has been spawned from a groundNode

                        newFallNode.c = _nodeWeights.GetNodeWeightByOrder(newFallNode.type);
                        _nodes.Add(newFallNode);

                        newFallNode.spawnedFrom.createdFallNodes.Add(newFallNode);
                    }
                }
            }
        }

        private void FindJumpNodes(List<pathNode> searchList)
        {
            float jumpHeight = _normalJump.Height * 8;

            if (jumpHeight > 0)
            {
                for (int i = 0; i < searchList.Count; i++)
                {
                    float curHeight = jumpHeight;

                    while (curHeight >= _minimumJump)
                    {
                        Vector3 air = searchList[i].pos;
                        air.y += curHeight;

                        if (!Physics2D.Linecast(searchList[i].pos, air, _collisionLayer))
                        {
                            pathNode newJumpNode = new pathNode(OrderType.Jump, air);

                            newJumpNode.spawnedFrom = searchList[i]; //this node has been spawned from a groundNode
                                                                     //jumpNodes.Add(newJumpNode);
                            newJumpNode.c = _nodeWeights.GetNodeWeightByOrder(newJumpNode.type);
                            newJumpNode.height = curHeight;
                            newJumpNode.realHeight = curHeight;
                            _nodes.Add(newJumpNode);

                            newJumpNode.spawnedFrom.createdJumpNodes.Add(newJumpNode);
                        }
                        else
                        {
                            float h = curHeight;
                            float minHeight = _blockSize * 0.8f; //2f
                            while (h > minHeight)
                            {
                                Vector3 newHeight = new Vector3(air.x, air.y - (curHeight - h), air.z);
                                if (!Physics2D.Linecast(searchList[i].pos, newHeight, _collisionLayer))
                                {
                                    pathNode newJumpNode = new pathNode(OrderType.Jump, newHeight);

                                    newJumpNode.spawnedFrom = searchList[i]; //this node has been spawned from a groundNode
                                                                             //jumpNodes.Add(newJumpNode);
                                    newJumpNode.c = _nodeWeights.GetNodeWeightByOrder(newJumpNode.type);
                                    newJumpNode.realHeight = curHeight;
                                    newJumpNode.height = h;
                                    _nodes.Add(newJumpNode);

                                    newJumpNode.spawnedFrom.createdJumpNodes.Add(newJumpNode);
                                    break;
                                }
                                else
                                {
                                    //0.5f
                                    h -= _blockSize * 0.1f;
                                }
                            }
                        }
                        curHeight -= _jumpHeightIncrement;
                    }
                }
            }
        }

        private void GroundNeighbors(List<pathNode> fromNodes, List<pathNode> toNodes)
        {
            //Distance max distance allowed between two nodes
            float distanceBetween = _blockSize + _groundMaxWidth;

            for (int i = 0; i < fromNodes.Count; i++)
            {
                pathNode a = fromNodes[i];

                for (int t = 0; t < toNodes.Count; t++)
                {
                    pathNode b = toNodes[t];

                    //PREFORM A - > B TESTING HERE

                    //Is the nodes are the same, continue
                    if (a.pos.x == b.pos.x && a.pos.y == b.pos.y)
                        continue;

                    //Testing distance between nodes
                    if (Mathf.Abs(a.pos.y - b.pos.y) < _blockSize * 0.7 && Vector3.Distance(a.pos, b.pos) < distanceBetween)
                    {
                        //Testing collision between nodes
                        if (!Physics2D.Linecast(a.pos, b.pos, _collisionLayer))
                        {
                            a.neighbours.Add(b);

                            if (_debugTools)
                                Debug.DrawLine(a.pos, b.pos, Color.red);
                        }
                    }

                    //END TESTING
                }
            }
        }

        private void JumpNeighbors(List<pathNode> fromNodes, List<pathNode> toNodes)
        {
            for (int i = 0; i < fromNodes.Count; i++)
            {
                pathNode a = fromNodes[i];

                //Add jump node as neigbour of the ground node
                a.spawnedFrom.neighbours.Add(a);

                if (_debugTools)
                    Debug.DrawLine(a.pos, a.spawnedFrom.pos, Color.red);

                for (int t = 0; t < toNodes.Count; t++)
                {
                    pathNode b = toNodes[t];

                    //PREFORM A - > B TESTING HERE

                    float xDistance = Mathf.Abs(a.pos.x - b.pos.x);

                    if (xDistance < _blockSize * _normalJump.Distance * 8 + _blockSize + _groundMaxWidth) //the x distance modifier used to be 0.72!

                        if (b != a.spawnedFrom && a.pos.y > b.pos.y + _blockSize * 0.2f &&
                            a.pos.y - b.pos.y > Mathf.Abs(a.pos.x - b.pos.x) * 0.9f - _blockSize * 1.8f &&
                            Mathf.Abs(a.pos.x - b.pos.x) < _blockSize * 4f + _groundMaxWidth)
                        {
                            if (!Physics2D.Linecast(a.pos, b.pos, _collisionLayer))
                            {
                                bool hitTest = true;
                                if ((Mathf.Abs(a.pos.x - b.pos.x) < _blockSize + _groundMaxWidth && a.spawnedFrom.pos.y == b.pos.y) ||
                                    (a.pos.y - a.spawnedFrom.pos.y + 0.01f < a.height && Mathf.Abs(a.pos.x - b.pos.x) > _blockSize + _groundMaxWidth))
                                {
                                    hitTest = false;
                                }

                                //hit head code... jump height must be above 2.5 to move Xdistance, 2.5 else you can only move 1 block when hitting head.
                                if (a.realHeight > a.height)
                                {
                                    float tempFloat = a.height > 2.5f ? 3.5f : 1.5f;
                                    if (tempFloat == 1.5f && a.height > 1.9f) { tempFloat = 2.2f; }
                                    if (a.spawnedFrom.pos.y < b.pos.y && Mathf.Abs(a.spawnedFrom.pos.x - b.pos.x) > _blockSize * 1.5f) { tempFloat = 0f; }
                                    if (Mathf.Abs(a.spawnedFrom.pos.x - b.pos.x) > _blockSize * tempFloat)
                                    {
                                        hitTest = false;
                                    }
                                }

                                if (hitTest)
                                {
                                    float middle = -(a.pos.x - b.pos.x) / 2f;
                                    float quarter = middle / 2f;

                                    Vector3 origin = a.spawnedFrom.pos;
                                    Vector3 midPoint = new Vector3(a.pos.x + middle, a.pos.y, a.pos.z);
                                    Vector3 quarterPoint = new Vector3(a.pos.x + quarter, a.pos.y, a.pos.z);

                                    Vector3 quarterPastMidPoint = new Vector3(a.pos.x + middle + quarter, a.pos.y - _blockSize, a.pos.z);
                                    Vector3 lowerMid = new Vector3(a.pos.x + middle, a.pos.y - _blockSize, a.pos.z);
                                    Vector3 straightUp = new Vector3(b.pos.x, a.pos.y - _blockSize, a.pos.z);

                                    if (xDistance > _blockSize + _groundMaxWidth)
                                        if (Physics2D.Linecast(origin, quarterPoint, _collisionLayer) ||

                                            (xDistance > _blockSize + _groundMaxWidth &&
                                             Physics2D.Linecast(b.pos, quarterPastMidPoint, _collisionLayer) &&
                                             a.spawnedFrom.pos.y >= b.pos.y - _groundNodeHeight) ||

                                            (Physics2D.Linecast(origin, midPoint, _collisionLayer)) ||

                                              (xDistance > _blockSize + _groundMaxWidth &&
                                             a.spawnedFrom.pos.y >= b.pos.y - _groundNodeHeight &&
                                             Physics2D.Linecast(lowerMid, b.pos, _collisionLayer)) ||

                                                (xDistance > _blockSize * 1f + _groundMaxWidth &&
                                             a.spawnedFrom.pos.y >= b.pos.y &&
                                              Physics2D.Linecast(b.pos, straightUp, _collisionLayer)))
                                        {
                                            hitTest = false;
                                        }
                                }

                                if (hitTest)
                                {
                                    a.neighbours.Add(b);

                                    if (_debugTools)
                                        Debug.DrawLine(a.pos, b.pos, Color.blue);
                                }
                            }
                        }

                    //END TESTING
                }
            }
        }

        private void FallNeighbors(List<pathNode> fromNodes, List<pathNode> toNodes)
        {
            for (int i = 0; i < fromNodes.Count; i++)
            {
                pathNode a = fromNodes[i];

                //Add fall node as neigbour of the ground node
                a.spawnedFrom.neighbours.Add(a);

                if (_debugTools)
                    Debug.DrawLine(a.spawnedFrom.pos, a.pos, Color.blue);

                for (int t = 0; t < toNodes.Count; t++)
                {
                    pathNode b = toNodes[t];

                    //PREFORM A - > B TESTING HERE
                    float xDistance = Mathf.Abs(a.pos.x - b.pos.x);

                    //FALL NODES REQUIRE TESTING
                    //CHARACTER WIDTH!
                    //probably a similar formula to jumpnode neighbors
                    if ((xDistance < _blockSize * 2 + _groundMaxWidth && a.pos.y > b.pos.y) || (a.pos.y - b.pos.y > Mathf.Abs(a.pos.x - b.pos.x) * 2.2f + _blockSize * 1f && //2.2 + blocksize * 1f
                        xDistance < _blockSize * 4f))
                    {
                        if (!Physics2D.Linecast(a.pos, b.pos, _collisionLayer))
                        {
                            bool hitTest = true;

                            float middle = -(a.pos.x - b.pos.x) * 0.5f;
                            float quarter = middle / 2f;

                            float reduceY = Mathf.Abs(a.pos.y - b.pos.y) > _blockSize * 4f ? _blockSize * 1.3f : 0f;

                            Vector3 middlePointDrop = new Vector3(a.pos.x + middle, a.pos.y - reduceY, a.pos.z);
                            Vector3 quarterPointTop = new Vector3(a.pos.x + quarter, a.pos.y, a.pos.z);
                            Vector3 quarterPointBot = new Vector3(b.pos.x - quarter, b.pos.y, b.pos.z);

                            //Vector3 corner = new Vector3(b.pos.x, (a.pos.y - _blockSize * xDistance - _blockSize * 0.5f) - _groundNodeHeight, a.pos.z);

                            //Got rid od the corner linecast and the y pos check, didn't know why they were here and made it bug
                            if (Physics2D.Linecast(quarterPointTop, b.pos, _collisionLayer) ||
                                Physics2D.Linecast(middlePointDrop, b.pos, _collisionLayer) ||
                                //a.pos.y > b.pos.y + _blockSize + _groundNodeHeight &&
                                //Physics2D.Linecast(corner, b.pos, _groundLayer) ||
                                Physics2D.Linecast(quarterPointBot, a.pos, _collisionLayer))
                            {
                                hitTest = false;
                            }

                            if (hitTest)
                            {
                                a.neighbours.Add(b);

                                if (_debugTools)
                                    Debug.DrawLine(a.pos, b.pos, Color.black);
                            }
                        }
                    }

                    //END TESTING
                }
            }
        }

        /// <summary>
        /// Get nearest node ground. Useful for finding start and end points of the path
        /// </summary>
        private pathNode GetNearestNode(Vector3 obj)
        {
            float dist = float.MaxValue;
            pathNode node = null;

            for (int i = 0; i < _groundNodes.Count; i++)
            {
                if (_groundNodes[i].neighbours.Count > 0 && obj.y > _groundNodes[i].pos.y - _blockSize * 0.8f && Mathf.Abs(obj.x - _groundNodes[i].pos.x) < _blockSize
                    /*only find ground nodes that are within 4f*/&& obj.y - _groundNodes[i].pos.y < 1000f)
                {
                    float temp = Vector3.Distance(obj, (Vector3)_groundNodes[i].pos);
                    if (dist > temp)
                    {
                        dist = temp;
                        node = _groundNodes[i];
                    }
                }
            }


            return node;
        }

        private pathNode GetNearestGroundNode(Vector3 obj)
        {
            float dist = float.MaxValue;
            pathNode node = null;

            for (int i = 0; i < _groundNodes.Count; i++)
            {
                if (_groundNodes[i].neighbours.Count > 0)
                {
                    float temp = Vector3.Distance(obj, _groundNodes[i].pos);
                    if (dist > temp)
                    {
                        //Added the -0.1f to account for the precision
                        if (obj.y >= _groundNodes[i].pos.y - _blockSize * 0.8f && Mathf.Abs(obj.x - _groundNodes[i].pos.x) < _blockSize)
                        {
                            dist = temp;
                            node = _groundNodes[i];
                        }
                    }
                }
            }
            return node;
        }

        /// <summary>
        /// Used when reconstructing pathnode connections
        /// </summary>
        private List<pathNode> AttachedJumpNodes(List<pathNode> pGround)
        {

            List<pathNode> returnNodes = new List<pathNode>();
            for (int i = 0; i < pGround.Count; i++)
            {
                returnNodes.AddRange(pGround[i].createdJumpNodes);
            }
            return returnNodes;
        }

        private List<pathNode> AttachedFallNodes(List<pathNode> pGround)
        {

            List<pathNode> returnNodes = new List<pathNode>();
            for (int i = 0; i < pGround.Count; i++)
            {

                returnNodes.AddRange(pGround[i].createdFallNodes);
            }
            return returnNodes;
        }

        /// <summary>
        /// Create a thread to compute the FindPath
        /// </summary>
        public void MakeThreadDoWork()
        {
            //If there are orders and the thread is either null or not alive
            if ((_orders.Count > 0 && _t == null) || (_orders.Count > 0 && !_t.IsAlive))
            {
                _t = new Thread(new ParameterizedThreadStart(FindPath));
                _t.IsBackground = true;
                _t.Start(_orders[0]);
                _orders.RemoveAt(0);
            }
        }

        private void OnDrawGizmos()
        {
            if (_debugTools)
            {
                for (int i = 0; i < _nodes.Count; i++)
                {
                    Gizmos.color = _nodeWeights.GetNodeColorByOrder(_nodes[i].type);
                    Gizmos.DrawSphere(_nodes[i].pos, 0.5f);
                }
            }
        }

        public class ThreadLock
        {
            public PathfindingAgent agent;
            public bool passed = false;
            public Vector3 agentPos;
            public Vector3 end;
            public float jump;
            public List<Instructions> instr = null;

            //Abilities
            public bool canMove;
            public bool canJump;
            public bool canFall;

            public ThreadLock(PathfindingAgent agent, Vector3 end, float jumpHeight, bool cMove, bool cJump, bool cFall)
            {
                this.agent = agent;
                agentPos = agent.transform.position;
                this.end = end;
                jump = jumpHeight;

                canMove = cMove;
                canJump = cJump;
                canFall = cFall;
            }
        }

        [System.Serializable]
        public class NodeWeight
        {
            public float groundNode = 1f;   //2
            public float jumpNode = 9.2f;   //18.4
            public float fallNode = 1f;     //2

            public float GetNodeWeightByOrder(OrderType nodeType)
            {
                switch (nodeType)
                {
                    case OrderType.None:
                        return 0f;
                    case OrderType.Walkable:
                        return groundNode;
                    case OrderType.Jump:
                        return jumpNode;
                    case OrderType.Fall:
                        return fallNode;
                    default:
                        return 0f;
                }
            }

            public Color GetNodeColorByOrder(OrderType nodeType)
            {
                switch (nodeType)
                {
                    case OrderType.None:
                        return Color.white;
                    case OrderType.Walkable:
                        return Color.yellow;
                    case OrderType.Jump:
                        return Color.blue;
                    case OrderType.Fall:
                        return Color.black;
                    default:
                        return Color.white;
                }
            }
        }

        #region Getters & Setters

        public List<pathNode> GroundNodes => _groundNodes;
        public LayerMask GroundLayer => _groundLayer;

        #endregion

        private void Log(string log)
        {
            Debug.Log("[Pathfinding] : " + log);
        }
    }

    public class pathNode
    {
        public Vector3 pos;
        public OrderType type;
        public float realHeight = 0f;
        public float height = 0f;

        /// <summary>
        /// Estimated distance from finish
        /// </summary>
        public float f = 0f;
        /// <summary>
        /// Cost to get to node
        /// </summary>
        public float g = 0f;
        /// <summary>
        /// Cost of node
        /// </summary>
        public float c = 0f;
        /// <summary>
        /// Node value
        /// </summary>
        public float h = 0f;

        public pathNode parent = null;
        public GameObject gameObject;

        /// <summary>
        /// The parent node
        /// </summary>
        public pathNode spawnedFrom = null;
        public List<pathNode> createdJumpNodes = new List<pathNode>();
        public List<pathNode> createdFallNodes = new List<pathNode>();

        public List<pathNode> neighbours = new List<pathNode>();

        public pathNode(OrderType typeOfNode, Vector3 position)
        {
            pos = position;
            type = typeOfNode;
        }
    }

    public class Instructions
    {
        public Vector3 pos = Vector3.zero;
        public OrderType order = OrderType.None;

        public Instructions(Vector3 position, OrderType order)
        {
            pos = position;
            this.order = order;
        }
    }

    public enum OrderType { None, Walkable, Jump, Fall }
}