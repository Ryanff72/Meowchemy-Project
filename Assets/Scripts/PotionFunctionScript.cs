using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionFunctionScript : MonoBehaviour
{

    private SoundManagerScript sms;
    public GameObject TeleportSmoke;


    private void Start()
    {
        sms = GameObject.Find("SoundManager").GetComponent<SoundManagerScript>();
    }

    public void potionCollide (string potionType, List<GameObject> enemiesInSplash, List<GameObject> objectsInSplash, float soundRadius, List<float> distanceToEnemies, List<float> distanceToObjects, Vector3 HitPos, float distanceToPlayer, GameObject player, bool wasPlayerHit, GameObject HitObject)
    {
        if (potionType == "BombPotion")
        {
            for (int i = 0; i < enemiesInSplash.Count; i++)
            {
                //if (enemiesInSplash[i].gameObject.GetComponent<AIBase>().aiState != AIBase.AIState.dead)
                //{
                if (enemiesInSplash[i].gameObject.GetComponent<AIBase>().aiState != AIBase.AIState.dead)
                {
                    enemiesInSplash[i].gameObject.GetComponent<AIBase>().aiState = AIBase.AIState.dead;
                }
                    Vector3 NormVel = Vector3.Normalize(enemiesInSplash[i].gameObject.transform.position - HitPos);
                    enemiesInSplash[i].gameObject.GetComponent<AIBase>().velocity = new Vector3(NormVel.x * (50f-distanceToEnemies[i]), NormVel.y * (65f - distanceToEnemies[i]), 0);
                //}
            }
            for (int i = 0; i < objectsInSplash.Count; i++)
            {
                
                Vector3 NormVel = Vector3.Normalize(objectsInSplash[i].gameObject.transform.position - HitPos);
                objectsInSplash[i].gameObject.GetComponent<SimpleBoxObjectPhysics>().velocity = new Vector3(NormVel.x * (30f - distanceToObjects[i]), NormVel.y * (50f - distanceToObjects[i]), 0);
            }
            sms.MakeSound(soundRadius, HitPos);
            if (wasPlayerHit == true)
            {
                player.GetComponent<PlayerController>().ps = PlayerController.PlayerState.dead;
                Vector3 NormVel = Vector3.Normalize(player.gameObject.transform.position - HitPos);
                player.GetComponent<PlayerController>().velocity = new Vector2(NormVel.x * (50f - distanceToPlayer), (NormVel.y * (65f - distanceToPlayer)));
            }
        }
        if (potionType == "SoundPotion")
        {
            sms.MakeSound(soundRadius, HitPos);
        }
        if (potionType == "TeleportPotion")
        {
            if (HitObject.tag == "Object")
            {
                if (Physics2D.Linecast(player.transform.position, player.transform.position + new Vector3(-2, 0, 0), 1 << LayerMask.NameToLayer("Ground")))
                {
                    Debug.Log("1");
                    HitObject.transform.position = player.transform.position + new Vector3(2f, 0, 0);
                }
                else if (Physics2D.Linecast(player.transform.position, player.transform.position + new Vector3(2, 0, 0), 1 << LayerMask.NameToLayer("Ground")))
                {
                    Debug.Log("2");
                    HitObject.transform.position = player.transform.position + new Vector3(-2f, 0, 0);
                }
                else
                {
                    HitObject.transform.position = player.transform.position;
                }
                Instantiate(TeleportSmoke, player.transform.position - new Vector3(0, 0.2f, 0), Quaternion.identity);
            }
            if (HitObject.tag == "Potion")
            {
                HitObject.transform.position = player.transform.position;
                Instantiate(TeleportSmoke, player.transform.position - new Vector3(0, 0.2f, 0), Quaternion.identity);
            }
            if (HitObject.tag == "Enemy")
            {
                RaycastHit2D CheckRight = Physics2D.Linecast(player.transform.position, player.transform.position + new Vector3(1.5f,0,0), 1 << LayerMask.NameToLayer("Ground"));
                RaycastHit2D CheckLeft = Physics2D.Linecast(player.transform.position, player.transform.position + new Vector3(-1.5f,0, 0), 1 << LayerMask.NameToLayer("Ground"));
                if (CheckLeft.collider != null)
                {
                    HitObject.GetComponent<AIBase>().Teleport(player.transform.position + new Vector3(-2f, 0, 0));
                }
                else if (CheckRight.collider != null)
                {
                    HitObject.GetComponent<AIBase>().Teleport(player.transform.position + new Vector3(2f, 0, 0));
                }
                else
                {
                    HitObject.GetComponent<AIBase>().Teleport(player.transform.position);
                }
            }
            if (GetComponent<PotionBase>().hitDir == "left")
            {
                player.transform.position = HitPos + new Vector3(3, 0, 0);
            }
            else if (GetComponent<PotionBase>().hitDir == "right")
            {
                player.transform.position = HitPos + new Vector3(-3, 0, 0);
            }
            else
            {
                player.transform.position = HitPos;
            }
            Instantiate(TeleportSmoke, player.transform.position - new Vector3(0,0.85f,0), Quaternion.identity);
            sms.MakeSound(soundRadius, HitPos);
        }
    }
}
