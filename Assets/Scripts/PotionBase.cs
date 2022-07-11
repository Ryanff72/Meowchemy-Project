using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionBase : MonoBehaviour
{

    public Vector2 velocity;
    private Vector3 HitPos;
    public float gravity;
    private Rigidbody2D rb2d;
    public GameObject HitFX;
    public float soundRadius;
    public float splashRadius;
    public List<float> distanceToEnemies = new List<float>();
    public List<float> distanceToObjects = new List<float>();
    public float distanceToPlayer;
    public GameObject[] enemies;
    public GameObject[] objects;
    private GameObject player;
    private GameObject HitObject;
    [SerializeField] GameObject SoundCreator;
    [SerializeField] AudioClip PotionBreakSound;
    private bool wasPlayerHit = false;
    public bool hasCollisionDirectionCheck;
    public List<GameObject> enemiesWithinSplash = new List<GameObject>();
    public List<GameObject> objectsWithinSplash = new List<GameObject>();
    [SerializeField] private string potionName;
    public string hitDir;
    int sendToFunctionCount;
    //private class 

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        rb2d = GetComponent<Rigidbody2D>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        objects = GameObject.FindGameObjectsWithTag("Object");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb2d.bodyType != RigidbodyType2D.Static)
        {
            rb2d.velocity = velocity;
            velocity.y -= gravity;
        }
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
        if(hasCollisionDirectionCheck == true)
        {
            RaycastHit2D RightCheck = Physics2D.Linecast(transform.position, transform.position + new Vector3(1.5f,0,0), 1 << LayerMask.NameToLayer("Ground"));
            RaycastHit2D LeftCheck = Physics2D.Linecast(transform.position, transform.position + new Vector3(-1.5f, 0, 0), 1 << LayerMask.NameToLayer("Ground"));
            RaycastHit2D TopCheck = Physics2D.Linecast(transform.position, transform.position + new Vector3(0, 1.5f, 0), 1 << LayerMask.NameToLayer("Ground"));
            RaycastHit2D BottomCheck = Physics2D.Linecast(transform.position, transform.position + new Vector3(0, -1.5f, 0), 1 << LayerMask.NameToLayer("Ground"));
            if (TopCheck.collider != null)
            {
                hitDir = "top";
            }
            if (BottomCheck.collider != null)
            {
                hitDir = "bottom";
            }
            if (RightCheck.collider != null)
            {
                hitDir = "right";
            }
            if (LeftCheck.collider != null)
            {
                hitDir = "left";
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject SC = Instantiate(SoundCreator, transform);
        SC.transform.position = transform.position;
        SC.GetComponent<AudioProximity>().PlaySound(PotionBreakSound, 70f, 0.6f);
        transform.GetChild(0).GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        HitObject = collision.gameObject;
        HitPos = transform.position;
        rb2d.bodyType = RigidbodyType2D.Static;
        velocity = new Vector2(0, 0);
        Instantiate(HitFX, transform.position, Quaternion.identity);
        gravity = 0;
        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        for (int i = 0; i < enemies.Length; i++)
        {

            if (Vector3.Distance(transform.position, enemies[i].transform.position) < splashRadius)
            {
                RaycastHit2D GroundCheckBetweenObjects = Physics2D.Linecast(transform.position, enemies[i].transform.position, 1 << LayerMask.NameToLayer("Ground"));
                if (GroundCheckBetweenObjects.collider == null)
                {
                    enemiesWithinSplash.Add(enemies[i] as GameObject);
                    distanceToEnemies.Add(Vector3.Distance(transform.position, enemies[i].transform.position));

                }
            }
            if (i + 1 == enemies.Length)
            {
                distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                RaycastHit2D GroundCheckBetweenObjects = Physics2D.Linecast(transform.position, player.transform.position, 1 << LayerMask.NameToLayer("Ground"));
                if (distanceToPlayer < splashRadius && GroundCheckBetweenObjects.collider == null)
                {
                    wasPlayerHit = true;
                }
            }


        }
        objects = GameObject.FindGameObjectsWithTag("Object");
        for (int i = 0; i < objects.Length; i++)
        {

            if (Vector3.Distance(transform.position, objects[i].transform.position) < splashRadius)
            {
                RaycastHit2D GroundCheckBetweenObjects = Physics2D.Linecast(transform.position, objects[i].transform.position, 1 << LayerMask.NameToLayer("Ground"));
                if (GroundCheckBetweenObjects.collider == null)
                {
                    objectsWithinSplash.Add(objects[i] as GameObject);
                    distanceToObjects.Add(Vector3.Distance(transform.position, objects[i].transform.position));
                }
            }
        }
        player = GameObject.Find("Player");
        if (sendToFunctionCount == 0)
        {
            sendToFunctionCount++;
            GetComponent<PotionFunctionScript>().potionCollide(potionName, enemiesWithinSplash, objectsWithinSplash, soundRadius, distanceToEnemies, distanceToObjects, HitPos, distanceToPlayer, player, wasPlayerHit, HitObject);
        }
        transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().Stop();
        GetComponent<DeleteAfterTime>().triggered = true;
        
    }
}
