using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    public Animator anim;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
    }

    void Update()
    {
        SwitchAnimation();
    }

    public void MoveToTarget(Vector3 target)
    {
        agent.destination = target;
    }

    private void SwitchAnimation()
    {
        // NevMeshAgent.velocity 是一个 Vector3 的变量，使用 sqrMagnitude 成员变量获取速度向量的平方长度
        // sqrMagnitude 平方获取要比 Magnitude 快一些，一般使用 sqrMagnitude
        anim.SetFloat("speed", agent.velocity.sqrMagnitude);
    }
}
