using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 direction;

    [Header("Basic Settings")]
    public float force;
    public GameObject target;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void FlyToTarget()
    {
        direction = (target.transform.position -
                     transform.position + Vector3.up).normalized;

        rb.AddForce(direction * force, ForceMode.Impulse);

    }
}
