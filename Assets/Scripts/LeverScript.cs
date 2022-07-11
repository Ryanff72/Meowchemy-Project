using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    private GameObject player;
    bool canInteract;
    [SerializeField] bool activated;
    public Transform HandleTransform;
    public ElevatorScript EScr;
    float activationTimer = 0.2f;
    float currentActivationTimer;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Interact") && canInteract == true && player.GetComponent<PlayerController>().hasWeapon == false && player.GetComponent<PlayerController>().canPickUpWeapon == false)
        {
            Activate();
        }
        if (activated)
        {
            HandleTransform.localPosition = Vector3.Lerp(HandleTransform.localPosition, new Vector3(2.5f, 2.21000004f, 0), Time.deltaTime * 5);
            HandleTransform.rotation = Quaternion.Lerp(HandleTransform.rotation, Quaternion.Euler(0, 0, 342.200012f), Time.deltaTime * 5);
        }
        else
        {
            HandleTransform.localPosition = Vector3.Lerp(HandleTransform.localPosition, new Vector3(-1.78999996f, 2.31999993f, 0), Time.deltaTime * 5);
            HandleTransform.rotation = Quaternion.Lerp(HandleTransform.rotation, Quaternion.Euler(0, 0, 49.8813362f), Time.deltaTime * 5);
        }
        currentActivationTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            canInteract = true;
        }
        if (collision.gameObject.tag == "Potion")
        {
            Activate();
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            canInteract = false;
        }
    }

    public void Activate()
    {
        if (activated == true && currentActivationTimer < 0)
        {
            activated = false;
            EScr.activated = false;
            currentActivationTimer = activationTimer;
        }
        else if (currentActivationTimer < 0)
        {
            activated = true;
            EScr.activated = true;
            currentActivationTimer = activationTimer;
        }
    }
}
