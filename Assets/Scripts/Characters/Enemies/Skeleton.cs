using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : EnemyController
{
    public int skillCountDown;
    private int attackCount;

    protected override void Awake()
    {
        base.Awake();
        attackCount = 0;
    }

    protected override void Attack()
    {
        transform.LookAt(attackTarget.transform);

        if (TargetInAttackRange() && attackCount < skillCountDown)
        {
            anim.SetTrigger("attack");
            attackCount++;
            Debug.Log(attackCount);
        }

        if (TargetInSkillRange() && attackCount >= skillCountDown)
        {
            anim.SetTrigger("skill");
            attackCount = 0;
        }

    }
}
