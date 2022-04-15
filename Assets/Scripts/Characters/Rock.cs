using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer, HitEnemy, None }
    private Rigidbody rb;
    private Vector3 direction;

    private int damage;
    private float force;
    public GameObject target;
    public GameObject breakEffect;
    public RockStates rockStates;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockStates = RockStates.HitPlayer;
    }

    void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1f)
        {
            gameObject.tag = "Attackable";
            rockStates = RockStates.None;
        }
    }

    public void FlyToTarget(int skillDamage, GameObject target, float throwForce)
    {
        damage = skillDamage;
        force = throwForce;

        direction = (target.transform.position -
                     transform.position + Vector3.up).normalized;

        rb.AddForce(direction * force, ForceMode.Impulse);

    }

    void OnCollisionEnter(Collision other)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;

                    other.gameObject.GetComponent<Animator>().SetTrigger("dizzy");

                    other.gameObject.GetComponent<PlayerStats>().TakeDamage(damage);

                    rockStates = RockStates.None;
                }

                break;

            case RockStates.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<EnemyStats>();
                    otherStats.TakeDamage(damage);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    other.gameObject.GetComponent<Animator>().SetTrigger("hit");
                    Destroy(gameObject);
                }

                break;

            case RockStates.None:


                break;
        }
    }
}
