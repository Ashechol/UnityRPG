using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    public Animator anim;
    public CapsuleCollider capColl;
    public CharacterStats characterStats;
    private GameObject attackTarget;
    private float lastAttackTime;
    private bool isDead;

    public bool IsDead { set { isDead = value; } }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        capColl = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        // += 多播委托：一个委托代表多个方法
        // 将方法订阅到事件
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;

        GameManager.Instance.RegisterPlayer(characterStats);
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
    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;

        transform.LookAt(attackTarget.transform);

        while (Vector3.Distance(attackTarget.transform.position,
                       transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;  // yield return 暂时挂起，下一帧继续从这里开始执行协程
        }

        agent.isStopped = true;

        // Attack
        if (lastAttackTime < 0)
        {
            characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
            anim.SetBool("critical", characterStats.isCritical);
            anim.SetTrigger("attack");
            // 重置冷却时间
            lastAttackTime = characterStats.attackData.coolDown;
        }

    }

    // Animation Event
    void Hit()
    {
        var targetStats = attackTarget.GetComponent<CharacterStats>();
        targetStats.TakeDamage(characterStats);
    }
}
