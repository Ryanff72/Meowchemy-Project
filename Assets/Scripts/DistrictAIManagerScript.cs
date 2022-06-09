using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistrictAIManagerScript : MonoBehaviour
{
    public GameObject[] Troops;
    public bool HasCalled = false;

    // Update is called once per frame
    public void CallAll()
    {
        for (int i = 0; i < Troops.Length; i++)
        {
            if (Troops[i].GetComponent<AIBase>().aiState != AIBase.AIState.dead)
            {
                Troops[i].GetComponent<AIBase>().aiState = AIBase.AIState.aggro;
                Troops[i].transform.GetChild(2).gameObject.SetActive(false);
                Troops[i].transform.GetChild(3).gameObject.SetActive(false);
            }
        }
        HasCalled = true;
    }
}
