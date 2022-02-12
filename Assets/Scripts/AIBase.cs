
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBase : MonoBehaviour
{
    // its satuday, garfelf's second favorit da

    [Header("Game Objects")]
    public GameObject leftGc;
    public GameObject rightGc;

    [Header("PatrolSettings")]
    public float leftLimit;
    public float rightLimit;
    public float breakTime; //time a guard will wait when they reach a point
    public float breakRng; //randomness allowed in breaktime, in seconds
    private string moveDir = "right";

    //[Header("Misc")]

    public enum AIState { idle, patrol };
    public AIState aiState;
    Vector2 velocity;
    public float gravity;
    public float speed;
    private Rigidbody2D rb2d;
    bool grounded;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
    public void StateMachine()
    {
        switch (aiState)
        {
            case AIState.idle:
                break;
            case AIState.patrol:
                Patrol();
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
            velocity.y = 0;
        }
        rb2d.velocity = new Vector2(velocity.x, velocity.y);
    }
}
