using System.Collections;
using UnityEngine;

public class AiController : MonoBehaviour
{

    public enum ai_state { none, groundpatrol, chase, attack } /*Add custom AI states here!*/

    public ai_state state = ai_state.groundpatrol;

    private CharacterController2D _controller;
    private PathfindingAgent _pathAgent;
    public static Pathfinding _pathScript;
    [System.NonSerialized]
    public TextMesh _behaviourText;

    public static GameObject player;
    private bool _destroy = false;

    private bool _timerChangeTarget = false;


    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        _controller = GetComponent<CharacterController2D>();
        _pathAgent = GetComponent<PathfindingAgent>();

        if (_pathScript == null) { _pathScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<Pathfinding>(); }

        _behaviourText = transform.Find("BehaviourText").GetComponent<TextMesh>();
        switch (state)
        {
            case ai_state.groundpatrol: _behaviourText.text = "Ground Patrol"; break;

            default: _behaviourText.text = ""; break;
        }
    }

    private bool PlayerInRange(float range, bool raycastOn)
    {
        if (player && Vector3.Distance(player.transform.position, transform.position) < range)
        {
            if (raycastOn && !Physics2D.Linecast(transform.position, player.transform.position, _controller.collisionMask))
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

    /*gets called from pathagent when character finishes navigating path*/
    public void PathCompleted()
    {
        switch (state)
        {
            case ai_state.groundpatrol: _behaviourText.text = ""; break;
            case ai_state.chase: _behaviourText.text = "Chase"; break;
        }
    }

    /*gets called from pathagent when character begins navigating path*/
    public void PathStarted()
    {
        switch (state)
        {
            case ai_state.groundpatrol: _behaviourText.text = "GroundPatrol"; break;
            case ai_state.chase: _behaviourText.text = "Chase"; break;
        }
    }


    private void Chase()
    { //Add this method into AiController
        if (!PlayerInRange(6f, false)) //Change boolean to true for OnSight aggro
        {
            state = ai_state.groundpatrol;
            return;

        }
        if (PlayerInRange(1f, true)) //Change boolean to true for OnSight aggro
        {
            state = ai_state.attack;
            return;

        }

        _pathAgent.pathfindingTarget = player;
        state = ai_state.chase;
        _behaviourText.text = "Chase";

    }

    private void GroundPatrol(ref Vector2 input)
    {
        //Switch to chase if player in range
        if (PlayerInRange(6f, true)) //Change boolean to true for OnSight aggro
        {
            state = ai_state.chase;
            return;

        }

        _behaviourText.text = "Ground Patrol";


        //Coroutine for changing targeted platform
        if (_timerChangeTarget == false)
        {
            StartCoroutine(TimerForSwitchTarget());
        }
    }

    IEnumerator TimerForSwitchTarget()
    {
        _timerChangeTarget = true;
        yield return new WaitForSeconds(5f);
        _pathAgent.pathfindingTarget = _pathScript.GroundNodes[Random.Range(0, _pathScript.GroundNodes.Count)].gameObject;
        _pathAgent.RequestPath(_pathAgent.pathfindingTarget.transform.position + Vector3.up);
        Debug.Log(_pathAgent.pathfindingTarget.transform.position + Vector3.up);
        _timerChangeTarget = false;
    }

    private void AttackOnRange()
    {
        if (!PlayerInRange(1f, true)) //Change boolean to true for OnSight aggro
        {
            state = ai_state.chase;
            return;
        }
        //Debug.Log("I'm in my attack state");
        _behaviourText.text = "Attack";

        //Insert attack behavior
    }
}