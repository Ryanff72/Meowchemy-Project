using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundParticlesScript : MonoBehaviour
{

    public GameObject SoundParticles;
    [SerializeField] private int particleCount;
    private float zRotValue;
    [SerializeField] float speed;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < particleCount; i++)
        {
            transform.rotation = Quaternion.Euler(0, 0, zRotValue);
            zRotValue += 10;
            GameObject newParticle = Instantiate(SoundParticles, transform.position, transform.rotation);
            newParticle.transform.localScale = new Vector3(0, 0, 0);
            newParticle.GetComponent<Rigidbody2D>().velocity = newParticle.transform.up * speed;
        }
    }

}
