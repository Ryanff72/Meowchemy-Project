using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
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
        if (collision.gameObject.layer == 3)
        {
            collision.gameObject.GetComponent<PlayerController>().StartCoroutine("Respawn");
        }
        else if (collision.gameObject.layer == 7 && collision.gameObject.GetComponent<AIBase>().killedByOtherAI == false)
        {
            collision.gameObject.GetComponent<AIBase>().killedByOtherAI = true;
            collision.gameObject.GetComponent<AIBase>().aiState = AIBase.AIState.dead;
            if (transform.position.x> collision.transform.position.x)
            {
                collision.gameObject.GetComponent<AIBase>().velocity.x = -25f;
            }
            else
            {
                collision.gameObject.GetComponent<AIBase>().velocity.x = 25f;
            }
            
        }
        Destroy(gameObject);
    }
}
