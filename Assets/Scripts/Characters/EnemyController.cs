using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyController : MonoBehaviour
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private GameObject attackTarget;
    private Animator anim;
    private Collider coll;
    private CharacterStats characterStats;
    private bool isWalk, isFollow, isChase, isHit, isDead;
    private float speed;
    private Vector3 guardPos;
    private Quaternion guardRotation;  // Unity中记录旋转信息的是四元数

    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;
    public float lookAtTime;
    private float remainLookAtTime;
    private float lastAttackTime;

    [Header("Patrol Settings")]
    public float patrolRange;
    public Vector3 wayPoint;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();
        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
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
        if (characterStats.CurrentHealth == 0)
            isDead = true;
        SwitchStates();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    void SwitchStates()
    {
        if (isDead)
            enemyStates = EnemyStates.DEAD;

        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
            // Debug.Log("Player Found!");
        }

        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;
                remainLookAtTime = lookAtTime;
                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.02f);
                    // 转回角度时保持移动动画
                    if (Quaternion.Angle(transform.rotation, guardRotation) <= 10)
                        isWalk = false;
                }
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.6f;

                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    else
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
                    // 脱战停止追击
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                        // Debug.Log("remainLookAtTime: " + remainLookAtTime);
                    }
                    else if (isGuard)
                        enemyStates = EnemyStates.GUARD;
                    else
                        enemyStates = EnemyStates.PATROL;
                }
                else
                {
                    agent.destination = attackTarget.transform.position;
                    isFollow = true;
                    agent.isStopped = false;
                }

                // 攻击范围检测
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;

                        // 暴击
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;

                        // 攻击
                        Attack();
                        // }
                    }
                }
                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                agent.enabled = false;  // 关闭 Agent 防止死亡后继续移动和攻击
                Destroy(gameObject, 2f);
                break;
        }
    }

    void SwitchAnimation()
    {
        anim.SetBool("walk", isWalk);
        anim.SetBool("follow", isFollow);
        anim.SetBool("chase", isChase);
        anim.SetBool("critical", characterStats.isCritical);
        anim.SetBool("dead", isDead);
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

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <=
                                    characterStats.attackData.attackRange;
        else
            return false;

    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <=
                                    characterStats.attackData.skillRange;
        else
            return false;
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);

        if (TargetInAttackRange())
        {
            anim.SetTrigger("attack");
        }

        if (TargetInSkillRange())
        {
            anim.SetTrigger("skill");
        }
    }

    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime * 1.5f;
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX,
                                          transform.position.y,  // y轴使用当前坐标防止悬浮
                                          guardPos.z + randomZ);

        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ?
                   hit.position : transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    // Animation Event
    void Hit()
    {
        if (attackTarget != null)  // 防止攻击时目标走出范围报错
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
}
