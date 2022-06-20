using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickupScript : MonoBehaviour
{
    private GameObject player;
    private bool pickupable;
    public int ammo;
    public Vector2 velocity;
    private Rigidbody2D rb2d;
    bool grounded;
    bool nearGrounded;
    bool hasSpawnedSmoke = false;
    [SerializeField] float gravity;
    [SerializeField] Transform leftGc;
    [SerializeField] Transform rightGc;
    [SerializeField] Transform WCLR;
    [SerializeField] Transform WCHL;
    [SerializeField] Transform WCLL;
    [SerializeField] Transform WCHR;
    [SerializeField] Transform CCR;
    [SerializeField] Transform CCL;
    bool velHasDiminished = false;
    [SerializeField] GameObject landingSmoke;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        rb2d = transform.parent.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D GroundCheck = Physics2D.Linecast(leftGc.position, rightGc.position, 1 << LayerMask.NameToLayer("Ground"));
        if (GroundCheck.collider != null)
        {
            grounded = true;
            if (hasSpawnedSmoke == false)
            {
                hasSpawnedSmoke = true;
                Instantiate(landingSmoke, transform.position + new Vector3(0, -0.72f, 0), Quaternion.Euler(-90, 0, 0));
            }
        }
        else
        {
            hasSpawnedSmoke = false;
            grounded = false;
        }
        RaycastHit2D NearGroundCheck = Physics2D.Linecast(leftGc.transform.position + new Vector3(0, -0.1f, 0), rightGc.transform.position + new Vector3(0, -0.1f, 0), 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D WallCheckRight = Physics2D.Linecast(WCLR.position, WCHR.position, 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D WallCheckLeft = Physics2D.Linecast(WCLL.position, WCHL.position, 1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D CeilingCheck = Physics2D.Linecast(CCR.position, CCL.position, 1 << LayerMask.NameToLayer("Ground"));
        if (NearGroundCheck.collider != null)
        {
            nearGrounded = true;
            velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * 3f);
        }
        else
        {
            nearGrounded = false;
        }
        if (grounded == true)
        {
            if (velocity.y < -15f)
            {
                velocity.y = Mathf.Abs(velocity.y);
                velHasDiminished = false;
            }
            else
            {
                velocity.y = Mathf.Lerp(velocity.y, -1, Time.deltaTime * 10);
                velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * 2f);
            }
        }
        else if (velHasDiminished == false)
        {
            velHasDiminished = true;
            velocity.x *= 0.5f;
            velocity.y *= 0.6f;
        }
        if (WallCheckLeft.collider != null)
        {
            velocity.x = Mathf.Abs(velocity.x);
        }
        if (WallCheckRight.collider != null)
        {
            velocity.x = -Mathf.Abs(velocity.x);
        }
        if (CeilingCheck.collider != null)
        {
            velocity.y = -Mathf.Abs(velocity.y);
        }
        if (nearGrounded == false)//workaround for gravity being wonky
        {
            velocity.y += gravity * Time.deltaTime;
        }
        rb2d.velocity = velocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            player.GetComponent<PlayerController>().canPickUpWeapon = true;
            player.GetComponent<PlayerController>().pickupAbleWeapon = gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        player.GetComponent<PlayerController>().canPickUpWeapon = false;
    }
}
