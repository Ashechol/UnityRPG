using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 25;
    public float throwForce = 30;
    public GameObject rockPrefab;
    public Transform handPos;

    // Animation Event
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform, sightAngle))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            attackTarget.GetComponent<Animator>().SetTrigger("dizzy");

            targetStats.TakeDamage(characterStats);
        }
    }

    // Animation Event
    public void ThrowRock()
    {
        if (attackTarget == null)
            attackTarget = FindObjectOfType<PlayerController>().gameObject;

        var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
        // rock.GetComponent<Rock>().target = attackTarget;
        // rock.GetComponent<Rock>().force = throwForce;
        rock.GetComponent<Rock>().FlyToTarget(characterStats.SkillDamage, attackTarget, throwForce);
    }

}
