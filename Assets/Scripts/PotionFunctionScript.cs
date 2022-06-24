using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionFunctionScript : MonoBehaviour
{

    private SoundManagerScript sms;

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
            
            if (HitObject.tag == "Enemy" || HitObject.tag == "ObjectPickup")
            {
                HitObject.transform.position = player.transform.position;
            }
            player.transform.position = HitPos;
            sms.MakeSound(soundRadius, HitPos);
        }
    }
}
