using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public enum DamageType { NORMAL, SKILL }
    Collider coll;
    DamageType dmgType;
    public bool isHit;
    public CharacterStats attacker;

    public DamageType DmgType { set { dmgType = value; } }

    void Awake()
    {
        coll = GetComponent<Collider>();
        attacker = GetComponentInParent<CharacterStats>();
    }

    public void OnTriggerEnter(Collider other)
    {
        //TODO: 能否优化?
        if (other.CompareTag("Player"))
        {
            if (dmgType == DamageType.NORMAL)
                other.GetComponent<PlayerStats>().TakeDamage(attacker.Damage, attacker.isCritical);

            if (dmgType == DamageType.SKILL)
                other.GetComponent<PlayerStats>().TakeDamage(attacker.SkillDamage, true);
        }
    }
}
