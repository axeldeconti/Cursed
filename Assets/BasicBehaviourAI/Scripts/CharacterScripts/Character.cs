using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(Rigidbody2D))]       /*Used for collision detection*/
public class Character : MonoBehaviour
{
    public GameObject target;

    [System.NonSerialized]
    public GameObject _graphics;
    [System.NonSerialized]
    public AiController _ai;
    BoxCollider2D _box;
    [System.NonSerialized]
    public Animator _anim;
    Rigidbody2D _body;                        /*Used for collision detection*/
    PathfindingAgent _pathingAgent;
    CharacterController2D _controller;

    public moveStats movement;
    public jumpStats jump;
    public ledgeGrabStats ledgegrab;

    [System.NonSerialized]
    public float gravity;                     /*is calculated automatically inside jump.UpdateJumpHeight, future versions may contain optional jump/gravity/apex customizations*/
    Vector3 velocity;

    private bool facingRight = true;          /*determines the direction character is facing*/
    public bool jumped = false;              /*used for detecting if jump key was pressed (also used in ai)*/
    public bool isAiControlled = false;      /*allows ai to take control over inputs*/
    public bool playerControlled = false;    /*allows input by player*/
    public bool rightClickPathFind = false;  /*allows player to search for path by left click*/
    public bool FallNodes = true;            /*true-- Allows the pathfinding agent to use 'fall' nodes*/


    void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        _body = GetComponent<Rigidbody2D>();
        _box = GetComponent<BoxCollider2D>();
        _pathingAgent = GetComponent<PathfindingAgent>();
        _ai = GetComponent<AiController>();
        _anim = transform.Find("Graphics").GetComponent<Animator>();
        _graphics = transform.Find("Graphics").gameObject; /*useful for preventing things from flipping when character is facing left*/

        /*allow movement abilities to access character script*/
        ledgegrab.SetCharacter(this);
        jump.SetCharacter(this);

