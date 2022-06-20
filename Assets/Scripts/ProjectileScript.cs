using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public int shotCount;
    public float ProjectileSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponent<CircleCollider2D>().enabled = false;
        if (collision.gameObject.layer == 3)
        {
            collision.gameObject.GetComponent<PlayerController>().ps = PlayerController.PlayerState.dead;
            collision.gameObject.GetComponent<PlayerController>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 15);
        }
        else if (collision.gameObject.layer == 7 && collision.gameObject.GetComponent<AIBase>().killedByOtherAI == false)
        {
            collision.gameObject.GetComponent<AIBase>().killedByOtherAI = true;
            collision.gameObject.GetComponent<AIBase>().aiState = AIBase.AIState.dead;
            if (transform.position.x> collision.transform.position.x)
            {
                collision.gameObject.GetComponent<AIBase>().velocity.x = -35f;
            }
            else
            {
                collision.gameObject.GetComponent<AIBase>().velocity.x = 35f;
            }
            
        }
        Destroy(gameObject);
    }
}
