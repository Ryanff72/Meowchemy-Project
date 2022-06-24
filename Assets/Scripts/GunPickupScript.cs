using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickupScript : MonoBehaviour
{
    private GameObject player;
    private bool pickupable;
    public int ammo;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

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
