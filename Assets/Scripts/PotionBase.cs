using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionBase : MonoBehaviour
{

    public Vector2 velocity;
    public float gravity;
    private Rigidbody2D rb2d;
    public GameObject HitFX;
    public float soundRadius;
    public float splashRadius;
    public List<float> distanceToEnemies = new List<float>();
    public float distanceToPlayer;
    public GameObject[] enemies;
    private GameObject player;
    private bool wasPlayerHit = false;
    public List<GameObject> enemiesWithinSplash = new List<GameObject>();
    [SerializeField] private string potionName;
    //private class 

    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        player = GameObject.Find("Player");
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb2d.velocity = velocity;
        velocity.y -= gravity;
        if (velocity.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, Mathf.Abs(transform.localScale.y) * -1, transform.localScale.z);
        }
    }

    private void Update()
    {
        float angle;
        Vector2 v = GetComponent<Rigidbody2D>().velocity;
        angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * 25);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponent<CircleCollider2D>().enabled = false;
        
        Instantiate(HitFX, transform.position, Quaternion.identity);
        gravity = 0;
        velocity = new Vector2(0, 0);
        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        for (int i = 0; i < enemies.Length; i++)
        {
            
            if (Vector3.Distance(transform.position, enemies[i].transform.position) < soundRadius)
            {
                //Debug.Log(i);
                
                enemies[i].GetComponent<AIBase>().currentSuspicion += 10;
                if (enemies[i].GetComponent<AIBase>().aiState != AIBase.AIState.dead && enemies[i].GetComponent<AIBase>().aiState != AIBase.AIState.aggro)
                {
                    enemies[i].GetComponent<AIBase>().aiState = AIBase.AIState.suspicious;
                }
            }
            if (Vector3.Distance(transform.position, enemies[i].transform.position) < splashRadius)
            {
                enemiesWithinSplash.Add(enemies[i] as GameObject);
                distanceToEnemies.Add(Vector3.Distance(transform.position, enemies[i].transform.position));
            }
            if (i+1 == enemies.Length)
            {
                
                distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                
                if (distanceToPlayer < splashRadius)
                {
                    wasPlayerHit = true;
                }
            }
            

        }
        GetComponent<PotionFunctionScript>().potionCollide(potionName, enemiesWithinSplash, distanceToEnemies, transform.position, distanceToPlayer, player, wasPlayerHit);
        transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().Stop();
        GetComponent<DeleteAfterTime>().triggered = true;
    }
}
