using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterTime : MonoBehaviour
{

    public float lifetime;
    public bool triggered;

    void Update()
    {
        if (triggered == true)
        {
            lifetime -= Time.deltaTime;
            if (lifetime < 0)
            {
                GameObject.Destroy(this.gameObject);
            }
        }
        
    }
}
