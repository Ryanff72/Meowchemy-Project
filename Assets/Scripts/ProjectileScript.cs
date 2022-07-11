using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public int shotCount;
    public float ProjectileSpeed;
    public bool playerBullet = false;
    [SerializeField] GameObject BulletEffect;
    float changeDirMaxTime = 0.1f;
    float changeDirCurrentTime;
    Rigidbody2D rb2d;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        changeDirCurrentTime = 0;
    }
    private void Update()
    {
        changeDirCurrentTime -= Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Instantiate(BulletEffect, transform.position, Quaternion.Euler(-90, 0, 0));
        GetComponent<CircleCollider2D>().enabled = false;
        if (collision.gameObject.layer == 13 || collision.gameObject.tag == "Lever")
        {
            collision.gameObject.GetComponent<LeverScript>().Activate();
        }
        if (collision.gameObject.layer == 3)
        {
            collision.gameObject.GetComponent<PlayerController>().ps = PlayerController.PlayerState.dead;
            collision.gameObject.GetComponent<PlayerController>().velocity = new Vector2(rb2d.velocity.x, 15);
        }
        else if (collision.gameObject.layer == 7 && collision.gameObject.GetComponent<AIBase>().killedByOtherAI == false)
        {
            if (playerBullet == false)
            {
                collision.gameObject.GetComponent<AIBase>().killedByOtherAI = true;
            }
            collision.gameObject.GetComponent<AIBase>().aiState = AIBase.AIState.dead;
            if (transform.position.x > collision.transform.position.x)
            {
                collision.gameObject.GetComponent<AIBase>().velocity = new Vector2(rb2d.velocity.x, 15);
            }
            else
            {
                collision.gameObject.GetComponent<AIBase>().velocity = new Vector2(rb2d.velocity.x, 15);
            }
            
        }
        else if (collision.gameObject.layer == 12)
        {
            collision.gameObject.GetComponent<SimpleBoxObjectPhysics>().velocity = new Vector2(rb2d.velocity.x, 12);
        }
        else if (collision.gameObject.layer == 10)
        {
            collision.gameObject.GetComponent<AIBase>().velocity = new Vector2(rb2d.velocity.x, 12);
        }

        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        rb2d.bodyType = RigidbodyType2D.Static;
        GetComponent<DeleteAfterTime>().triggered = true;
    }

    public void ChangeDir()
    {
        if (changeDirCurrentTime < 0)
        {
            rb2d.velocity = new Vector2(-rb2d.velocity.x, rb2d.velocity.y);
            changeDirCurrentTime = changeDirMaxTime;
        }
    }
}
