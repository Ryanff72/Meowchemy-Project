using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PotionManager : MonoBehaviour
{

    public int equippedPotionIndex = 0;
    public GameObject leftHand;
    public float maxThrowForce;
    private float throwForce;
    public float gravity;
    public int[] numberPotionsRemaining;
    public GameObject[] Potions;
    public GameObject hotBar;
    [SerializeField] private GameObject Projectile;
    [SerializeField] private GameObject gunTip;
    public List<GameObject> enemiesWithinSound = new List<GameObject>();
    Vector2 direction;
    float distanceToMouse;
    GameObject ThrownPotion;
    SoundManagerScript sms;

    public GameObject player;
[Header("Points")]
    public GameObject point;
    int lastPoint=15;

    GameObject[] points;
    public GameObject GunFX;
    public int numberOfPoints;
    public float spaceBetweenPoints;
    Vector3 heldPotionSize;

    // Start is called before the first frame update
    void Start()
    {
        sms = GameObject.Find("SoundManager").GetComponent<SoundManagerScript>();
        heldPotionSize = transform.GetChild(0).transform.localScale;
        points = new GameObject[numberOfPoints];
        for (int i = 0; i< numberOfPoints; i++)
        {
            points[i] = Instantiate(point, new Vector3(transform.position.x, transform.position.y+2,0), Quaternion.identity);
            points[i].transform.localScale = new Vector3(0.2f-(i*0.005f), 0.2f - (i * 0.005f), 0);
            //points[i].GetComponent<SpriteRenderer>().enabled = false;
        }
        //updats # of potions
        for (int i = 0; i < Potions.Length; i++)
        {
            hotBar.transform.GetChild(i).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text=numberPotionsRemaining[i].ToString();
            hotBar.transform.GetChild(i).transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Potions[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;

        }
        for (int o = 0; o < hotBar.transform.childCount; o++)
        {
            if (o > Potions.Length-1)
            {
                hotBar.transform.GetChild(o).gameObject.SetActive(false);
            }
        }
        //decides if potions should be rendered in the player's hand
        if (numberPotionsRemaining[equippedPotionIndex] > 0)
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Potions[equippedPotionIndex].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        }

    }
    public void GotSomePotions()
    {
        for (int i = 0; i < Potions.Length; i++)
        {
            hotBar.transform.GetChild(i).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = numberPotionsRemaining[i].ToString();
            hotBar.transform.GetChild(i).transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Potions[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;

        }
        for (int o = 0; o < hotBar.transform.childCount; o++)
        {
            if (o > Potions.Length - 1)
            {
                hotBar.transform.GetChild(o).gameObject.SetActive(false);
            }
        }
        //decides if potions should be rendered in the player's hand
        if (numberPotionsRemaining[equippedPotionIndex] > 0)
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Potions[equippedPotionIndex].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        //aiming stuff
        if (player.GetComponent<PlayerController>().hasWeapon == false)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = mousePosition - new Vector2(transform.parent.position.x, transform.parent.position.y);
            distanceToMouse = Vector2.Distance(mousePosition, player.transform.position);
            transform.right = direction;
            //decides where the players hand is
            transform.position = leftHand.transform.position + new Vector3(0, 0.3f, 0);
            //makes the player be able to scroll through potions
            if (Input.GetAxis("Mouse ScrollWheel") < 0f) // forwards
            {
                if (equippedPotionIndex == Potions.Length - 1)
                {
                    equippedPotionIndex = 0;
                }
                else
                {
                    equippedPotionIndex++;
                }
                for (int i = 0; i < Potions.Length; i++)
                {
                    hotBar.transform.GetChild(i).transform.GetChild(4).GetComponent<SpriteRenderer>().enabled = false;
                }
                if (numberPotionsRemaining[equippedPotionIndex] > 0)
                {
                    transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                }
                else
                {
                    transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                }
                transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Potions[equippedPotionIndex].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
                hotBar.transform.GetChild(equippedPotionIndex).transform.GetChild(4).GetComponent<SpriteRenderer>().enabled = true;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0f) // backwards
            {
                if (equippedPotionIndex == 0)
                {
                    equippedPotionIndex = Potions.Length - 1;
                }
                else
                {
                    equippedPotionIndex--;
                }
                for (int i = 0; i < Potions.Length; i++)
                {
                    hotBar.transform.GetChild(i).transform.GetChild(4).GetComponent<SpriteRenderer>().enabled = false;
                }
                if (numberPotionsRemaining[equippedPotionIndex] > 0)
                {
                    transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                }
                else
                {
                    transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                }
                transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Potions[equippedPotionIndex].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
                hotBar.transform.GetChild(equippedPotionIndex).transform.GetChild(4).GetComponent<SpriteRenderer>().enabled = true;
            }
            //does stuff with the sprite of the potion in the players hand
            if (transform.GetChild(0).rotation.eulerAngles.z > 270 || transform.GetChild(0).rotation.eulerAngles.z < 90)
            {
                transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipX = false;
                transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipY = false;
            }
            else
            {
                transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipX = true;
                transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipY = true;
            }
            transform.GetChild(0).transform.localScale = Vector3.Lerp(transform.GetChild(0).transform.localScale, heldPotionSize, Time.deltaTime * 5f);
            //throw force decider
            if (distanceToMouse > 10 || distanceToMouse == 0)
            {
                throwForce = maxThrowForce;
            }
            else
            {
                throwForce = maxThrowForce * (distanceToMouse / 10);
                //numberOfPoints = 25 - Mathf.RoundToInt(24-distanceToMouse*2.4f);
            }

            //handles if the line shows for aiming or not
            if (Input.GetButton("Aim"))
            {
                player.GetComponent<PlayerController>().canMoveLeft = false;
                player.GetComponent<PlayerController>().canMoveRight = false;
                for (int i = 0; i < numberOfPoints; i++)
                {
                    
                    if (i != numberOfPoints-1)
                    {
                        if (Physics2D.Linecast(points[i].transform.position, points[i + 1].transform.position, (1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Pickup") | 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("DeadEnemy") | 1 << LayerMask.NameToLayer("Lever"))))
                        {
                            //points[i].GetComponent<SpriteRenderer>().enabled = false;
                            for (int o = i; o < (numberOfPoints); o++)
                            {
                                points[o].GetComponent<SpriteRenderer>().enabled = false;
                            }
                            lastPoint = i;
                        }
                        else if (i == numberOfPoints - 2)
                        {
                            lastPoint = numberOfPoints;
                        }
                        else if (i <= lastPoint)
                        {
                            points[i].GetComponent<SpriteRenderer>().enabled = true;
                        }
                    }
                    if (i == numberOfPoints)
                    {
                        lastPoint = numberOfPoints;
                    }

                }
            }
            else
            {
                for (int i = 0; i < numberOfPoints; i++)
                {
                    points[i].GetComponent<SpriteRenderer>().enabled = false;
                }

            }

            if (Input.GetButtonDown("Throw")) //&& Input.GetButton("Aim"))
            {
                Throw();
            }

            //does the aiming stuff
            for (int i = 0; i < numberOfPoints; i++)
            {
                points[i].transform.position = PointPosition(i * spaceBetweenPoints);
            }

            for (int i = 0; i < numberOfPoints; i++)
            {
                if (i >= numberOfPoints)
                {
                    //points[i].GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }
        else if (Input.GetButtonDown("Throw")) // fires off the weapon
        {
            sms.MakeSound(10, transform.position);
            for (int i = 0; i < Projectile.GetComponent<ProjectileScript>().shotCount; i++)
            {
                GameObject NewProj;
                if (player.transform.GetChild(1).GetChild(0).localScale.x > 0)
                {
                    NewProj = Instantiate(Projectile, gunTip.transform.position, Quaternion.identity);
                    NewProj.GetComponent<Rigidbody2D>().velocity = new Vector2(Projectile.GetComponent<ProjectileScript>().ProjectileSpeed, Random.Range(-0.5f, 2f));
                    NewProj.GetComponent<ProjectileScript>().playerBullet = true;
                    player.GetComponent<PlayerController>().velocity = new Vector2(-80, 15);
                    Instantiate(GunFX, gunTip.transform.position, Quaternion.Euler(-90, 0, 0));
                }
                else
                {
                    NewProj = Instantiate(Projectile, gunTip.transform.position, Quaternion.identity);
                    NewProj.GetComponent<Rigidbody2D>().velocity = new Vector2(-Projectile.GetComponent<ProjectileScript>().ProjectileSpeed, Random.Range(-0.5f, 2f));
                    NewProj.GetComponent<ProjectileScript>().playerBullet = true;
                    player.GetComponent<PlayerController>().velocity = new Vector2(80
                        , 15);
                    Instantiate(GunFX, gunTip.transform.position, Quaternion.Euler(-90, 180, 0));
                }
            }
            player.GetComponent<PlayerController>().ammoCount--;

        }
        
    }
    void Throw()
    {
        if (numberPotionsRemaining[equippedPotionIndex] >= 1 && transform.parent.transform.parent.transform.parent.transform.parent.GetComponent<PlayerController>().ps != PlayerController.PlayerState.dead)
        {
            ThrownPotion = Instantiate(Potions[equippedPotionIndex], transform.position, Quaternion.identity);
            ThrownPotion.GetComponent<PotionBase>().velocity = transform.right * throwForce;
            transform.GetChild(0).transform.localScale = new Vector3(0, 0, 0);
            numberPotionsRemaining[equippedPotionIndex]--;
        }
        if (numberPotionsRemaining[equippedPotionIndex] == 0)
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        }
        for (int i = 0; i < Potions.Length; i++)
        {
            hotBar.transform.GetChild(i).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = numberPotionsRemaining[i].ToString();
        }
    }

    //for aiming
    Vector2 PointPosition(float t)
    {
        Vector2 position = (Vector2)transform.position + (direction.normalized * throwForce * t) + 0.5f * new Vector2(0, -gravity) * (t * t);
        return position;
    }
}
