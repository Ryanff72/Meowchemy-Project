using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionFunctionScript : MonoBehaviour
{

    public void potionCollide (string potionType, List<GameObject> enemiesInSplash , List<float> distanceToEnemies, Vector3 HitPos, float distanceToPlayer, GameObject player, bool wasPlayerHit)
    {
        if (potionType == "BombPotion")
        {
            for (int i = 0; i < enemiesInSplash.Count; i++)
            {
                if (enemiesInSplash[i].gameObject.GetComponent<AIBase>().aiState != AIBase.AIState.dead)
                {
                    enemiesInSplash[i].gameObject.GetComponent<AIBase>().aiState = AIBase.AIState.dead;
                    Vector3 NormVel = Vector3.Normalize(enemiesInSplash[i].gameObject.transform.position - HitPos);
                    enemiesInSplash[i].gameObject.GetComponent<AIBase>().velocity = new Vector3(NormVel.x * (45f-distanceToEnemies[i]), (NormVel.y * (60f - distanceToEnemies[i])+8f), 0);
                        
                        //new Vector3((enemiesInSplash[i].gameObject.transform.position.x - HitPos.x) * 11f, 
                        //(enemiesInSplash[i].gameObject.transform.position.y - HitPos.y + 3) * 6f, 0);

                }
                

            }
            if (wasPlayerHit == true)
            {
                player.GetComponent<PlayerController>().ps = PlayerController.PlayerState.dead;
                Vector3 NormVel = Vector3.Normalize(player.gameObject.transform.position - HitPos);
                player.GetComponent<PlayerController>().velocity = new Vector2(NormVel.x * (35f - distanceToPlayer), (NormVel.y * (80f - distanceToPlayer)));
            }
        }
    }
}
