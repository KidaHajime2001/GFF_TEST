using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WizardEnemyController : MonoBehaviour
{
    [SerializeField] GameObject targetObject;
    [SerializeField] BaseManager baseManager;
    [SerializeField] SphereCollider col;
    [SerializeField] ParticleSystem bless;
    [SerializeField] ParticleSystem fire;

    NavMeshAgent agent;
    float attackDistance = 3.0f;
    bool isAttack = false;

    Animator animator;
    int animIDAttackFlag;
    int animIDSpeed;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    void SetAnimatorID()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDAttackFlag = Animator.StringToHash("IsAttack");

    }

    // Start is called before the first frame update
    void Start()
    {
        col.enabled = false;
        SetAnimatorID();

        bless.Stop();
        fire.Stop();
    }

    // Update is called once per frame
    void Update()
    {        // NavMeshが準備できているなら
        if (agent.pathStatus != NavMeshPathStatus.PathInvalid)
        {
            // NavMeshAgentに目的地をセット
            agent.SetDestination(targetObject.transform.position);
        }
        if (Vector3.Distance(agent.transform.position, targetObject.transform.position) <= attackDistance)
        {
            Debug.Log("到着");
            agent.isStopped = true;
            isAttack = true;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            agent.isStopped = false;
            isAttack = false;
            agent.SetDestination(baseManager.GetComponent<BaseManager>().GetNearBasesPosition(transform.position));
            ;
        }
        if (isAttack)
        {
            animator.SetBool(animIDAttackFlag, true);
        }
        else
        {
            Debug.Log(agent.speed);
            animator.SetBool(animIDAttackFlag, false);
            animator.SetFloat(animIDSpeed, agent.speed);
        }


    }
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = col.enabled ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(col.transform.position, col.radius);
    }
    public void StartBless()
    {
        Debug.Log(bless!);
        col.enabled = true;
        bless.Play();
        fire.Play();
    }

    public void StopBless()
    {
        col.enabled = false;
        bless.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        fire.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
