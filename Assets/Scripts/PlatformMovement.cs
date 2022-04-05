using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{

    public Rigidbody2D rb2d;
    public float speed;
    public GameObject[] platform_guides;
    int guide_num = 1;
    public bool reverse_at_end = true;
    public float pause_time;
    bool stopped = false;
    float time_waited = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb2d.gravityScale = 0.0f;
    }

    private void FixedUpdate()
    {
        if (!stopped)
        {
            transform.position = Vector2.MoveTowards(transform.position, platform_guides[guide_num].transform.position, Time.deltaTime * speed);
            if (transform.position == platform_guides[guide_num].transform.position)
            {
                guide_num += 1;
                stopped = true;
                time_waited = 0.0f;
                if (guide_num == platform_guides.Length)
                {
                    if (reverse_at_end)
                    {
                        System.Array.Reverse(platform_guides);
                        guide_num = 1;
                    }
                    else guide_num = 0;
                }
            }
        }
        else
        {
            time_waited += Time.deltaTime;
            if (time_waited >= pause_time)
                stopped = false;
        }
    }
}