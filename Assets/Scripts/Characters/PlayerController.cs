using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    public Animator anim;
    public CharacterStats characterStats;
    private GameObject attackTarget;
    private float lastAttackTime;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
    }

    void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
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
    }

    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines(); // 停止其他协程
        agent.isStopped = false;
        agent.destination = target;
    }

    private void EventAttack(GameObject target)
    {
        if (target != null)  // 防止敌人死亡后目标消失报错
        {
            attackTarget = target;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;

        transform.LookAt(attackTarget.transform);

        //TODO: 修改攻击范围
        while (Vector3.Distance(attackTarget.transform.position,
                transform.position) > 1)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;  // yield return 暂时挂起，下一帧继续执行协程
        }

        agent.isStopped = true;

        // Attack
        if (lastAttackTime < 0)
        {
            anim.SetTrigger("attack");
            // 重置冷却时间
            lastAttackTime = 0.5f;
        }

    }
}
