﻿using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cursed.Character;

    public class Pathfinding : MonoBehaviour
    {
        LayerMask groundLayer;

        public GameObject _currentMap;

        public float blockSize = 1f; //each block is square. This should probably match your square 2dCollider on a tile.
        public float jumpHeight = 3.8f; //the maximum jump height of a character
        public float maxJumpBlocksX = 3f; //the furthest a character can jump without momentum
        public float jumpHeightIncrement = 1f;
        public float minimumJump = 1.8f;

        [SerializeField]
        private float _groundNodeHeight = 0.01f; //percentage of blockSize (Determines height off ground level for a groundNode)
        private float _groundMaxWidth = 0.35f; //percentage of blockSize (Determines max spacing allowed between two groundNodes)
        private float _fall_X_Spacing = 0.25f; //percentage of blockSize (Determines space away from groundNode's side to place the fallNode)
        private float _fall_Y_GrndDist = 0.02f;
        private Thread _t;

        private List<pathNode> _nodes = new List<pathNode>();
        private List<pathNode> _groundNodes = new List<pathNode>();

        public List<threadLock> orders = new List<threadLock>();
        public List<threadLock> readyOrders = new List<threadLock>();

        public nodeWeight nodeWeights;

        public bool debugTools = false; /*Pauses game on runtime and displays pathnode connections*/

        void Awake()
        {
            groundLayer = LayerManager.instance.groundLayer;        
        }

        void Start()
        {
            CreateNodeMap();
        }

        void Update()
        {
            DeliverPathfindingInstructions();
            MakeThreadDoWork();

        }

        void CreateNodeMap()
        {
            _nodes = new List<pathNode>();
            _groundNodes = new List<pathNode>();

            List<GameObject> groundObjects = new List<GameObject>();

            //Find all children of tile parent
            foreach (Transform child in _currentMap.transform)
            {
                if (1 << child.gameObject.layer == groundLayer.value)
                {
                    groundObjects.Add(child.gameObject);
                }
            }

            FindGroundNodes(groundObjects);
            Debug.Log(_groundNodes.Count);
            FindFallNodes(_groundNodes); //@param list of nodes to search (tiles)
            FindJumpNodes(_groundNodes);

            GroundNeighbors(_groundNodes, _groundNodes);
            JumpNeighbors(attachedJumpNodes(_groundNodes), _groundNodes); //CHANGE this function to find all jump nodes attached to ground nodes **********TODO
            FallNeighbors(attachedFallNodes(_groundNodes), _groundNodes);  //CHANGE this function to find all fall nodes attached to ground nodes **********TODO

            if (debugTools)
            {
                Debug.Break();
            }
        }

        public void RequestPathInstructions(GameObject character, Vector3 location, float jumpH,/*char abilities*/ bool movement, bool jump, bool fall)
        {
            bool replaced = false;
            threadLock newLocker = new threadLock(character, location, jumpH, movement, jump, fall);

            for (int i = 1; i < orders.Count; i++)
            {
                if (orders[i].character == character)
                {
                    orders[i] = newLocker; replaced = true; break;
                }
            }

            if (!replaced)
            {
                orders.Add(newLocker);
            }
        }

        public void FindPath(object threadLocker)
        {
            threadLock a = (threadLock)threadLocker;
            Vector3 character = a.charPos;
            Vector3 location = a.end;
            float characterJump = a.jump;

            List<instructions> instr = new List<instructions>();

            List<pathNode> openNodes = new List<pathNode>();
            List<pathNode> closedNodes = new List<pathNode>();
            List<pathNode> pathNodes = new List<pathNode>();

            ResetLists(); //sets parent to null

            pathNode startNode = new pathNode("", Vector3.zero);
            startNode = getNearestGroundNode(character);

            pathNode endNode = getNearestNode(location);

            /*if a point couldnt be found or if character can't move cancel path*/
            if (endNode == null || startNode == null || !a.canMove)
            {
                a.passed = false;
                a.instr = instr;
                readyOrders.Add(a);
                return;
            }

            startNode.g = 0;
            startNode.f = Vector3.Distance(startNode.pos, endNode.pos);

            openNodes.Add(startNode);


            pathNode currentNode = new pathNode("0", Vector3.zero);
            while (openNodes.Count > 0)
            {
                float lowestScore = float.MaxValue;
                for (int i = 0; i < openNodes.Count; i++)
                {
                    if (openNodes[i].f < lowestScore)
                    {
                        currentNode = openNodes[i]; lowestScore = currentNode.f;
                    }
                }
                if (currentNode == endNode) { closedNodes.Add(currentNode); break; }
                else
                {
                    closedNodes.Add(currentNode);
                    openNodes.Remove(currentNode);
                    if (currentNode.type != "jump" || (currentNode.type == "jump"
                        && Mathf.Abs(currentNode.realHeight - characterJump) < jumpHeightIncrement * 0.92) && characterJump <= currentNode.realHeight + jumpHeightIncrement * 0.08)
                    {
                        for (int i = 0; i < currentNode.neighbours.Count; i++)
                        {
                            //check if node can be used by character
                            if (!a.canJump && currentNode.neighbours[i].type == "jump") { continue; }
                            if (!a.canFall && currentNode.neighbours[i].type == "fall") { continue; }

                            if (currentNode.neighbours[i].parent == null)
                            {

                                currentNode.neighbours[i].g = currentNode.neighbours[i].c + currentNode.g;
                                currentNode.neighbours[i].h = Vector3.Distance(currentNode.neighbours[i].pos, endNode.pos);
                                if (currentNode.neighbours[i].type == "jump") { currentNode.neighbours[i].h += currentNode.neighbours[i].realHeight; }
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

            for (int i = 0; i < 700; i++)
            {
                if (i > 600) { Debug.Log("somethingwrong"); }
                pathNodes.Add(currentNode);

                if (currentNode.parent != null)
                {
                    currentNode = currentNode.parent;
                }
                else { break; }
                if (currentNode == startNode)
                {
                    pathNodes.Add(startNode);
                    break;
                }
            }

            if (pathNodes[0] != endNode)
            {
                a.passed = false;
            }
            else { a.passed = true;}

            pathNodes.Reverse();
            for (int i = 0; i < pathNodes.Count; i++)
            {
                instructions temp = new instructions(pathNodes[i].pos, pathNodes[i].type);
                instr.Add(temp);
            }

            a.instr = instr;
            readyOrders.Add(a);
        }

        public void DeliverPathfindingInstructions()
        {
            for (int i = 0; i < readyOrders.Count; i++)
            {

                if (readyOrders[i].character)
                {

                    if (readyOrders[i].character.transform.GetComponent<PathfindingAgent>() != null)
                    {
                        readyOrders[i].character.transform.GetComponent<PathfindingAgent>().ReceivePathInstructions(readyOrders[i].instr, readyOrders[i].passed);
                    }
                }
            }
            readyOrders = new List<threadLock>();
        }

        private void ResetLists()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].parent = null;
            }
        }

        //Used for runtime adding terrain block to pathfinding nodes
        public void CreateBlockCalled(Vector3 position)
        {

            GameObject newGameObject = (GameObject)Instantiate(Resources.Load("Tiles/GroundTile"), new Vector3(position.x, position.y, 0.0f), Quaternion.identity) as GameObject;
            newGameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/ground2", typeof(Sprite)) as Sprite;
            //add the ground node, 

            Vector3 ground = newGameObject.transform.position; ground.y += blockSize * 0.5f + blockSize * _groundNodeHeight;
            pathNode newGroundNode = new pathNode("walkable", ground);
            _groundNodes.Add(newGroundNode);

            newGroundNode.c = nodeWeights.GetNodeWeightByString(newGroundNode.type);
            _nodes.Add(newGroundNode);

            newGroundNode.gameObject = newGameObject;

            //run it through the list

            RefreshAreaAroundBlock(newGameObject, false);
        }

        //Used by creating and Removing pathfinding nodes
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

            JumpNeighbors(attachedJumpNodes(collection), largerCollection);
            FallNeighbors(attachedFallNodes(collection), largerCollection);

            if (Input.GetKey(KeyCode.LeftShift)) { Debug.Break(); } //make node neighbor mesh visible
        }

        private void FindGroundNodes(List<GameObject> objects)
        {
            _nodes = new List<pathNode>();

            for (int i = 0; i < objects.Count; i++)
            {
                Vector3 ground = objects[i].transform.position; ground.y += blockSize * 0.5f + blockSize * _groundNodeHeight;
                pathNode newGroundNode = new pathNode("walkable", ground);
                _groundNodes.Add(newGroundNode);

                newGroundNode.c = nodeWeights.GetNodeWeightByString(newGroundNode.type);
                _nodes.Add(newGroundNode);

                newGroundNode.gameObject = objects[i];
            }
        }

        private void GroundNeighbors(List<pathNode> fromNodes, List<pathNode> toNodes)
        {
            //Distance max distance allowed between two nodes
            float distanceBetween = blockSize + _groundMaxWidth;

            for (int i = 0; i < fromNodes.Count; i++)
            {
                pathNode a = fromNodes[i];

                for (int t = 0; t < toNodes.Count; t++)
                {
                    pathNode b = toNodes[t];

                    //PREFORM A - > B TESTING HERE

                    //testing distance between nodes
                    if (Mathf.Abs(a.pos.y - b.pos.y) < blockSize * 0.7 && Vector3.Distance(a.pos, b.pos) < distanceBetween)
                    {
                        //testing collision between nodes
                        if (!Physics2D.Linecast(a.pos, b.pos, groundLayer))
                        {
                            a.neighbours.Add(b);
                            if (debugTools)
                            {
                                Debug.DrawLine(a.pos, b.pos, Color.red);
                            }

                        }
                    }

                    //END TESTING
                }
            }
        }

        private void FindJumpNodes(List<pathNode> searchList)
        {
            if (jumpHeight > 0)
            {
                for (int i = 0; i < searchList.Count; i++)
                {
                    float curHeight = jumpHeight;

                    while (curHeight >= minimumJump)
                    {
                        Vector3 air = searchList[i].pos; air.y += curHeight;

                        if (!Physics2D.Linecast(searchList[i].pos, air, groundLayer))
                        {
                            pathNode newJumpNode = new pathNode("jump", air);

                            newJumpNode.spawnedFrom = searchList[i]; //this node has been spawned from a groundNode
                                                                     //jumpNodes.Add(newJumpNode);
                            newJumpNode.c = nodeWeights.GetNodeWeightByString(newJumpNode.type);
                            newJumpNode.height = curHeight;
                            newJumpNode.realHeight = curHeight;
                            _nodes.Add(newJumpNode);

                            newJumpNode.spawnedFrom.createdJumpNodes.Add(newJumpNode);
                        }
                        else
                        {
                            float h = curHeight;
                            float minHeight = blockSize * 1f; //2f
                            while (h > minHeight)
                            {
                                Vector3 newHeight = new Vector3(air.x, air.y - (curHeight - h), air.z);
                                if (!Physics2D.Linecast(searchList[i].pos, newHeight, groundLayer))
                                {
                                    pathNode newJumpNode = new pathNode("jump", newHeight);

                                    newJumpNode.spawnedFrom = searchList[i]; //this node has been spawned from a groundNode
                                                                             //jumpNodes.Add(newJumpNode);
                                    newJumpNode.c = nodeWeights.GetNodeWeightByString(newJumpNode.type);
                                    newJumpNode.realHeight = curHeight;
                                    newJumpNode.height = h;
                                    _nodes.Add(newJumpNode);

                                    newJumpNode.spawnedFrom.createdJumpNodes.Add(newJumpNode);
                                    break;
                                }
                                else
                                {
                                    //0.5f
                                    h -= blockSize * 0.1f;
                                }
                            }
                        }
                        curHeight -= jumpHeightIncrement;
                    }
                }
            }
        }
        private void JumpNeighbors(List<pathNode> fromNodes, List<pathNode> toNodes)
        {
            for (int i = 0; i < fromNodes.Count; i++)
            {
                pathNode a = fromNodes[i];

                for (int t = 0; t < toNodes.Count; t++)
                {
                    pathNode b = toNodes[t];

                    //PREFORM A - > B TESTING HERE

                    a.spawnedFrom.neighbours.Add(a);
                    if (debugTools)
                    {
                        Debug.DrawLine(a.pos, a.spawnedFrom.pos, Color.red);
                    }

                    float xDistance = Mathf.Abs(a.pos.x - b.pos.x);


                    if (xDistance < blockSize * maxJumpBlocksX + blockSize + _groundMaxWidth) //
                                                                                              //the x distance modifier used to be 0.72!
                        if (b != a.spawnedFrom && a.pos.y > b.pos.y + blockSize * 0.5f &&

                            a.pos.y - b.pos.y > Mathf.Abs(a.pos.x - b.pos.x) * 0.9f - blockSize * 1f &&
                              Mathf.Abs(a.pos.x - b.pos.x) < blockSize * 4f + _groundMaxWidth)
                        { //4.7, 4Xjump, +1Y isnt working
                            if (!Physics2D.Linecast(a.pos, b.pos, groundLayer))
                            {
                                bool hitTest = true;
                                if ((Mathf.Abs(a.pos.x - b.pos.x) < blockSize + _groundMaxWidth && a.spawnedFrom.pos.y == b.pos.y) ||
                                    (a.pos.y - a.spawnedFrom.pos.y + 0.01f < a.height && Mathf.Abs(a.pos.x - b.pos.x) > blockSize + _groundMaxWidth))
                                {
                                    hitTest = false;

                                }

                                //hit head code... jump height must be above 2.5 to move Xdistance2.5 else you can only move 1 block when hitting head.
                                if (a.realHeight > a.height)
                                {
                                    float tempFloat = a.height > 2.5f ? 3.5f : 1.5f;
                                    if (tempFloat == 1.5f && a.height > 1.9f) { tempFloat = 2.2f; }
                                    if (a.spawnedFrom.pos.y < b.pos.y && Mathf.Abs(a.spawnedFrom.pos.x - b.pos.x) > blockSize * 1.5f) { tempFloat = 0f; }
                                    if (Mathf.Abs(a.spawnedFrom.pos.x - b.pos.x) > blockSize * tempFloat)
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

                                    Vector3 quarterPastMidPoint = new Vector3(a.pos.x + middle + quarter, a.pos.y - blockSize, a.pos.z);
                                    Vector3 lowerMid = new Vector3(a.pos.x + middle, a.pos.y - blockSize, a.pos.z);
                                    Vector3 straightUp = new Vector3(b.pos.x, a.pos.y - blockSize, a.pos.z);

                                    if (xDistance > blockSize + _groundMaxWidth)
                                        if (Physics2D.Linecast(origin, quarterPoint, groundLayer) ||

                                            (xDistance > blockSize + _groundMaxWidth &&
                                             Physics2D.Linecast(b.pos, quarterPastMidPoint, groundLayer) &&
                                             a.spawnedFrom.pos.y >= b.pos.y - _groundNodeHeight) ||

                                            (Physics2D.Linecast(origin, midPoint, groundLayer)) ||

                                              (xDistance > blockSize + _groundMaxWidth &&
                                             a.spawnedFrom.pos.y >= b.pos.y - _groundNodeHeight &&
                                             Physics2D.Linecast(lowerMid, b.pos, groundLayer)) ||

                                                (xDistance > blockSize * 1f + _groundMaxWidth &&
                                             a.spawnedFrom.pos.y >= b.pos.y &&
                                              Physics2D.Linecast(b.pos, straightUp, groundLayer))
                                           )
                                        {
                                            hitTest = false;
                                        }

                                }

                                if (hitTest)
                                {
                                    a.neighbours.Add(b);
                                    if (debugTools)
                                    {
                                        Debug.DrawLine(a.pos, b.pos, Color.blue);
                                    }
                                }
                            }
                        }

                    //END TESTING
                }
            }
        }

        private void FindFallNodes(List<pathNode> searchList)
        {
            float spacing = blockSize * 0.5f + blockSize * _fall_X_Spacing;

            for (int i = 0; i < searchList.Count; i++)
            {
                Vector3 leftNode = searchList[i].pos; leftNode.x -= spacing;
                Vector3 rightNode = searchList[i].pos; rightNode.x += spacing;

                //raycheck left
                if (!Physics2D.Linecast(searchList[i].pos, leftNode, groundLayer))
                {
                    Vector3 colliderCheck = leftNode;
                    colliderCheck.y -= _fall_Y_GrndDist;

                    //raycheck down
                    if (!Physics2D.Linecast(leftNode, colliderCheck, groundLayer))
                    {
                        pathNode newFallNode = new pathNode("fall", leftNode);

                        newFallNode.spawnedFrom = searchList[i]; //this node has been spawned from a groundNode
                                                                 //fallNodes.Add(newFallNode);

                        newFallNode.c = nodeWeights.GetNodeWeightByString(newFallNode.type);
                        _nodes.Add(newFallNode);

                        newFallNode.spawnedFrom.createdFallNodes.Add(newFallNode);
                    }
                }

                //raycheck right
                if (!Physics2D.Linecast(searchList[i].pos, rightNode, groundLayer))
                {
                    Vector3 colliderCheck = rightNode;
                    colliderCheck.y -= _fall_Y_GrndDist;

                    //raycheck down
                    if (!Physics2D.Linecast(rightNode, colliderCheck, groundLayer))
                    {
                        pathNode newFallNode = new pathNode("fall", rightNode);

                        newFallNode.spawnedFrom = searchList[i]; //this node has been spawned from a groundNode

                        newFallNode.c = nodeWeights.GetNodeWeightByString(newFallNode.type);
                        _nodes.Add(newFallNode);

                        newFallNode.spawnedFrom.createdFallNodes.Add(newFallNode);
                    }
                }
            }
        }
        private void FallNeighbors(List<pathNode> fromNodes, List<pathNode> toNodes)
        {
            for (int i = 0; i < fromNodes.Count; i++)
            {
                pathNode a = fromNodes[i];

                for (int t = 0; t < toNodes.Count; t++)
                {
                    pathNode b = toNodes[t];

                    //PREFORM A - > B TESTING HERE
                    float xDistance = Mathf.Abs(a.pos.x - b.pos.x);
                    a.spawnedFrom.neighbours.Add(a);
                    if (debugTools)
                    {
                        Debug.DrawLine(a.spawnedFrom.pos, a.pos, Color.blue);
                    }

                    //FALL NODES REQUIRE TESTING
                    //CHARACTER WIDTH!
                    //probably a similar formula to jumpnode neighbors
                    if ((xDistance < blockSize * 1f + _groundMaxWidth && a.pos.y > b.pos.y) || (a.pos.y - b.pos.y > Mathf.Abs(a.pos.x - b.pos.x) * 2.2f + blockSize * 1f && //2.2 + blocksize * 1f
                        xDistance < blockSize * 4f))
                    {
                        if (!Physics2D.Linecast(a.pos, b.pos, groundLayer))
                        {
                            bool hitTest = true;

                            float middle = -(a.pos.x - b.pos.x) * 0.5f;
                            float quarter = middle / 2f;

                            float reduceY = Mathf.Abs(a.pos.y - b.pos.y) > blockSize * 4f ? blockSize * 1.3f : 0f;

                            Vector3 middlePointDrop = new Vector3(a.pos.x + middle, a.pos.y - reduceY, a.pos.z);
                            Vector3 quarterPointTop = new Vector3(a.pos.x + quarter, a.pos.y, a.pos.z);
                            Vector3 quarterPointBot = new Vector3(b.pos.x - quarter, b.pos.y, b.pos.z);

                            Vector3 corner = new Vector3(b.pos.x, (a.pos.y - blockSize * xDistance - blockSize * 0.5f) - _groundNodeHeight, a.pos.z);

                            if (Physics2D.Linecast(quarterPointTop, b.pos, groundLayer) ||
                                Physics2D.Linecast(middlePointDrop, b.pos, groundLayer) ||
                                a.pos.y > b.pos.y + blockSize + _groundNodeHeight && Physics2D.Linecast(corner, b.pos, groundLayer) ||
                                Physics2D.Linecast(quarterPointBot, a.pos, groundLayer))
                            {
                                hitTest = false;
                            }
                            if (hitTest)
                            {
                                a.neighbours.Add(b);
                                if (debugTools)
                                {
                                    Debug.DrawLine(a.pos, b.pos, Color.black);
                                }
                            }
                        }
                    }

                    //END TESTING
                }
            }
        }

        //get nearest node ground. Useful for finding start and end points of the path
        private pathNode getNearestNode(Vector3 obj)
        {
            float dist = float.MaxValue;
            pathNode node = null;

            for (int i = 0; i < _groundNodes.Count; i++)
            {
                if (_groundNodes[i].neighbours.Count > 0 && obj.y > _groundNodes[i].pos.y && Mathf.Abs(obj.x - _groundNodes[i].pos.x) < blockSize
                    /*only find ground nodes that are within 4f*/&& obj.y - _groundNodes[i].pos.y < 10000f)
                {
                    Debug.Log("testino");
                    float temp = Vector3.Distance(obj, (Vector3)_groundNodes[i].pos);
                    if (dist > temp)
                    {
                        Debug.Log("nodeset");
                        dist = temp; node = _groundNodes[i];
                    }
                }
            }


            return node;
        }
        private pathNode getNearestGroundNode(Vector3 obj)
        {
            float dist = float.MaxValue;
            pathNode node = null;

            for (int i = 0; i < _groundNodes.Count; i++)
            {
                if (_groundNodes[i].neighbours.Count > 0)
                {
                    float temp = Vector3.Distance(obj, (Vector3)_groundNodes[i].pos);
                    if (dist > temp)
                    {
                        if (obj.y >= _groundNodes[i].pos.y && Mathf.Abs(obj.x - _groundNodes[i].pos.x) < blockSize)
                        {
                            dist = temp; node = _groundNodes[i];
                        }
                    }
                }
            }
            return node;
        }

        //Used when reconstructing pathnode connections
        List<pathNode> attachedJumpNodes(List<pathNode> pGround)
        {

            List<pathNode> returnNodes = new List<pathNode>();
            for (int i = 0; i < pGround.Count; i++)
            {

                returnNodes.AddRange(pGround[i].createdJumpNodes);
            }
            return returnNodes;
        }
        List<pathNode> attachedFallNodes(List<pathNode> pGround)
        {

            List<pathNode> returnNodes = new List<pathNode>();
            for (int i = 0; i < pGround.Count; i++)
            {

                returnNodes.AddRange(pGround[i].createdFallNodes);
            }
            return returnNodes;
        }

        public void MakeThreadDoWork()
        {
            if ((orders.Count > 0 && _t == null) || (orders.Count > 0 && !_t.IsAlive))
            {
                _t = new Thread(new ParameterizedThreadStart(FindPath));
                _t.IsBackground = true;
                _t.Start(orders[0]);
                orders.RemoveAt(0);
            }
        }

        private void OnDrawGizmos()
        {
            if (debugTools)
            {

                for (int i = 0; i < _nodes.Count; i++)
                {
                    Gizmos.color = nodeWeights.GetNodeColorByString(_nodes[i].type);
                    Gizmos.DrawSphere(_nodes[i].pos, 0.5f);
                }
            }
        }

        public class threadLock
        {
            public GameObject character;
            public bool passed = false;
            public Vector3 charPos, end;
            public float jump;
            public List<instructions> instr = null;

            //abilities
            public bool canMove;
            public bool canJump;
            public bool canFall;

            public threadLock(GameObject pC, Vector3 pE, float jumpHeight, bool cMove, bool cJump, bool cFall)
            {
                character = pC;
                charPos = pC.transform.position;
                end = pE;
                jump = jumpHeight;

                canMove = cMove;
                canJump = cJump;
                canFall = cFall;
            }
        }

        [System.Serializable]
        public class nodeWeight
        {

            public float groundNode = 1f;
            public float jumpNode = 9.2f;
            public float fallNode = 1f;

            public float GetNodeWeightByString(string nodeType)
            {
                switch (nodeType)
                {
                    case "walkable": return groundNode;
                    case "jump": return jumpNode;
                    case "fall": return fallNode;
                }
                return 0f;
            }

            public Color GetNodeColorByString(string nodeType)
            {
                switch (nodeType)
                {
                    case "walkable": return Color.yellow;
                    case "jump": return Color.blue;
                    case "fall": return Color.black;
                }
                return Color.white;
            }
        }

        public List<pathNode> GroundNodes => _groundNodes;
    }

    //Accessible classes from other scripts below

    public class pathNode
    {
        public Vector3 pos;
        public string type;
        public float realHeight = 0f;
        public float height = 0f;

        public float f = 0f; //estimated distance from finish
        public float g = 0f; //cost to get to node
        public float c = 0f; //cost of node

        public float h = 0f; //nodeValue

        public pathNode parent = null;
        public GameObject gameObject;

        public pathNode spawnedFrom = null; //the node that created this.
        public List<pathNode> createdJumpNodes = new List<pathNode>();
        public List<pathNode> createdFallNodes = new List<pathNode>();

        public List<pathNode> neighbours = new List<pathNode>();

        public pathNode(string typeOfNode, Vector3 position)
        {
            pos = position;
            type = typeOfNode;
        }
    }

    public class instructions
    {
        public Vector3 pos = Vector3.zero;
        public string order = "none";

        public instructions(Vector3 position, string pOrder)
        {
            pos = position;
            order = pOrder;
        }
    }