using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public GameObject[] enemies;
    public List<GameObject> enemiesWithinSound = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }
    public void MakeSound(float soundRadius, Vector3 hitPos)
    {
        for (int i = 0; i < enemies.Length; i++)
        {

            if (Vector3.Distance(hitPos, enemies[i].transform.position) < soundRadius)
            {
                RaycastHit2D GroundCheckBetweenObjects = Physics2D.Linecast(hitPos, enemies[i].transform.position, 1 << LayerMask.NameToLayer("Ground"));
                if (GroundCheckBetweenObjects.collider == null)
                {
                    enemiesWithinSound.Add(enemies[i]);
                }
                if (GroundCheckBetweenObjects.collider != null && Vector3.Distance(hitPos, enemies[i].transform.position) < soundRadius / 1.5)
                {
                    enemiesWithinSound.Add(enemies[i]);
                }
            }
        }
        for (int i = 0; i < enemiesWithinSound.Count; i++)
        {

            enemiesWithinSound[i].GetComponent<AIBase>().HearSound(hitPos);

        }
        enemiesWithinSound.Clear();
    }
}
