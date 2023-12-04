using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogController : MonoBehaviour
{
    [SerializeField] GameObject targetObject;

    NavMeshAgent agent;
    float attackDistance=3f;
    bool isAttack=false;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();    
    }

    // Update is called once per frame
    void Update()
    {
        // NavMeshが準備できているなら
        if (agent.pathStatus != NavMeshPathStatus.PathInvalid)
        {
            // NavMeshAgentに目的地をセット
            agent.SetDestination(targetObject.transform.position);
        }
        if(Vector3.Distance( agent.transform.position,targetObject.transform.position)<=attackDistance)
        {
            Debug.Log("到着");
            agent.isStopped = true;
            isAttack = true;
        }

    }

    public float GetMoveVectorMagnitude()
    {
        return agent.velocity.magnitude;
    }
    public bool IsRotation()
    {
        Debug.Log(agent.angularSpeed);
        if (agent.angularSpeed>=30f)
        {

            return true;
        }

        return false;
    }
    public bool IsAttack() 
    {
        return isAttack;
    }
}
