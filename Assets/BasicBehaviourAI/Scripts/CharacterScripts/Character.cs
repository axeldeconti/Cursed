using UnityEngine;
using Cursed.Character;


    [RequireComponent(typeof(CharacterController2D))]
    [RequireComponent(typeof(Rigidbody2D))]       /*Used for collision detection*/
    public class Character : MonoBehaviour
    {
        [System.NonSerialized]
        public GameObject _graphics;
        [System.NonSerialized]
        public AiController _ai;
        BoxCollider2D _box;
        [System.NonSerialized]
        //public Animator _anim;
        PathfindingAgent _pathAgent;
        CharacterController2D _controller;
        private IInputController _input;

        public moveStats movement;
        public jumpStats jump;

        [System.NonSerialized]
        public float gravity;                     /*is calculated automatically inside jump.UpdateJumpHeight, future versions may contain optional jump/gravity/apex customizations*/
        Vector3 velocity;

        private bool _facingRight = true;        /*determines the direction character is facing*/
        public bool jumped = false;              /*used for detecting if jump key was pressed (also used in ai)*/
        public bool isAiControlled = false;      /*allows ai to take control over inputs*/
        public bool playerControlled = false;    /*allows input by player*/
        public bool fallNodes = true;            /*true-- Allows the pathfinding agent to use 'fall' nodes*/

        void Awake()
        {
            _controller = GetComponent<CharacterController2D>();
            _box = GetComponent<BoxCollider2D>();
            _pathAgent = GetComponent<PathfindingAgent>();
            _ai = GetComponent<AiController>();
            //_anim = transform.Find("Graphics").GetComponent<Animator>();
            _graphics = transform.Find("Graphics").gameObject; /*useful for preventing things from flipping when character is facing left*/
            _input = GetComponent<IInputController>();

            /*allow movement abilities to access character script*/
            jump.SetCharacter(this);
        }

        void Start()
        {
            jump.UpdateJumpHeight();
        }

        void Update()
        {
            if (playerControlled)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    jumped = true;
                }
        }
        }

        void FixedUpdate()
        {
            Vector2 input = Vector2.zero;
            if (playerControlled)
            {
                input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                if (isAiControlled && (input.x != 0 || input.y != 0)) { if (isAiControlled) { _pathAgent.CancelPathing(); } isAiControlled = false; } /*turns off Ai control to avoid confusion user error*/
            }
            if (isAiControlled)
            {
                _ai.GetInput(ref velocity, ref input, ref jumped);

                //A changer
                /*if (jumped)
                    _input.Jump.Trigger();

                if (input.x != 0)
                {
                    ;
                }
                if (input.y != 0)
                {
                    ;
                }*/
                //........
                //........
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
            
            if (input.x > 0 && !_facingRight)
            {
                Flip();
            }
            else if (input.x < 0 && _facingRight)
            {
                Flip();
            }

            //Movement-x
            if (movement.ability)
            { //If character has the ability of moving
                float targetVelocityX = input.x * movement.moveSpeed;
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref movement.velocityXSmoothing, 0);                
            }
            //Gravity
            if (velocity.y > -jump.maxFallVelocity)
            {
                velocity.y += gravity * Time.deltaTime;
            }
            _controller.Move(velocity * Time.deltaTime, input);

            //animation
            //_anim.SetFloat("speed", input.x != 0 ? 1f : 0f);
            //_anim.SetBool("grounded", _controller.collisions.below);

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
            _facingRight = !_facingRight;
            // Multiply the player's x local scale by -1
            Vector3 theScale = _graphics.transform.localScale;
            theScale.x *= -1; _graphics.transform.localScale = theScale;
        }

        //Movement Class Abilities
        public class movementEssentials
        {
            /*gets inherited by movement abilities*/
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
            private float _maxHeight = 2.5f;
            [SerializeField]
            private float _minHeight = 2.5f;

            public float maxJumpHeight
            {
                get { return _maxHeight; }
                set { _maxHeight = value; UpdateJumpHeight(); }
            }

            public float minJumpHeight
            {
                get { return _minHeight; }
                set { _minHeight = value; UpdateJumpHeight(); }
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
                _character.gravity = -(2 * _maxHeight) / Mathf.Pow(timeToApex, 2);
                maxJumpVelocity = Mathf.Abs(_character.gravity) * timeToApex;
                minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_character.gravity) * _minHeight);
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
    }