using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistrictAIManagerScript : MonoBehaviour
{
    public GameObject[] Troops;
    public bool HasCalled = false;
    public float aggroTime = 10;
    public float aggroTimeLeft;

    private void Start()
    {
        aggroTimeLeft = aggroTime;
    }

    private void FixedUpdate()
    {
        if (HasCalled == true)
        {
            aggroTimeLeft -= Time.deltaTime;
            if (aggroTimeLeft <= 0)
            {
                aggroTimeLeft = aggroTime;
                DismissAll();
            }
        }
    }
    // Update is called once per frame
    public void CallAll()
    {
        for (int i = 0; i < Troops.Length; i++)
        {
            if (Troops[i].GetComponent<AIBase>().aiState != AIBase.AIState.dead)
            {
                StartCoroutine(Troops[i].GetComponent<AIBase>().Angry());
                Troops[i].transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(9).gameObject.SetActive(false);
                Troops[i].transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(8).gameObject.SetActive(false);
                Troops[i].transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(true);
                Troops[i].GetComponent<AIBase>().aiState = AIBase.AIState.aggro;
                Troops[i].GetComponent<AIBase>().currentSuspicion = 110;
                //Troops[i].transform.GetChild(2).gameObject.SetActive(false);
                //Troops[i].transform.GetChild(3).gameObject.SetActive(false);
            }
        }
        HasCalled = true;
    }
    public void CallAllSus()
    {
        for (int i = 0; i < Troops.Length; i++)
        {
            if (Troops[i].GetComponent<AIBase>().aiState != AIBase.AIState.dead && Troops[i].GetComponent<AIBase>().aiState != AIBase.AIState.aggro)
            {
                Troops[i].GetComponent<AIBase>().aiState = AIBase.AIState.suspicious;
                StartCoroutine(Troops[i].GetComponent<AIBase>().Worried());
                Troops[i].transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(9).gameObject.SetActive(false);
                Troops[i].transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(8).gameObject.SetActive(true);
            }
        }
    }

    public void DismissAll()
    {
        HasCalled = false;
        for (int i = 0; i < Troops.Length; i++)
        {
            if (Troops[i].GetComponent<AIBase>().aiState != AIBase.AIState.dead)
            {
                Troops[i].GetComponent<AIBase>().currentSuspicion = Troops[i].GetComponent<AIBase>().suspicionTriggerLevel / 2;
                Troops[i].GetComponent<AIBase>().aiState = AIBase.AIState.suspicious;
                Troops[i].GetComponent<AIBase>().steadfast = false;
                StartCoroutine(Troops[i].GetComponent<AIBase>().Worried());
                Troops[i].transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(9).gameObject.SetActive(false);
                Troops[i].transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(8).gameObject.SetActive(true);
                Troops[i].transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(false);
            }
        }
    }
}
