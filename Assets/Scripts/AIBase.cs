
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

    [Header("PatrolSettings")]
    public float leftLimit;
    public float rightLimit;
    public float breakTime; //time a guard will wait when they reach a point
    public float breakRng; //randomness allowed in breaktime, in seconds
    [SerializeField] private string moveDir;
    public float canSee;
    //[SerializeField] private FieldOfView fieldOfView;

    //[Header("Misc")]

    public enum AIState { idle, patrol, aggro };
    public AIState aiState;
    Vector2 velocity;
    public float gravity;
    public float speed;
    public float aggroSpeed;
    private Rigidbody2D rb2d;
    bool grounded;
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
            case AIState.patrol:
                speed = 6f;
                hasSetAggro = false;
                Patrol();
                break;
            case AIState.aggro:
                speed = aggroSpeed;
                setAggro();
                hasSetAggro = true;
                Aggro();
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
        if (shotTimer <= 0)
        {
            shotTimer = Random.Range(ShotFireTimeRange.x, ShotFireTimeRange.y);
            for (int i = 0; i < shotCount; i++)
            {
                GameObject NewProj = Instantiate(Projectile, transform.position, Quaternion.identity);
                if (velocity.x > 0)
                {
                    NewProj.GetComponent<Rigidbody2D>().velocity = new Vector2(ProjectileSpeed, Random.Range(-0.5f,2.5f));
                }
                else
                {
                    NewProj.GetComponent<Rigidbody2D>().velocity = new Vector2(-ProjectileSpeed, Random.Range(-0.5f, 2.5f));
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
    }

    private IEnumerator PatrolBreak()
    {
        aiState = AIState.idle;
        velocity = new Vector2(0, rb2d.velocity.y);
        yield return new WaitForSeconds (Random.Range(breakTime-breakRng, breakTime+breakRng));
        if (aiState == AIState.idle)
        {
            aiState = AIState.patrol;
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
    // Update is called once per frame
    void Update()
    {
        StateMachine();
        fovScript.SetOrigin(transform.position);
        //check for ground
        RaycastHit2D GroundCheckLeft = Physics2D.Linecast(leftGc.transform.position, leftGc.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D GroundCheckRight = Physics2D.Linecast(rightGc.transform.position, rightGc.transform.position - new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        if (GroundCheckLeft.collider != null || GroundCheckRight.collider != null)
        {

            grounded = true;

        }
        else
        {
            grounded = false;
        }

        if (angerBar.transform.localScale.x <= 1)
        {
            angerBar.transform.localScale = new Vector3(currentSuspicion / 100, 1,1 );
            angerBar.transform.localPosition = new Vector2(-0.5f + (currentSuspicion / 200), 0);
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
            Debug.Log("angered");
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
            //velocity.y = 0;
        }
        rb2d.velocity = new Vector2(velocity.x, velocity.y);

        //makes enemy look direction they are moving
        if(moveDir == "right")
        {
            fovScript.SetAimDirection(moveDir);
        }
        else
        {
            fovScript.SetAimDirection(moveDir);
        }
    }

    public void setAggro()
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

    public IEnumerator CallFriends() // talks to the parent to anger all AI in the area
    {
        anim.Play("Calling");
        yield return new WaitForSeconds(3.35f);
        transform.parent.GetComponent<DistrictAIManagerScript>().CallAll();
    }
}
