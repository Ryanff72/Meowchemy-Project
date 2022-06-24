using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public int shotCount;
    public float ProjectileSpeed;
    public bool playerBullet = false;
    [SerializeField] GameObject BulletEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Instantiate(BulletEffect, transform.position, Quaternion.Euler(-90, 0, 0));
        GetComponent<CircleCollider2D>().enabled = false;
        if (collision.gameObject.layer == 3)
        {
            collision.gameObject.GetComponent<PlayerController>().ps = PlayerController.PlayerState.dead;
            collision.gameObject.GetComponent<PlayerController>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 15);
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
                collision.gameObject.GetComponent<AIBase>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 15);
            }
            else
            {
                collision.gameObject.GetComponent<AIBase>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 15);
            }
            
        }
        else if (collision.gameObject.layer == 12)
        {
            collision.gameObject.GetComponent<SimpleBoxObjectPhysics>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 12);
        }
        else if (collision.gameObject.layer == 10)
        {
            collision.gameObject.GetComponent<AIBase>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 12);
        }
        
            Destroy(gameObject);
    }
}
