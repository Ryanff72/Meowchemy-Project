
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBase : MonoBehaviour
{
    // its satuday, garfelf's second favorit da

    [Header("Game Objects")]
    public GameObject leftGc;
    public GameObject rightGc;
    public GameObject WCLL;
    public GameObject WCHL;
    public GameObject WCLR;
    public GameObject WCHR;
    public GameObject CCR;
    public GameObject CCL;

    [Header("PatrolSettings")]
    public float leftLimit;
    public float rightLimit;
    public float breakTime; //time a guard will wait when they reach a point
    public float breakRng; //randomness allowed in breaktime, in seconds
    [SerializeField] private string moveDir;
    public float canSee;
    //[SerializeField] private FieldOfView fieldOfView;

    //[Header("Misc")]

    public enum AIState { idle, suspicious, boundedPatrol, aggro, dead };
    public AIState aiState;
    public Vector2 velocity;
    public float gravity;
    public float speed;
    public float aggroSpeed;
    private Rigidbody2D rb2d;
    [SerializeField] bool grounded;
    bool hasSetAggro = false;
    public Vector2 JumpTimeRange;
    public Vector2 JumpHeightRange;
    public Vector2 ShotFireTimeRange;
    public Transform shotPos;
    public GameObject Projectile;
    private float jumpTimer;
    private float shotTimer;
    public int shotCount;
    public float ProjectileSpeed;
    public float currentSuspicion; //now sus the enemy is
    private float suspicionTriggerLevel = 100;
    private bool queueJump = false;
    public Animator anim;
    [SerializeField] FieldOfViewScript fovScript;
    public GameObject angerBar;
    //helps gravity work correctly
    bool setyVel0;
    public bool killedByOtherAI;
    string isPaused = "unassigned"; // for suspicion state
    bool velHasDiminished = false;//to help the bouncing after death

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        jumpTimer = JumpTimeRange.y;
        shotTimer = ShotFireTimeRange.y;
    }
    public void StateMachine()
    {
        switch (aiState)
        {
            case AIState.idle:
                hasSetAggro = false;
                break;
            case AIState.boundedPatrol:
                speed = 6f;
                hasSetAggro = false;
                Patrol();
                break;
            case AIState.suspicious:
                speed = 4f;
                hasSetAggro = false;
                StartCoroutine("Suspicious");
                break;
            case AIState.aggro:
                speed = aggroSpeed;
                setAggro();
                hasSetAggro = true;
                Aggro();
                break;
            case AIState.dead:
                DeathProcedure();
                break;
        }
    }

    void Patrol()
    {
        if(moveDir == "right")
        {
            if (transform.position.x <= rightLimit)
            {
                velocity = new Vector2(speed, rb2d.velocity.y);
            }
            else
            {
                StartCoroutine("PatrolBreak");
            }
        }
        if (moveDir == "left")
        {
            if (transform.position.x >= leftLimit)
            {
                velocity = new Vector2(-speed, rb2d.velocity.y);
            }
            else
            {
                StartCoroutine("PatrolBreak");
            }
        }

    }

    void Aggro()
    {
        //check for wall
        RaycastHit2D WallCheckLowLeft = Physics2D.Linecast(WCLL.transform.position, WCLL.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D WallCheckHighLeft = Physics2D.Linecast(WCHL.transform.position, WCHL.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D WallCheckLowRight = Physics2D.Linecast(WCLR.transform.position, WCLR.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D WallCheckHighRight = Physics2D.Linecast(WCHR.transform.position, WCHR.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        if (WallCheckHighLeft.collider != null || WallCheckLowLeft.collider != null)
        {
            velocity.x = speed;
        }
        else if (WallCheckHighRight.collider != null || WallCheckLowRight.collider != null)
        {
            velocity.x = -speed;
        }
        shotTimer -= Time.deltaTime;
        if (shotTimer <= 0 && -1.5f < (transform.position.y-GameObject.Find("Player").transform.position.y) && (transform.position.y - GameObject.Find("Player").transform.position.y) < 1.5f )
        {
            shotTimer = Random.Range(ShotFireTimeRange.x, ShotFireTimeRange.y);
            for (int i = 0; i < shotCount; i++)
            {
                GameObject NewProj;
                if (moveDir == "right")
                {
                    NewProj = Instantiate(Projectile, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), Quaternion.identity);
                }
                else
                {
                    NewProj = Instantiate(Projectile, new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), Quaternion.identity);
                }
                if (velocity.x > 0)
                {
                    NewProj.GetComponent<Rigidbody2D>().velocity = new Vector2(ProjectileSpeed, Random.Range(-0.5f,2f));
                }
                else
                {
                    NewProj.GetComponent<Rigidbody2D>().velocity = new Vector2(-ProjectileSpeed, Random.Range(-0.5f, 2f));
                }
            } 
        }       
        jumpTimer -= Time.deltaTime;
        if (jumpTimer <= 0)
        {
            queueJump = true;
        }

        if (grounded == true && queueJump == true)
        {
            queueJump = false;
            velocity.y = Random.Range(JumpHeightRange.x, JumpHeightRange.y);
            jumpTimer = Random.Range(JumpTimeRange.x, JumpTimeRange.y);
        }

        if (grounded == false)
        {
            setyVel0 = true;
        }
    }

    private IEnumerator PatrolBreak()
    {
        aiState = AIState.idle;
        velocity = new Vector2(0, rb2d.velocity.y);
        yield return new WaitForSeconds (Random.Range(breakTime-breakRng, breakTime+breakRng));
        if (aiState == AIState.idle)
        {
            aiState = AIState.boundedPatrol;
            if (moveDir == "right")
            {
                moveDir = "left";
            }
            else
            {
                moveDir = "right";
            }
        }
    }

    private IEnumerator Suspicious()
    {
        
        RaycastHit2D GroundCheckLeft = Physics2D.Linecast(leftGc.transform.position, leftGc.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D GroundCheckRight = Physics2D.Linecast(rightGc.transform.position, rightGc.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D CeilingCheckRight = Physics2D.Linecast(CCR.transform.position, CCR.transform.position - new Vector3(0, 0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D WallCheckLowLeft = Physics2D.Linecast(WCLL.transform.position, WCLL.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D WallCheckHighLeft = Physics2D.Linecast(WCHL.transform.position, WCHL.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D WallCheckLowRight = Physics2D.Linecast(WCLR.transform.position, WCLR.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D WallCheckHighRight = Physics2D.Linecast(WCHR.transform.position, WCHR.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        if (GroundCheckLeft.collider == null && GroundCheckRight.collider != null || (WallCheckHighLeft.collider != null || WallCheckLowLeft.collider != null))
        {
            //velocity.x = Mathf.Abs(velocity.x);
            moveDir = "right";
        }
        else if (GroundCheckLeft.collider != null && GroundCheckRight.collider == null || (WallCheckHighRight.collider != null || WallCheckLowRight.collider != null))
        {
            //velocity.x = -Mathf.Abs(velocity.x);
            moveDir = "left";
        }
        if (moveDir == "right")
        {
            if (isPaused == "true")
            {
                velocity.x = 0f;
            }
            else
            {
                velocity.x = speed;
            }
            
        }
        else
        {
            if (isPaused == "true")
            {
                velocity.x = 0f;
            }
            else
            {
                velocity.x = -speed;
            }
        }
        if (isPaused == "false")
        {
            yield return new WaitForSeconds(4f);
            if (Random.Range(0, 100) > 50f && isPaused == "false")
            {
                isPaused = "true";
                yield return new WaitForSeconds(Random.Range(2, 5));
                isPaused = "false";
            }
        }
        else if (isPaused != "true")
        {
            isPaused = "true";
            yield return new WaitForSeconds(Random.Range(2, 5));
            isPaused = "false";
        }
        
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine();
        
            
        fovScript.SetOrigin(transform.position);
        //check for ground
        RaycastHit2D GroundCheckLeft = Physics2D.Linecast(leftGc.transform.position, leftGc.transform.position - new Vector3(0, -0.1f,0),1<<LayerMask.NameToLayer("Ground"));
        RaycastHit2D GroundCheckRight = Physics2D.Linecast(rightGc.transform.position, rightGc.transform.position - new Vector3(0, -0.1f,0),1<<LayerMask.NameToLayer("Ground"));
        if (GroundCheckLeft.collider != null || GroundCheckRight.collider != null)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        

        if (aiState != AIState.dead)
        {
            if (angerBar.transform.localScale.x <= 1)
            {
                angerBar.transform.localScale = new Vector3(currentSuspicion / 100, 1, 1);
                angerBar.transform.localPosition = new Vector2(-0.5f + (currentSuspicion / 200), 0);
                if (angerBar.transform.localScale.x >= 1)
                {
                    angerBar.transform.localScale = new Vector3(1, 1, 0);
                    angerBar.transform.localPosition = new Vector3(0,0, 0);
                }
            }
        }
        

    }

    private void FixedUpdate()
    {

        if (fovScript.canSeePlayer == true)
        {
            currentSuspicion += 0.6f*fovScript.suspicionMultiplier;
        }
        if (currentSuspicion > suspicionTriggerLevel)
        {
            setAggro();
        }
        if (velocity.x > 0)
        {
            moveDir = "right";
        }
        else if (velocity.x < 0)
        {
            moveDir = "left";
        }
        if (grounded == false)
        {
            velocity.y += gravity;
        }
        else
        {
            if (setyVel0 == true)
            {
                velocity.y = 0;
                setyVel0 = false;
            }
            
        }
        rb2d.velocity = new Vector2(velocity.x, velocity.y);

        //makes enemy look direction they are moving
        if (moveDir == "right")
        {
            fovScript.SetAimDirection(moveDir);
        }
        else
        {
            fovScript.SetAimDirection(moveDir);
        }
        //makes them hit their head on the ceiling
        RaycastHit2D CeilingCheckRight = Physics2D.Linecast(CCR.transform.position, CCR.transform.position - new Vector3(0, 0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D CeilingCheckLeft = Physics2D.Linecast(CCL.transform.position, CCL.transform.position - new Vector3(0, 0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        if (CeilingCheckLeft.collider != null || CeilingCheckRight.collider != null)
        {
            velocity = new Vector2(velocity.x, Mathf.Abs(velocity.y) * -0.4f);
        }
    }

    public void setAggro()
    {
        if (aiState != AIState.dead)
        {
            
            if (transform.parent.GetComponent<DistrictAIManagerScript>().HasCalled == false)
            {
                StartCoroutine("CallFriends");
            }
            else if (hasSetAggro == false)
            {

                aiState = AIState.aggro;
                if (GameObject.Find("Player").GetComponent<Transform>().position.x > transform.position.x)
                {
                    velocity.x = aggroSpeed;
                }
                else
                {
                    velocity.x = -aggroSpeed;
                }
                hasSetAggro = true;
            }
        }
        
        
    }

    public IEnumerator CallFriends() // talks to the parent to anger all AI in the area
    {
        anim.Play("Calling");
        yield return new WaitForSeconds(3.35f);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        if (aiState != AIState.dead)
        {
            transform.parent.GetComponent<DistrictAIManagerScript>().CallAll();
        }
        
    }

    private void DeathProcedure()
    {
        
        //check for wall
        RaycastHit2D WallCheckLowLeft = Physics2D.Linecast(WCLL.transform.position, WCLL.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D WallCheckHighLeft = Physics2D.Linecast(WCHL.transform.position, WCHL.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D WallCheckLowRight = Physics2D.Linecast(WCLR.transform.position, WCLR.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D WallCheckHighRight = Physics2D.Linecast(WCHR.transform.position, WCHR.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D CeilingCheckRight = Physics2D.Linecast(CCR.transform.position, CCR.transform.position - new Vector3(0, 0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D CeilingCheckLeft = Physics2D.Linecast(CCL.transform.position, CCL.transform.position - new Vector3(0, 0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        if (grounded == true)
        { 
            if(velocity.y > -2f && velHasDiminished == true)
            {
                velocity.y = 0f;
            }
            velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime*2f);
            velocity.y = Mathf.Abs(velocity.y);
            velHasDiminished = false;
        }
        else if (velHasDiminished == false)
        {
            velHasDiminished = true;
            velocity.x *= 0.5f;
            velocity.y *= 0.5f;
        }
        if (WallCheckHighLeft.collider != null || WallCheckLowLeft.collider != null)
        {
            velocity.x = Mathf.Abs(velocity.x);
        }
        else if (WallCheckHighRight.collider != null || WallCheckLowRight.collider != null)
        {
            velocity.x = -Mathf.Abs(velocity.x);
        }
        else if (CeilingCheckLeft.collider != null || CeilingCheckRight.collider != null)
        {
            velocity.y = -Mathf.Abs(velocity.y);
        }
        velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime*0.2f);
        fovScript.gameObject.GetComponent<MeshRenderer>().enabled = false;
        angerBar.transform.parent.gameObject.SetActive(false);
        gameObject.layer = 10;
    }

}
