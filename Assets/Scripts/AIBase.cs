
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
    private string moveDir = "right";

    //[Header("Misc")]

    public enum AIState { idle, patrol, aggro };
    public AIState aiState;
    Vector2 velocity;
    public float gravity;
    public float speed;
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
    private bool queueJump = false;

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
                speed = 28f;
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
                    NewProj.GetComponent<Rigidbody2D>().velocity = new Vector2(ProjectileSpeed, Random.Range(-8,8));
                }
                else
                {
                    NewProj.GetComponent<Rigidbody2D>().velocity = new Vector2(-ProjectileSpeed, Random.Range(-8, 8));
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
            Debug.Log("hello");
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
        aiState = AIState.patrol;
        if(moveDir == "right")
        {
            moveDir = "left";
        }
        else
        {
            moveDir = "right";
        }
    }
    // Update is called once per frame
    void Update()
    {
        StateMachine();

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

      


    }

    private void FixedUpdate()
    {
        if (grounded == false)
        {
            velocity.y += gravity;
        }
        else
        {
            //velocity.y = 0;
        }
        rb2d.velocity = new Vector2(velocity.x, velocity.y);
    }

    public void setAggro()
    {
        if (hasSetAggro == false)
        {
            if (GameObject.Find("Player").GetComponent<Transform>().position.x > transform.position.x)
            {
                velocity.x = speed;
            }
            else
            {
                velocity.x = -speed;
            }
            hasSetAggro = true;
        }
        
    }
}
