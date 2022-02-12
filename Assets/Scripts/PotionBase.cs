using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionBase : MonoBehaviour
{

    public Vector2 velocity;
    public float gravity;
    private Rigidbody2D rb2d;
    public GameObject HitFX;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb2d.velocity = velocity;
        velocity.y -= gravity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<CircleCollider2D>().enabled = false;
        Instantiate(HitFX, transform.position, Quaternion.identity);
        gravity = 0;
        velocity = new Vector2(0, 0);
        GetComponent<SpriteRenderer>().enabled = false;
        
        GetComponent<DeleteAfterTime>().triggered = true;
    }
}
