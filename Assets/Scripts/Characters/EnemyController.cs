using UnityEngine;
using UnityEngine.AI;
using Utils;
public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }
public enum EnemyType { Normal, Elite, Boss }

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(HealthBarUI))]

public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    protected GameObject attackTarget;
    protected Animator anim;
    private Collider coll;
    protected EnemyStats stats;
    private bool isWalk, isFollow, isChase, isHit, isDead;
    private bool playerDead;
    protected bool speciality;
    private float speed;
    private Vector3 guardPos;
    private Quaternion guardRotation;  // Unity中记录旋转信息的是四元数

    public bool IsDead { set { isDead = value; } }

    [Header("Basic Settings")]
    public float sightRadius;
    [Range(0, 180)]
    public float sightAngle;
    private float chaseSightAngle;
    public bool isGuard;
    public float lookAtTime;
    private float remainLookAtTime;
    private float lastAttackTime;

    [Header("Patrol Settings")]
    public float patrolRange;
    public Vector3 wayPoint;

    [Header("Type Settings")]
    public EnemyType monsterType;
    public int level;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        stats = GetComponent<EnemyStats>();
        coll = GetComponent<Collider>();
        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        chaseSightAngle = sightAngle;
    }

    protected virtual void Start()
    {
        if (isGuard)
            enemyStates = EnemyStates.GUARD;
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }

        //TODO: 怪物类型设计
        if (monsterType == EnemyType.Normal)
            speciality = false;
        else
            speciality = true;


        //FIXME: 场景切换后修改
        GameManager.Instance.AddObserver(this);
    }

    // 切换场景启用
    // void OnEnable()
    // {
    //     GameManager.Instance.AddObserver(this);
    // }
    void OnDisable()
    {
        if (!GameManager.IsInit) return;
        GameManager.Instance.RemoveObserver(this);
    }

    void Update()
    {
        if (!playerDead)
        {
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
    }

    void SwitchStates()
    {
        if (isDead)
            enemyStates = EnemyStates.DEAD;
        else if (FoundPlayer())
            enemyStates = EnemyStates.CHASE;

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

                if (Vector3.SqrMagnitude(wayPoint - transform.position) <= agent.stoppingDistance)
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
                chaseSightAngle = 180;  //  追击中视野变大，防止被绕后脱离

                if (!FoundPlayer())
                {
                    // 脱战停止追击
                    chaseSightAngle = sightAngle;  // 恢复正常视野范围
                    isFollow = false;
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
                        lastAttackTime = stats.attackData.attackRate;

                        // 暴击
                        stats.isCritical = Random.value < stats.attackData.criticalChance;

                        // 攻击
                        Attack();
                        // }
                    }
                }
                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                Destroy(gameObject, 2f);
                break;
        }
    }

    void SwitchAnimation()
    {
        anim.SetBool("walk", isWalk);
        anim.SetBool("follow", isFollow);
        anim.SetBool("chase", isChase);
        anim.SetBool("critical", stats.isCritical);
        anim.SetBool("dead", isDead);
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach (var target in colliders)
        {
            if (target.CompareTag("Player") &&
                transform.IsFacingTarget(target.transform, chaseSightAngle))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }

        attackTarget = null;

        return false;
    }

    protected bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <=
                                    stats.attackData.attackRange;
        else
            return false;

    }

    protected bool TargetInSkillRange()
    {
        if (speciality && attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <=
                                    stats.attackData.skillRange;
        else
            return false;
    }

    protected virtual void Attack()
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

    public void Alarm()
    {
        if (enemyStates != EnemyStates.CHASE)
            enemyStates = EnemyStates.CHASE;
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
        // Gizmos.color = Color.green;
        // Gizmos.DrawWireSphere(transform.position, sightRadius);
        // 绘制视野扇形
        GizmosEx.DrawWireArc(transform, sightRadius, sightAngle, Color.green, 50, 1);
        // GizmosEx.DrawWireArc(transform, sightRadius, chaseSightAngle, Color.red, 50, 1);
    }

    // Animation Event
    void Hit()
    {
        // 防止攻击时目标走出范围报错; 判断玩家是否走到背后
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform, sightAngle))
        {
            // Debug.Log("player!");
            var targetStats = attackTarget.GetComponent<PlayerStats>();
            targetStats.TakeDamage(stats.Damage, stats.isCritical);
        }
    }

    public void EndNotify()
    {
        // 获胜动画
        // 停止移动
        // 停止Agent
        playerDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;
        anim.SetBool("win", true);
    }
}
