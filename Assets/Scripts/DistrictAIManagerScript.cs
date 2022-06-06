using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistrictAIManagerScript : MonoBehaviour
{
    public GameObject[] Troops;
    public bool HasCalled = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void CallAll()
    {
        for (int i = 0; i < Troops.Length; i++)
        {
            Troops[i].GetComponent<AIBase>().aiState = AIBase.AIState.aggro;
            HasCalled = true;
        }
    }
}
