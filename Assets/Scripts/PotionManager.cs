using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionManager : MonoBehaviour
{

    public GameObject potion;
    public GameObject rightHand;
    public float maxThrowForce;
    private float throwForce;
    public float gravity;
    Vector2 direction;
    float distanceToMouse;
    GameObject ThrownPotion;

    public GameObject player;
[Header("Points")]
    public GameObject point;
    
    GameObject[] points;
    public int numberOfPoints;
    public float spaceBetweenPoints;

    // Start is called before the first frame update
    void Start()
    {
        points = new GameObject[numberOfPoints];
        for (int i  = 0; i< numberOfPoints; i++)
        {
            points[i] = Instantiate(point, transform.position, Quaternion.identity);
            points[i].GetComponent<SpriteRenderer>().enabled = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePosition - new Vector2 (transform.parent.position.x, transform.parent.position.y);
        distanceToMouse = Vector2.Distance(mousePosition, player.transform.position);
        transform.right = direction;
        //decides where the players hand is
        transform.position = rightHand.transform.position;
        //throw force decider
        if (distanceToMouse > 10 || distanceToMouse == 0)
        {
            throwForce = maxThrowForce;
            numberOfPoints = 25;
        }
        else
        {
            throwForce = maxThrowForce * (distanceToMouse/10);
            numberOfPoints = 25 - Mathf.RoundToInt(24-distanceToMouse*2.4f);
        }

        //handles if the line shows for aiming or not
        if (Input.GetButton("Aim"))
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                points[i].GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        if(Input.GetButtonUp("Aim"))
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

        for (int i = 0; i < 25; i++)
        {
            if (i >= numberOfPoints)
            {
                points[i].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    void Throw()
    {
        
        ThrownPotion = Instantiate(potion, transform.position, Quaternion.identity);
        ThrownPotion.GetComponent<PotionBase>().velocity = transform.right * throwForce;
    }

    //for aiming
    Vector2 PointPosition(float t)
    {
        Vector2 position = (Vector2)transform.position + (direction.normalized * throwForce * t) + 0.5f * new Vector2(0, -gravity) * (t * t);
        return position;
    }
}
