using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    public Animator anim;
    public CapsuleCollider capColl;
    public PlayerStats stats;
    public float hitForce = 20;
    private GameObject attackTarget;
    private float lastAttackTime;
    private float stopDistance;
    private bool isDead;

    public bool IsDead { set { isDead = value; } }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
        capColl = GetComponent<CapsuleCollider>();
        stopDistance = agent.stoppingDistance;
    }

    void Start()
    {
        // += 多播委托：一个委托代表多个方法
        // 将方法订阅到事件
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;

        GameManager.Instance.RegisterPlayer(stats);
    }


    private void Update()
    {
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        // NevMeshAgent.velocity 是一个 Vector3 的变量，使用 sqrMagnitude 成员变量获取速度向量的平方长度
        // sqrMagnitude 平方获取要比 Magnitude 快一些，一般使用 sqrMagnitude
        anim.SetFloat("speed", agent.velocity.sqrMagnitude);
        anim.SetBool("dead", isDead);
    }

    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines(); // 停止其他协程
        if (isDead) return;
        agent.isStopped = false;
        agent.destination = target;
        agent.stoppingDistance = stopDistance;  // 正常移动不需要按照攻击距离判断
    }

    private void EventAttack(GameObject target)
    {
        if (isDead) return;

        if (target != null)  // 防止敌人死亡后目标消失报错
        {
            attackTarget = target;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    // Coroutine
    //TODO: 优化代码结构
    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = stats.attackData.attackRange;  // 停止距离变为攻击距离，防止被体形大的敌人阻挡

        transform.LookAt(attackTarget.transform);

        if (attackTarget.CompareTag("Attackable"))
        {
            var closestPoint = attackTarget.GetComponent<Collider>().bounds.ClosestPoint(transform.position);
            while (Vector3.Distance(closestPoint, transform.position) > 1)
            {
                agent.destination = attackTarget.transform.position;
                yield return null;  // yield return 暂时挂起，下一帧继续从这里开始执行协程
            }
        }
        else
            while (Vector3.Distance(attackTarget.transform.position,
                           transform.position) > stats.attackData.attackRange)
            {
                agent.destination = attackTarget.transform.position;
                yield return null;  // yield return 暂时挂起，下一帧继续从这里开始执行协程
            }

        agent.isStopped = true;

        // Attack
        if (lastAttackTime < 0)
        {
            stats.isCritical = Random.value < stats.attackData.criticalChance;
            anim.SetBool("critical", stats.isCritical);
            anim.SetTrigger("attack");
            // 重置冷却时间
            lastAttackTime = stats.attackData.coolDown;
        }

    }

    // Animation Event
    void Hit()
    {
        if (attackTarget.CompareTag("Enemy"))
        {
            var targetStats = attackTarget.GetComponent<EnemyStats>();
            targetStats.TakeDamage(stats);
        }

        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>())
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward *
                                                                hitForce, ForceMode.Impulse);
                attackTarget.GetComponent<Collider>().enabled = false;
            }
        }
    }
}
