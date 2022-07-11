using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorScript : MonoBehaviour
{
    public LineRenderer LR;
    public LineRenderer LR1; // line renderer attatched to the bottom
    public float ropeLength;
    public float upSpeed;
    [SerializeField] Vector3 highest;
    [SerializeField] Vector3 lowest;
    private SimpleBoxObjectPhysics sbop;
    string direction;
    public bool ropeCut;
    bool moveUp = false;
    [SerializeField] bool cuttableRope;
    public bool activated;
    public Rigidbody2D rb2d;
    // Start is called before the first frame update
    void Start()
    {
        sbop = transform.GetChild(1).GetComponent<SimpleBoxObjectPhysics>();
        ropeLength = -transform.GetChild(1).transform.position.y;
    }

    private void FixedUpdate()
    {
        if (ropeCut == false)
        {
            LR.SetPosition(0, transform.GetChild(1).transform.position);
            LR.SetPosition(1, transform.GetChild(0).transform.position);
            LR1.SetPosition(1, transform.GetChild(1).transform.position);
        }

        if (cuttableRope == true)
        {
            LR1.SetPosition(0, transform.GetChild(1).transform.position);
            RaycastHit2D BreakCheckEnemyProjectile0 = Physics2D.Linecast(transform.position, transform.GetChild(1).transform.position, 1 << LayerMask.NameToLayer("EnemyProjectile"));
            RaycastHit2D BreakCheckEnemyProjectile1 = Physics2D.Linecast(transform.position + new Vector3(0.5f, 0, 0), transform.GetChild(1).transform.position + new Vector3(0.5f, 0, 0), 1 << LayerMask.NameToLayer("EnemyProjectile"));
            RaycastHit2D BreakCheckEnemyProjectile2 = Physics2D.Linecast(transform.position + new Vector3(-0.5f, 0, 0), transform.GetChild(1).transform.position + new Vector3(-0.5f, 0, 0), 1 << LayerMask.NameToLayer("EnemyProjectile"));
            RaycastHit2D BreakCheckPotion0 = Physics2D.Linecast(transform.position, transform.GetChild(1).transform.position, 1 << LayerMask.NameToLayer("Potion"));
            RaycastHit2D BreakCheckPotion1 = Physics2D.Linecast(transform.position + new Vector3(0.4f, 0, 0), transform.GetChild(1).transform.position + new Vector3(0.4f, 0, 0), 1 << LayerMask.NameToLayer("Potion"));
            RaycastHit2D BreakCheckPotion2 = Physics2D.Linecast(transform.position + new Vector3(-0.4f, 0, 0), transform.GetChild(1).transform.position + new Vector3(-0.4f, 0, 0), 1 << LayerMask.NameToLayer("Potion"));
            if (BreakCheckEnemyProjectile0.collider != null && ropeCut == false)
            {
                ropeCut = true;
                sbop.gravity = -40;
                LR.SetPosition(0, new Vector3(transform.position.x, BreakCheckEnemyProjectile0.point.y, 0));
                LR1.SetPosition(1, new Vector3(transform.position.x, BreakCheckEnemyProjectile0.point.y, 0));
            }
            else if (BreakCheckEnemyProjectile1.collider != null && ropeCut == false)
            {
                ropeCut = true;
                sbop.gravity = -40;
                LR.SetPosition(0, new Vector3(transform.position.x, BreakCheckEnemyProjectile1.point.y, 0));
                LR1.SetPosition(1, new Vector3(transform.position.x, BreakCheckEnemyProjectile1.point.y, 0));

            }
            else if (BreakCheckEnemyProjectile2.collider != null && ropeCut == false)
            {
                ropeCut = true;
                sbop.gravity = -40;
                LR.SetPosition(0, new Vector3(transform.position.x, BreakCheckEnemyProjectile2.point.y, 0));
                LR1.SetPosition(1, new Vector3(transform.position.x, BreakCheckEnemyProjectile2.point.y, 0));
            }
            if (BreakCheckPotion0.collider != null && cuttableRope == true && ropeCut == false)
            {
                ropeCut = true;
                sbop.gravity = -40;
                LR.SetPosition(0, new Vector3(transform.position.x, BreakCheckPotion0.point.y, 0));
                LR1.SetPosition(1, new Vector3(transform.position.x, BreakCheckPotion0.point.y, 0));
            }
            else if (BreakCheckPotion1.collider != null && cuttableRope == true && ropeCut == false)
            {
                ropeCut = true;
                sbop.gravity = -40;
                LR.SetPosition(0, new Vector3(transform.position.x, BreakCheckPotion1.point.y, 0));
                LR1.SetPosition(1, new Vector3(transform.position.x, BreakCheckPotion1.point.y, 0));
            }
            else if (BreakCheckPotion2.collider != null && cuttableRope == true && ropeCut == false)
            {
                ropeCut = true;
                sbop.gravity = -40;
                LR.SetPosition(0, new Vector3(transform.position.x, BreakCheckPotion2.point.y, 0));
                LR1.SetPosition(1, new Vector3(transform.position.x, BreakCheckPotion2.point.y, 0));
            }
        }
        if (activated == true)
        {
            moveUp = true;
        }
        else
        {
            moveUp = false;
        }
        if (ropeCut == false)
        {
            if (moveUp)
            {
                if (transform.GetChild(1).localPosition.y < highest.y)
                {
                    sbop.velocity.y = 8;
                }
                else
                {
                    sbop.velocity.y = 0;
                }

            }
            else if (moveUp == false)
            {
                if (transform.GetChild(1).localPosition.y > lowest.y)
                {
                    sbop.velocity.y = -8;
                }
                else
                {
                    sbop.velocity.y = 0;
                }
            }
        }
        else
        {
            LR1.SetPosition(1, Vector3.Lerp(LR1.GetPosition(1), transform.GetChild(1).transform.position, Time.deltaTime * 15));
        }
        
    }


}
