using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : EnemyController
{
    public int skillCountDown;
    private int attackCount;
    public HitBox hitbox;

    protected override void Awake()
    {
        base.Awake();
        attackCount = 0;
        hitbox = GetComponentInChildren<HitBox>();
    }

    protected override void Attack()
    {
        transform.LookAt(attackTarget.transform);

        if (TargetInAttackRange() && attackCount < skillCountDown)
        {
            hitbox.DmgType = HitBox.DamageType.NORMAL;
            anim.SetTrigger("attack");
            attackCount++;
            Debug.Log(attackCount);
        }
        else if (TargetInSkillRange() && attackCount == skillCountDown)
        {
            hitbox.DmgType = HitBox.DamageType.SKILL;
            anim.SetTrigger("skill");
            attackCount = 0;
        }

    }
}
