using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptForIndividualSoundParticle : MonoBehaviour
{
    private bool shutDown = false;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x < 0.33f && shutDown == false)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(0.35f, 0.35f), Time.deltaTime *12f);
        }
        else if (transform.localScale.x > 0.33f && transform.localScale.x < 0.35f && shutDown == false)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(0.38f, 0.38f), Time.deltaTime *12f);
            GetComponent<Rigidbody2D>().velocity = Vector2.Lerp(GetComponent<Rigidbody2D>().velocity, Vector2.zero, 2f * Time.deltaTime);
        }
        else
        {
            shutDown = true;
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(0f, 0f), Time.deltaTime * 16f);
            GetComponent<Rigidbody2D>().velocity = Vector2.Lerp(GetComponent<Rigidbody2D>().velocity, Vector2.zero, 6f * Time.deltaTime);
        }
        
    }
}
