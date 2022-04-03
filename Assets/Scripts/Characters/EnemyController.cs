using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DIE }

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyController : MonoBehaviour
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private GameObject attackTarget;
    private Animator anim;
    private bool isWalk, isFollow, isChase;
    private float speed;

    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;

    [Header("Patrol Settings")]
    public float patrolRange;
    public Vector3 wayPoint;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        speed = agent.speed;
    }

    void Start()
    {
        if (isGuard)
            enemyStates = EnemyStates.GUARD;
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
    }
    void Update()
    {
        SwitchStates();
        SwitchAnimation();
        Debug.Log(agent.destination);
    }

    void SwitchStates()
    {
        if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
            // Debug.Log("Player Found!");
        }

        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.6f;

                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    GetNewWayPoint();
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }

                break;
            case EnemyStates.CHASE:
                agent.speed = speed;
                isWalk = false;
                isChase = true;
                //TODO: 做一个走出区域脱战
                if (!FoundPlayer())
                {
                    isFollow = false;
                    agent.destination = transform.position;  // 脱战停止追击
                }
                else
                {
                    agent.destination = attackTarget.transform.position;
                    isFollow = true;
                }
                break;
            case EnemyStates.DIE:
                break;
        }
    }

    void SwitchAnimation()
    {
        anim.SetBool("walk", isWalk);
        anim.SetBool("follow", isFollow);
        anim.SetBool("chase", isChase);
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }

        attackTarget = null;

        return false;
    }

    void GetNewWayPoint()
    {
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        // FIXME: Bug
        Vector3 randomPoint = new Vector3(transform.position.x + randomX,
                                          transform.position.y,
                                          transform.position.z + randomZ);

        wayPoint = randomPoint;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
}
