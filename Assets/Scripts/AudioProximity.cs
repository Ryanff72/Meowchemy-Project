using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioProximity : MonoBehaviour
{
    AudioSource audiosrc;
    [SerializeField] bool isLooping;
    Transform Player;
    float canPlayMaxTime = 0.5f;
    float canPlayCurrentTime = 0;
    // Start is called before the first frame update
    void Awake()
    {

        Player = GameObject.Find("Player").transform;
        audiosrc = GetComponent<AudioSource>();
        if (isLooping == true)
        {
            audiosrc.loop = true;
            audiosrc.Play();
        }
        
    }

    private void Update()
    {
        canPlayCurrentTime -= Time.deltaTime;
    }

    public void PlaySound(AudioClip audioclp, float maxDistance, float maxVolume)
    {
        if (canPlayCurrentTime <= 0)
        {
            canPlayCurrentTime = canPlayMaxTime;
            audiosrc.volume = maxVolume - (Vector2.Distance(transform.position, Player.position) / maxDistance);
            audiosrc.PlayOneShot(audioclp);
        }
        
    }
}