        _body.isKinematic = true;

    }

    void Start()
    {
        jump.UpdateJumpHeight();

        if (target)
        {
            isAiControlled = true; //allow character to be controlled by AI for when we recieve pathfinding
            _ai.state = AiController.ai_state.pathfinding; //set character AI type to pathfinding
            _pathingAgent.RequestPath(target.transform.position + Vector3.up);
        }
    }

    void Update()
    {

        if (playerControlled)
        {

            if (Input.GetKeyDown(KeyCode.Space) && !ledgegrab.ledgeGrabbed)
            {
                    jumped = true;        
            }


        }

        if (rightClickPathFind && Input.GetMouseButtonDown(1))
        { //GetKeyDown(KeyCode.C)) {//
            isAiControlled = true; //allow character to be controlled by AI for when we recieve pathfinding
            _ai.state = AiController.ai_state.pathfinding; //set character AI type to pathfinding
            _pathingAgent.RequestPath(Camera.main.ScreenToWorldPoint(Input.mousePosition)); //request a path and wait for instructions
        }
    }

    void FixedUpdate()
    {

        CooldownTicks();

        Vector2 input = Vector2.zero;
        if (playerControlled)
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (isAiControlled && (input.x != 0 || input.y != 0)) { if (isAiControlled) { _pathingAgent.CancelPathing(); } isAiControlled = false; _ai._behaviourText.text = ""; } /*turns off Ai control to avoid confusion user error*/
        }
        if (isAiControlled)
        {
            _ai.GetInput(ref velocity, ref input, ref jumped);
        }


        //Ledgegrabbing
        if (ledgegrab.ability)
        {
            if ((input.y == -1 || (facingRight != (input.x == 1) && input.x != 0)) && ledgegrab.ledgeGrabbed)
            {
                ledgegrab.StopLedgeGrab();
            }
            if ((input.y == 1 || Input.GetKey(KeyCode.Space)) && ledgegrab.ledgeGrabbed && ledgegrab.ledgeState == 0)
            {

                //start climb process, in fixed, we raycast in the direction we're facing and climbing upwards until no collision, then
                //we move towards direction we're looking until we have ground collision
                ledgegrab.ledgeState = 1;
            }
            if (input.y != -1 && !_controller.collisions.fallingThroughPlatform) { ledgegrab.LedgeGrabState(); }
        }


        //Jump
        if (jumped)
        {
            jumped = false;
            
            if (jump.ability && jump.jumpCount < jump.maxJumps) { velocity.y = jump.maxJumpVelocity; jump.jumpCount++; }
        }
        //Jump sensitivity
        if (jump.ability && Input.GetKeyUp(KeyCode.Space))
        { //think about adding an isCurrentlyJumping bool that gets reset to false on landing or other forces affecting y
            if (velocity.y > jump.minJumpVelocity)
            {
                velocity.y = jump.minJumpVelocity;
            }
        }


        if (!ledgegrab.ledgeGrabbed)
        {

            if (input.x > 0 && !facingRight)
            {
                Flip();
            }
            else if (input.x < 0 && facingRight)
            {
                Flip();
            }

            //Movement-x
            if (movement.ability)
            { //If character has the ability of moving
                float targetVelocityX = input.x * movement.moveSpeed;
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref movement.velocityXSmoothing, (_controller.collisions.below) ? movement.accelerationTimeGrounded : jump.accelerationTimeAirborne);
            }
            //Gravity
            if (velocity.y > -jump.maxFallVelocity)
            {
                velocity.y += gravity * Time.deltaTime;
            }
            _controller.Move(velocity * Time.deltaTime, input);
        }

        //animation
        _anim.SetFloat("speed", input.x != 0 ? 1f : 0f);
        _anim.SetBool("grounded", _controller.collisions.below);

        //Grounded + Jump reset
        if (_controller.collisions.below)
        {
            jump.jumpCount = 0;
        }
        if (_controller.collisions.above || _controller.collisions.below)
        {
            velocity.y = 0;

        }
        else
        { // else if the character falls off an edge, 1 jump is lost
            if (jump.jumpCount == 0) { jump.jumpCount++; }
        }
    }


    //changes characters facing direction
    private void Flip()
    {
        // Switch the way the player is labelled as facing
        facingRight = !facingRight;
        // Multiply the player's x local scale by -1
        Vector3 theScale = _graphics.transform.localScale;
        theScale.x *= -1; _graphics.transform.localScale = theScale;
    }

    //keep all timers that must be called in fixed update
    private void CooldownTicks()
    {

        if (ledgegrab.ledgeCooldownBool)
        {
            ledgegrab.fLedgeCooldown += Time.deltaTime;
            if (ledgegrab.fLedgeCooldown >= ledgegrab.ledgeCooldown)
            {
                ledgegrab.fLedgeCooldown = 0f;
                ledgegrab.ledgeCooldownBool = false;
            }
        }

    }

    //Movement Class Abilities
    public class movementEssentials
    {         /*gets inherited by movement abilities*/
        [System.NonSerialized]
        public Character _character;
        public void SetCharacter(Character c)
        {
            _character = c;
        }
    }
    [System.Serializable]
    public class jumpStats : movementEssentials
    {

        public bool ability = true;

        [SerializeField]
        private float maxHeight = 2.5f;
        [SerializeField]
        private float minHeight = 2.5f;

        public float maxJumpHeight
        {
            get { return maxHeight; }
            set { maxHeight = value; UpdateJumpHeight(); }
        }

        public float minJumpHeight
        {
            get { return minHeight; }
            set { minHeight = value; UpdateJumpHeight(); }
        }

        public float timeToApex = 0.435f;
        public float accelerationTimeAirborne = 0.09f;
        public float maxFallVelocity = 25f;
        public int maxJumps = 1;
        [System.NonSerialized]
        public int jumpCount = 0;

        [System.NonSerialized]
        public float maxJumpVelocity;
        [System.NonSerialized]
        public float minJumpVelocity;

        //If jump height changes during runtime, this must be called to adjust physics.
        public void UpdateJumpHeight()
        {
            _character.gravity = -(2 * maxHeight) / Mathf.Pow(timeToApex, 2);
            maxJumpVelocity = Mathf.Abs(_character.gravity) * timeToApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_character.gravity) * minHeight);
            //print("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);
        }
    }
    [System.Serializable]
    public class moveStats
    {

        public bool ability = true;
        public float accelerationTimeGrounded = 0.06f;
        public float moveSpeed = 4.87f;
        [System.NonSerialized]
        public float velocityXSmoothing;
    }


    [System.Serializable]
    public class ledgeGrabStats : movementEssentials
    {

        public bool ability = true;
        public float grabHeight = 0.2f;
        public float characterYPosition = -0.15f;
        public float ledgeClimbSpeed = 0.05f;
        public float ledgeMoveDistance = 0.35f;
        public float ledgeGrabCooldown = 0.3f;
        public float ledgeGrabDistance = 0.12f;
        public float ledgeCooldown = 0.8f;
        public bool ledgeGrabbed = false;

        [System.NonSerialized]
        public Vector2 grabbedCorner = Vector2.zero;
        [System.NonSerialized]
        public bool ledgeCooldownBool = false;
        [System.NonSerialized]
        public float fLedgeGrabCooldown;
        [System.NonSerialized]
        public float fLedgeCooldown;
        [System.NonSerialized]
        public int ledgeState = 0;


        public void LedgeGrabState()
        {

            if (!ledgeGrabbed && !_character._controller.collisions.below && _character.velocity.y <= 0 && !ledgeCooldownBool)
            {
                Vector2 direction = _character.facingRight ? Vector2.right : Vector2.right * -1f;
                Vector3 position = _character.transform.position;
                position.y += _character.transform.localScale.y * _character._box.size.y * 0.5f + characterYPosition + grabHeight;

                /*check collision (if no collision, the next collision check will determine if it can be grabbed)*/
                if (!Physics2D.Raycast(position, direction, ledgeGrabDistance + _character.transform.localScale.x * _character._box.size.x * 0.5f, _character._controller.collisionMask))
                {

                    RaycastHit2D hit = Physics2D.Raycast(_character.transform.position, direction, ledgeGrabDistance + _character.transform.localScale.x * _character._box.size.x * 0.5f, _character._controller.collisionMask);
                    if (hit && hit.collider.tag != "oneway")
                    { /*check collision beside character, if its oneway, ignore*/

                        RaycastHit2D vertHit = Physics2D.Raycast(_character.transform.position, Vector2.up, _character.transform.localScale.y * _character._box.size.y, _character._controller.collisionMask);
                        if (!vertHit || vertHit.collider.tag == "oneway")
                        { /*check collision above character, if its oneway, ignore*/

                            Vector3 reposition = Vector3.zero;
                            reposition.x = hit.point.x - (direction.x * _character.transform.localScale.x * _character._box.size.x * 0.5f + 0.01f);
                            reposition.y = hit.collider.transform.position.y + hit.collider.transform.localScale.y * _character._box.size.y * 0.5f;
                            grabbedCorner = new Vector2(hit.point.x, reposition.y);
                            reposition.y -= (characterYPosition + _character.transform.localScale.y * _character._box.size.y * 0.5f);
                            ledgeGrabbed = true;
                            _character._anim.SetBool("ledge", true);
                            _character.transform.position = reposition;

                            Debug.DrawRay(position, direction, Color.red, 2f);
                            Debug.DrawRay(_character.transform.position, Vector2.up, Color.red, 2f);
                        }
                    }
                }

            }

            if (ledgeGrabbed && ledgeState != 0 && !ledgeCooldownBool)
            {

                Vector2 direction = _character.facingRight ? Vector2.right : Vector2.right * -1f;

                if (ledgeState == 1)
                {
                    _character._anim.SetBool("ledgeClimbing", true);

                    _character._controller.Test(new Vector3(0.0001f, 0.001f, 0f));
                    if (_character._controller.collisions.above)
                    {

                        StopLedgeGrab();
                    }
                    if (_character.transform.position.y - _character.transform.localScale.y * _character._box.size.y * 0.5f < grabbedCorner.y)
                    {
                        Vector3 newPos = _character.transform.position;
                        newPos.y += ledgeClimbSpeed;
                        _character.transform.position = newPos;
                    }
                    else
                    {
                        ledgeState = 2;
                    }

                }
                if (ledgeState == 2)
                {

                    Vector3 newPos = _character.transform.position;
                    newPos.x += direction.x * ledgeClimbSpeed;
                    _character.transform.position = newPos;


                    _character._controller.Test(new Vector3(direction.x * 0.01f, 0, 0f));
                    if (direction.x == 1)
                    {
                        if (_character._controller.collisions.right || (_character.transform.position.x - _character.transform.localScale.x * _character._box.size.x * 0.5f) > grabbedCorner.x)
                        {

                            StopLedgeGrab();
                        }
                    }
                    if (direction.x == -1)
                    {
                        if (_character._controller.collisions.left || (_character.transform.position.x + _character.transform.localScale.x * _character._box.size.x * 0.5f) < grabbedCorner.x)
                        {

                            StopLedgeGrab();
                        }
                    }

                }
            }
        }

        public void StopLedgeGrab()
        {
            if (ledgeGrabbed)
            {
                ledgeGrabbed = false;
                ledgeState = 0;
                _character._anim.SetBool("ledge", false);
                _character._anim.SetBool("ledgeClimbing", false);
                ledgeCooldownBool = true;
                _character.velocity.x = 0;
                _character.velocity.y = 0;
            }
        }

    }
    
}