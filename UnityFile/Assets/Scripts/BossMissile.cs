using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet
{
    public Transform target;
    public bool isSelfDestruct = false;
    NavMeshAgent nav;
    Rigidbody rigid;

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();

        Invoke(nameof(SelfDestruct), 1f);
    }

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void SelfDestruct()
    {
        isSelfDestruct = true;
    }

    void Update()
    {
        nav.SetDestination(target.position);
    }

    void FixedUpdate()
    {
        if (rigid.transform.position.y > 1)
            Destroy(gameObject);
    }
}
