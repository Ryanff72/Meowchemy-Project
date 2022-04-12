using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField]
    private float accelSpeed;
    [SerializeField]
    private float decelSpeed;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float jumpHeight;
    [SerializeField]
    public float gravity;
    [SerializeField]
    public float gravityJump;
    [SerializeField]
    public float gravityFall;
    [SerializeField]
    public Vector2 velocity;

    [Header("Game Objects")]
    public GameObject leftGc;
    public GameObject rightGc;
    public GameObject wcbr;
    public GameObject wctr;
    public GameObject wcbl;
    public GameObject wctl;
    

    [Header("Components")]
    public Rigidbody2D rb2d;

    [Header("Bools")]
    public bool grounded; //a bool determined by a raycast checking for the ground
    private bool earlyJumpTriggered; //a bool that is set true when the player hits the jump button when in the air, and is set false when a short window of time passes.
    public bool moveleft;
    public bool moveright;
    public bool limitSpeed;
    public bool canMoveLeft;
    public bool canMoveRight;
    private bool setyvelzero; //checks if the y velocity is zero when the player is grounded. if it is not, it sets it to be.
    private bool hasSpawnedDashFX = false;

    [Header("Floats")]
    private float edgeJumpTimer; //the time that the player can jump after leaving an edge
    private float earlyJumpTimer; //the time that the player has before hitting the ground to jump

    [Header("Strings")]
    public string gotToAirBy = "jumping";
    private string fatFallDir = "none";


    public enum PlayerState {neutral};
    public PlayerState ps;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        ps = PlayerState.neutral;
        //control helper
        if (Controls.JumpButtonName == KeyCode.None)
        {
            Controls.JumpButtonName = KeyCode.Space;
        }
    }

    //statemachine
    public void StateMachine()
    {
        switch (ps)
        {
            case PlayerState.neutral:
                JumpAble();
                Movement();
                break;
        }
    }
    //for moving left/right
    private void FixedUpdate()
    {
         //this stuff stops the player when they stop moving
         if ((Input.GetAxis("Horizontal") == 0) || (canMoveLeft == false || canMoveRight == false))
         {
             float xVelocity = 0;
             velocity = new Vector2(Mathf.SmoothDamp(velocity.x, 0, ref xVelocity, decelSpeed), velocity.y);
         }

        if (moveright)
        {
            float xVelocity = 0;
            velocity.x = Mathf.SmoothDamp(velocity.x, maxSpeed, ref xVelocity, accelSpeed);
        }
        if (moveleft)
        {
            float xVelocity = 0;
            velocity.x = Mathf.SmoothDamp(velocity.x, -maxSpeed, ref xVelocity, accelSpeed);
        }
        
    }
    void Update()
    {
        //runs da state machine bc you know
        StateMachine();
      
    }
    void Movement()
    {
        //checks
            //check for wall on right
            RaycastHit2D WallCheckBottomRight = Physics2D.Linecast(wcbr.transform.position, wcbr.transform.position - new Vector3(0, 0, 0), 1 << LayerMask.NameToLayer("Ground"));
            RaycastHit2D WallCheckTopRight = Physics2D.Linecast(wctr.transform.position, wctr.transform.position - new Vector3(0, 0, 0), 1 << LayerMask.NameToLayer("Ground"));
            RaycastHit2D WallCheckBottomLeft = Physics2D.Linecast(wcbl.transform.position, wcbl.transform.position - new Vector3(0, 0, 0), 1 << LayerMask.NameToLayer("Ground"));
            RaycastHit2D WallCheckTopLeft = Physics2D.Linecast(wctl.transform.position, wctl.transform.position - new Vector3(0, 0, 0), 1 << LayerMask.NameToLayer("Ground"));
            if (WallCheckBottomRight == true || WallCheckTopRight == true)
            {
                canMoveRight = false;
            }
            else
            {
                canMoveRight = true;
            }
            if (WallCheckBottomLeft == true || WallCheckTopLeft == true)
            {
                canMoveLeft = false;
            }
            else
            {
                canMoveLeft = true;
            }
            //check for ground
            RaycastHit2D GroundCheckLeft = Physics2D.Linecast(leftGc.transform.position, leftGc.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
            RaycastHit2D GroundCheckRight = Physics2D.Linecast(rightGc.transform.position, rightGc.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
            if (GroundCheckLeft.collider != null || GroundCheckRight.collider != null)
            {
                if (earlyJumpTriggered == true)
                {
                    earlyJumpTimer = 0.0f;
                    Invoke("Jump", 0.01f);
                    gotToAirBy = "jumping";
                }
                edgeJumpTimer = 0.0f;
                gravity = gravityJump;
                grounded = true;
                earlyJumpTriggered = false;
                //this bit of code is to ensure that when the player leaves the ground via edge the code does not think that the player jumped.
                if (gotToAirBy == "jumping")
                {
                    gotToAirBy = "falling";
                }
                limitSpeed = true;
                //reset y vel
                if (setyvelzero == false && rb2d.velocity.y <= 0.5f)
                {
                    velocity.y = 0;
                    setyvelzero = true;
                }
                //if (GroundCheckLeft.collider.gameObject.tag != null || GroundCheckRight.collider.gameObject.tag != null)
                //{
                //    if (GroundCheckLeft.collider.gameObject.tag == "Platform")
                //    {
                //        transform.SetParent(GroundCheckLeft.collider.transform);
                //    }
                //    if (GroundCheckRight.collider.gameObject.tag == "Platform")
                //    {
                //        transform.SetParent(GroundCheckLeft.collider.transform);
                //    }
                //}
                //else
                //{
                //    transform.SetParent(null);
                //}
            }
            else
            {
                if (setyvelzero == true)
                {
                    setyvelzero = false;
                }
                //gravity
                velocity.y += gravity * Time.deltaTime;
                edgeJumpTimer += Time.deltaTime;
                grounded = false;
            }

        //movement
        rb2d.velocity = velocity;
        if (Input.GetAxis("Horizontal") > 0)
        {

            if (velocity.x < maxSpeed && canMoveRight == true)
            {
                moveright = true;
            }
            else if (limitSpeed == true && canMoveRight == true)
            {
                velocity = new Vector2(maxSpeed, velocity.y);
            }
        }
        else
        {
            moveright = false;
        }
        if (Input.GetAxis("Horizontal") < 0)
        {

            if (velocity.x > -maxSpeed && canMoveLeft == true)
            {
                moveleft = true;
            }
            else if (limitSpeed == true && canMoveLeft == true)
            {
                velocity = new Vector2(-maxSpeed, velocity.y);
            }

        }
        else
        {
            moveleft = false;

        }
    }
    void JumpAble()
    {
        //the code for jumping/edge jumping
        if (((Input.GetButtonDown("Jump") || Input.GetKeyDown(Controls.JumpButtonName)) && (grounded == true)) || (((Input.GetButtonDown("Jump") || Input.GetKeyDown(Controls.JumpButtonName))) && (edgeJumpTimer < 0.1f) && gotToAirBy == "falling"))
        {
            Jump();
        }
        else if (grounded == false)
        {
            if (Input.GetButtonDown("Jump") || Input.GetKeyDown(Controls.JumpButtonName))
            {
                earlyJumpTriggered = true;
            }
        }


        //early jump timer
        if (earlyJumpTriggered)
        {
            earlyJumpTimer += Time.deltaTime;
            if (earlyJumpTimer > 0.1f)
            {
                earlyJumpTriggered = false;
                earlyJumpTimer = 0.0f;
            }
        }

        //the early jump is triggered here. for a short period after this variable is toggled, if the player hits the ground they will jump.
        
        
        //shrinks the upward force when releasing space (causes jumps to vary in height)
        if (Input.GetButtonUp("Jump") || Input.GetKeyUp(Controls.JumpButtonName)) // && rb2d.velocity.y > 0)
        {
            gravity = gravityFall;
        }

    }

    void Jump()
    {
        gotToAirBy = "jumping";
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(Controls.JumpButtonName))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        else
        {
            velocity.y = Mathf.Sqrt(jumpHeight * gravity * -2f);
        }
    }

   

  
}
