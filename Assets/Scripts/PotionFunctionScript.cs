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

    public void potionCollide (string potionType, List<GameObject> enemiesInSplash, float soundRadius, List<float> distanceToEnemies, Vector3 HitPos, float distanceToPlayer, GameObject player, bool wasPlayerHit)
    {
        if (potionType == "BombPotion")
        {
            for (int i = 0; i < enemiesInSplash.Count; i++)
            {
                if (enemiesInSplash[i].gameObject.GetComponent<AIBase>().aiState != AIBase.AIState.dead)
                {
                    enemiesInSplash[i].gameObject.GetComponent<AIBase>().aiState = AIBase.AIState.dead;
                    Vector3 NormVel = Vector3.Normalize(enemiesInSplash[i].gameObject.transform.position - HitPos);
                    enemiesInSplash[i].gameObject.GetComponent<AIBase>().velocity = new Vector3(NormVel.x * (65f-distanceToEnemies[i]), (NormVel.y * (80f - distanceToEnemies[i])+6f), 0);
                }
            }
            sms.MakeSound(soundRadius, HitPos);
            if (wasPlayerHit == true)
            {
                player.GetComponent<PlayerController>().ps = PlayerController.PlayerState.dead;
                Vector3 NormVel = Vector3.Normalize(player.gameObject.transform.position - HitPos);
                player.GetComponent<PlayerController>().velocity = new Vector2(NormVel.x * (65f - distanceToPlayer), (NormVel.y * (80f - distanceToPlayer)));
            }
        }
        if (potionType == "SoundPotion")
        {
            sms.MakeSound(soundRadius, HitPos);
        }
        if(potionType == "TeleportPotion")
        {

        }
    }
}
