using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioProximity : MonoBehaviour
{
    AudioSource audiosrc;
    [SerializeField] bool isLooping;
    Transform Player;
    float canPlayMaxTime = 5;
    float canPlayCurrentTime = 0;
    [SerializeField] float mxVolm;
    [SerializeField] float maxDist = 1;
    [SerializeField] AudioClip audiocp;
    // Start is called before the first frame update
    void Awake()
    {
        Player = GameObject.Find("Player").transform;
        audiosrc = GetComponent<AudioSource>();
        if (isLooping == true)
        {
            audiosrc.clip = audiocp;
            audiosrc.loop = true;
            audiosrc.Play();
        }
    }

    private void FixedUpdate()
    {
        canPlayCurrentTime -= Time.deltaTime;
        audiosrc.volume = mxVolm - (Vector2.Distance(transform.position, Player.position) / maxDist);
    }

    public void PlaySound(AudioClip audioclp, float maxDistance, float maxVolume)
    {
        mxVolm = maxVolume;
        maxDist = maxDistance;
        audiocp = audioclp;
        if (canPlayCurrentTime <= 0)
        {
            
            canPlayCurrentTime = canPlayMaxTime;
            
            audiosrc.PlayOneShot(audioclp);
        }
        
    }
}
