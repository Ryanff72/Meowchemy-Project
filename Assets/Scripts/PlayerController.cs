using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


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
    float savedMaxSpeed;//for when the player is holding a gun
    float savedJumpHeight;

    [Header("Game Objects")]
    public GameObject leftGc;
    public GameObject rightGc;
    public GameObject wcbr;
    public GameObject wctr;
    public GameObject wcbl;
    public GameObject wctl;
    public GameObject ccr;
    public GameObject ccl;
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject spriteParent;
    public GameObject landingSmoke;
    public GameObject pickupAbleWeapon;
    public GameObject leftPartGun;
    public GameObject rightPartGun;
    public GameObject dogGun;
    public TextMeshProUGUI ammoText;
    public GameObject crushEffect;
    private GameObject InstantiatedWeapon;
    [SerializeField] private GameObject WeaponPickup;
    
    

    [Header("Components")]
    public Rigidbody2D rb2d;
    public ParticleSystem walkingSmoke;


    [Header("Bools")]
    public bool grounded; //a bool determined by a raycast checking for the ground
    private bool earlyJumpTriggered; //a bool that is set true when the player hits the jump button when in the air, and is set false when a short window of time passes.
    public bool moveleft;
    public bool moveright;
    public bool limitSpeed;
    public bool canMoveLeft;
    public bool canMoveRight;
    private bool setyvelzero; //checks if the y velocity is zero when the player is grounded. if it is not, it sets it to be.
    private bool hasSquished = false;
    private bool hasSpawnedLandingFX;
    public bool canPickUpWeapon;
    public bool hasWeapon;
    bool wasOnPlatform;

    [Header("Floats")]
    private float edgeJumpTimer; //the time that the player can jump after leaving an edge
    private float earlyJumpTimer; //the time that the player has before hitting the ground to jump

    [Header("Strings")]
    public string gotToAirBy = "jumping";

    [Header("Misc")]
    private Animator anim;
    public enum PlayerState {neutral, frozen, dead};
    public PlayerState ps;
    public Transform respawnPos;
    private bool velHasDiminished;
    //private bool enemyInKillingPosition = false;
    public int ammoCount;
    float crushtime;

    void Start()
    {
        anim = spriteParent.GetComponent<Animator>();
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
            case PlayerState.frozen:
                Freeze();
                break;
            case PlayerState.dead:
                Dead();
                break;
        }
    }
    //for moving left/right
    private void FixedUpdate()
    {
        if (ps != PlayerState.dead)
        {
            //this stuff stops the player when they stop moving
            if ((Input.GetAxis("Horizontal") == 0) || (canMoveLeft == false || canMoveRight == false))
            {
                float xVelocity = 0;
                velocity = new Vector2(Mathf.SmoothDamp(velocity.x, 0, ref xVelocity, decelSpeed), velocity.y);
                if (grounded)
                {
                    anim.SetBool("Run", false);
                }

                

            }

            if (moveright)
            {
                float xVelocity = 0;
                velocity.x = Mathf.SmoothDamp(velocity.x, maxSpeed, ref xVelocity, accelSpeed);
                spriteParent.transform.localScale = new Vector3(1, 1, 1);
                if (grounded)
                {
                    anim.SetBool("Run", true);
                }
            }
            if (moveleft)
            {
                float xVelocity = 0;
                velocity.x = Mathf.SmoothDamp(velocity.x, -maxSpeed, ref xVelocity, accelSpeed);
                spriteParent.transform.localScale = new Vector3(-1, 1, 1);
                if (grounded)
                {
                    anim.SetBool("Run", true);
                }
            }
        }
         
    }
    void Update()
    {
        //runs da state machine bc you know
        StateMachine();

        RaycastHit2D GroundCheck = Physics2D.Linecast(leftGc.transform.position, rightGc.transform.position, 1 << LayerMask.NameToLayer("Ground"));
        if (GroundCheck.collider != null)
        {
            grounded = true;
            
            if (hasSpawnedLandingFX == false)
            {
                hasSpawnedLandingFX = true;
                Instantiate(landingSmoke, transform.GetChild(0).transform.position + new Vector3(0,-0.6f,0), Quaternion.Euler(-90, 0, 0));
                
            }
            if (GroundCheck.collider.gameObject.tag == "Platform")
            {
                transform.SetParent(GroundCheck.collider.gameObject.transform);
                wasOnPlatform = true;
            }
            else
            {
                transform.parent = null;
                wasOnPlatform = false;
            }
            StartCoroutine("SquishOnLand");//a coroutine that makes a squish animation
        }
        else
        {
            transform.SetParent(null);
            hasSpawnedLandingFX = false;
            if (wasOnPlatform == true)
            {
                wasOnPlatform = false;
                if (velocity.y < 0.1)
                {
                    velocity.y = 0;
                }
                

            }
            grounded = false;
            hasSquished = false;
        }
        if (Input.GetKey("r"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetButtonDown("Interact") && canPickUpWeapon == true && hasWeapon == false)
        {
            anim.SetBool("HasGun", true);
            ammoCount = pickupAbleWeapon.GetComponent<GunPickupScript>().ammo;
            hasWeapon = true;
            savedMaxSpeed = maxSpeed;
            maxSpeed = 5;
            savedJumpHeight = jumpHeight;
            jumpHeight = 1;
            Destroy(pickupAbleWeapon.transform.parent.gameObject);
            dogGun.gameObject.SetActive(true);
            transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (hasWeapon == true && Input.GetButtonDown("Interact"))
        {
            anim.SetBool("HasGun", false);
            RaycastHit2D CheckRight = Physics2D.Linecast(transform.position, transform.position + new Vector3(1,0, 0), 1 << LayerMask.NameToLayer("Ground"));
            RaycastHit2D CheckLeft = Physics2D.Linecast(transform.position, transform.position + new Vector3(-1,0, 0), 1 << LayerMask.NameToLayer("Ground"));
            if (CheckLeft.collider != null)
            {
                InstantiatedWeapon = Instantiate(WeaponPickup, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
            else if (CheckRight.collider != null)
            {
                InstantiatedWeapon = Instantiate(WeaponPickup, transform.position + new Vector3(-1, 1, 0), Quaternion.identity);
            }
            else
            {
                InstantiatedWeapon = Instantiate(WeaponPickup, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
            
            InstantiatedWeapon.transform.GetChild(0).GetComponent<GunPickupScript>().ammo = ammoCount;
            InstantiatedWeapon.GetComponent<SimpleBoxObjectPhysics>().velocity = velocity;
            maxSpeed = savedMaxSpeed;
            jumpHeight = savedJumpHeight;
            dogGun.gameObject.SetActive(false);
            transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = true;
            hasWeapon = false;
        }
        if (hasWeapon == true)
        {
            ammoText.text = "x" + ammoCount;
        }
        if (ammoCount <= 0 && hasWeapon == true)
        {
            hasWeapon = false;
            anim.SetBool("HasGun", false);
            maxSpeed = savedMaxSpeed;
            jumpHeight = savedJumpHeight;
            dogGun.gameObject.SetActive(false);
            transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = true;
            GameObject InstantiatedLeftPart = Instantiate(leftPartGun, transform.position, Quaternion.identity);
            GameObject InstantiatedRightPart = Instantiate(rightPartGun, transform.position, Quaternion.identity);
            if (anim.gameObject.transform.localScale.x > 0)
            {
                InstantiatedLeftPart.GetComponent<SpriteRenderer>().flipX = true;
                InstantiatedRightPart.GetComponent<SpriteRenderer>().flipX = true;
                InstantiatedRightPart.GetComponent<Rigidbody2D>().velocity = new Vector2(-5, 5);
                InstantiatedLeftPart.GetComponent<Rigidbody2D>().velocity = new Vector2(5, 5);
            }
            else
            {
                InstantiatedRightPart.GetComponent<Rigidbody2D>().velocity = new Vector2(5, 5);
                InstantiatedLeftPart.GetComponent<Rigidbody2D>().velocity = new Vector2(-5, 5);
            }
            
        }

    }
    void Movement()
    {
        //checks
        //check for wall in various directions
        RaycastHit2D WallCheckRight = Physics2D.Linecast(wcbr.transform.position, wctr.transform.position - new Vector3(0, 0, 0),1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D WallCheckLeft = Physics2D.Linecast(wcbl.transform.position, wctl.transform.position - new Vector3(0, 0,0),1<<LayerMask.NameToLayer("Ground"));
        RaycastHit2D CeilingCheck = Physics2D.Linecast(ccr.transform.position, ccl.transform.position - new Vector3(0, 0.1f, 0),1 << LayerMask.NameToLayer("Ground"));
        if (WallCheckRight == true)
        {
            canMoveRight = false;
            moveright = false;

            if (WallCheckRight.collider.gameObject.tag == "Platform")
            {
                transform.position = new Vector3(transform.position.x - 0.1f, transform.position.y, 0);
            }
        }
        else
        {
            canMoveRight = true;

        }
        if (WallCheckLeft == true)
        {
            canMoveLeft = false;
            moveleft = false;

            if (WallCheckLeft.collider.gameObject.tag == "Platform")
            {
                transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y,0);
            }
        }
        else
        {
            canMoveLeft = true;
        }
        if (CeilingCheck.collider != null)
        {
            velocity = new Vector2(velocity.x, Mathf.Abs(velocity.y) * -0.7f);
        }
        if (CeilingCheck.collider != null && grounded == true)
        {
            
            crushtime += Time.deltaTime;
            if (crushtime > 0.04f) 
            {
                Crushed();
            }

        }
        else
        {
            crushtime = 0;
        }
        //check for ground
        
        if (grounded == true)
        {
            if (gotToAirBy == "jumping" && transform.parent == null)
            {
                gotToAirBy = "falling";
            }
            if (earlyJumpTriggered == true)
            {
                earlyJumpTimer = 0.0f;
                Invoke("Jump", 0.01f);
                gotToAirBy = "jumping";
            }
            edgeJumpTimer = 0.0f;
            gravity = gravityJump;
            grounded = true;
            
            anim.SetBool("Jump", false);
            earlyJumpTriggered = false;
            limitSpeed = true;
            //reset y vel
            if (setyvelzero == false && rb2d.velocity.y <= 0.5f && transform.parent == null)
            {
                velocity.y = 0;
                setyvelzero = true;
            }
            else if (transform.parent != null)
            {
               // velocity.y = rb2d.velocity.y;
            }
            anim.SetBool("JumpDown", false);
            //this bit of code is to ensure that when the player leaves the ground via edge the code does not think that the player jumped.
            
        }
        else
        {
            if (setyvelzero == true)
            {
                setyvelzero = false;
            }
             //gravity
            anim.SetBool("Jump", true); 
            if (velocity.y < 0)
            {
                anim.SetBool("JumpDown", true);
            }
            else
            {
                anim.SetBool("JumpDown", false);
            }
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
        // smoke walking
        if (grounded && (moveleft == true || moveright == true))
        {
            //ParticleSystem.EmissionModule em = walkingSmoke.emission;
            //em.enabled = true;
        }
        else if (grounded == false || (moveright == false && moveleft == false))
        {
            //ParticleSystem.EmissionModule em = walkingSmoke.emission;
            //em.enabled = false;
        }
        // killing stuff, removed for the purpose of keeping the player from overusing lol
        //RaycastHit2D EnemyCheckLeft = Physics2D.Linecast(transform.position, transform.position + new Vector3(-3, 0, 0), 1 << //LayerMask.NameToLayer("Enemy"));
        //RaycastHit2D EnemyCheckRight = Physics2D.Linecast(transform.position, transform.position + new Vector3(3, 0, 0), 1 << //LayerMask.NameToLayer("Enemy"));
        //if (EnemyCheckLeft.collider != null && spriteParent.transform.localScale.x < 0)
        //{
        //    enemyInKillingPosition = true;
        //}
        //else if (EnemyCheckRight.collider != null && spriteParent.transform.localScale.x > 0)
        //{
        //    enemyInKillingPosition = true;
        //}
        //else
        //{
        //    enemyInKillingPosition = false;
        //}
        //if (moveleft == false && moveright == false && Input.GetKeyDown(KeyCode.E) && enemyInKillingPosition == true && //spriteParent.transform.localScale.x < 0)
        //{
        //    if (EnemyCheckLeft.collider.gameObject.GetComponent<AIBase>().aiState != AIBase.AIState.aggro && //EnemyCheckLeft.collider.gameObject.GetComponent<AIBase>().aiState != AIBase.AIState.dead)
        //    {
        //        canMoveLeft = false;
        //        canMoveRight = false;
        //        GameObject HitEnemy = EnemyCheckLeft.collider.gameObject;
        //        EnemyCheckLeft.collider.gameObject.GetComponent<AIBase>().velocity = new Vector2(0, 0);
        //        StartCoroutine("KillEnemy", HitEnemy);
        //    }
        //    
        //}
        //else if (moveleft == false && moveright == false && Input.GetKeyDown(KeyCode.E)  && enemyInKillingPosition == true && //spriteParent.transform.localScale.x > 0)
        //{
        //    if (EnemyCheckRight.collider.gameObject.GetComponent<AIBase>().aiState != AIBase.AIState.aggro && //EnemyCheckRight.collider.gameObject.GetComponent<AIBase>().aiState != AIBase.AIState.dead)
        //    {
        //        canMoveLeft = false;
        //        canMoveRight = false;
        //        GameObject HitEnemy = EnemyCheckRight.collider.gameObject;
        //        EnemyCheckRight.collider.gameObject.GetComponent<AIBase>().velocity = new Vector2(0, 0);
        //        StartCoroutine("KillEnemy", HitEnemy);
        //    }
        //    
        //}
    }
    void Dead()
    {
        bool nearGrounded;
        rb2d.velocity = velocity;
        if (velocity.x > 0)
        {
            anim.gameObject.transform.localScale = new Vector3(1, 1, 0.5f);
        }
        else
        {
            anim.gameObject.transform.localScale = new Vector3(1, -1, 0.5f);
        }
        canPickUpWeapon = false;
        if (hasWeapon)
        {
            dogGun.gameObject.SetActive(false);
            hasWeapon = false;
            GameObject droppedWeapon = Instantiate(WeaponPickup, transform.position+new Vector3(0,1,0), Quaternion.identity);
            droppedWeapon.transform.GetComponent<SimpleBoxObjectPhysics>().velocity = velocity * 0.7f;
        }
        transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().size = new Vector2(1.5f, 1);
        transform.GetChild(1).GetChild(0).transform.localPosition = new Vector3(-0.0109999999f, 1.08899999f, 0);
        leftGc.transform.localPosition = new Vector3(-0.74f, -0.157f, 0);
        rightGc.transform.localPosition = new Vector3(0.74f, -0.157f, 0);
        wcbr.transform.localPosition = new Vector3(0.78f, -0.102f, 0);
        wcbl.transform.localPosition = new Vector3(-0.78f, -0.102f, 0);
        wctl.transform.localPosition = new Vector3(-0.78f, 0.835f, 0);
        wctr.transform.localPosition = new Vector3(0.78f, 0.835f, 0);
        ccl.transform.localPosition = new Vector3(-0.726f, 0.898f, 0);
        ccr.transform.localPosition = new Vector3(0.726f, 0.898f, 0);
        RaycastHit2D NearGroundCheck = Physics2D.Linecast(leftGc.transform.position+ new Vector3(0,-0.1f,0), rightGc.transform.position + new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D GroundCheck = Physics2D.Linecast(leftGc.transform.position, rightGc.transform.position, 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D WallCheckRight = Physics2D.Linecast(wcbr.transform.position, wctr.transform.position, 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D WallCheckLeft = Physics2D.Linecast(wcbl.transform.position, wctl.transform.position, 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D CeilingCheck = Physics2D.Linecast(ccr.transform.position, ccl.transform.position, 1 << LayerMask.NameToLayer("Ground"));
        if (NearGroundCheck.collider != null)
        {
            nearGrounded = true;
            velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * 3f);
        }
        else
        {
            nearGrounded = false;
        }
        if (GroundCheck.collider != null)
        {
            if (velocity.y < -15f)
            {
                velocity.y = Mathf.Abs(velocity.y);
                velHasDiminished = false;
            }
            else
            {
                velocity.y = Mathf.Lerp(velocity.y, -1, Time.deltaTime * 10);
                velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * 2f);
            }
             
            
        }
        else if (velHasDiminished == false)
        {
            velHasDiminished = true;
            velocity.x *= 0.5f;
            velocity.y *= 0.5f;
        }
        if (WallCheckLeft.collider != null)
        {
            velocity.x = Mathf.Abs(velocity.x);
        }
        if (WallCheckRight.collider != null)
        {
            velocity.x = -Mathf.Abs(velocity.x);
        }
        if (CeilingCheck.collider != null)
        {
            velocity.y = -Mathf.Abs(velocity.y);
        }
        if (velocity.y >= -4 && nearGrounded == false)
        {
            anim.SetBool("DeadUp", true);
            anim.SetBool("DeadDown", false);
        }
        else if (nearGrounded == false)
        {
            anim.SetBool("DeadUp", false);
            anim.SetBool("DeadDown", true);
        }
        if (grounded == true || nearGrounded == true)
        {
            anim.SetBool("DeadUp", true);
            anim.SetBool("DeadDown", false);
        }
        if (nearGrounded == false)//workaround for gravity being wonky
        {
            velocity.y += gravity * Time.deltaTime;
        }
        rb2d.velocity = velocity;
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
        if (Input.GetButtonUp("Jump") || Input.GetKeyUp(Controls.JumpButtonName) || (rb2d.velocity.y <= 0 && edgeJumpTimer>0.1f)) // && rb2d.velocity.y > 0)
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
            anim.gameObject.transform.localScale = new Vector3(1, 1f, 1);
        }
        
    }

    public void Freeze()
    {
        velocity = new Vector2(0, 0);
        rb2d.velocity = new Vector2(0, 0);
    }

    public IEnumerator Respawn()
    {
        ps = PlayerState.frozen;
        yield return new WaitForSeconds(0.5f);
        transform.position = respawnPos.position;
        ps = PlayerState.neutral;
    }

   // private IEnumerator KillEnemy(GameObject hitEnemy)
   // {
   //         anim.SetBool("IsKilling", true);
   //         yield return new WaitForSeconds(0.65f);
   //         if (hitEnemy.GetComponent<AIBase>().aiState != AIBase.AIState.aggro && enemyInKillingPosition == true)
   //         {
   //             hitEnemy.GetComponent<AIBase>().aiState = AIBase.AIState.dead;
   //         }
   //         else
   //         {
   //             anim.SetBool("FailedKill", true);
   //         }
   //         yield return new WaitForSeconds(0.8f);
   //         anim.SetBool("FailedKill", false);
   //         anim.SetBool("IsKilling", false);
   //
   //     
   // }

    private IEnumerator SquishOnLand()
    {
        if (hasSquished == false)
        {
            
            anim.gameObject.transform.parent.transform.localScale = Vector3.Lerp(anim.gameObject.transform.parent.transform.localScale, new Vector3(1.15f, 0.85f, 1), Time.deltaTime * 45f);
            yield return new WaitForSeconds(0.08f);
            hasSquished = true;

        }
        anim.gameObject.transform.parent.transform.localScale = Vector3.Lerp(anim.gameObject.transform.parent.transform.localScale, new Vector3(1, 1f, 1), Time.deltaTime * 45f);
        
    }
   
    public void Crushed()
    {
        ps = PlayerState.dead;
        rb2d.bodyType = RigidbodyType2D.Static;
        transform.GetChild(1).gameObject.SetActive(false);
        Instantiate(crushEffect, transform.position, Quaternion.identity);
        GetComponent<BoxCollider2D>().enabled = false;
    }
  
}
