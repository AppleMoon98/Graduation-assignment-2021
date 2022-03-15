using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : BossPattern
{
    public Parttern[] partterns;
    public int[] shotMissile;
    public int[] maxMissile;
    public float[] nextShotTime;
    public float[] nextPartternTime;
    public float[] speed;

    public GameObject[] missile;
    public int[] missileNum;

    public Vector3 lookVec;
    public Vector3 tauntVec;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true;
        StartCoroutine(Think());
    }

    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }
        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
        else if(!isIceEffect)
                nav.SetDestination(tauntVec);
    }

    IEnumerator Think()
    {
        for(int index = 0; index < partterns.Length; index++)
        {
            StartCoroutine(PartternSelect(partterns[index],missile[missileNum[index]], 
                shotMissile[index], maxMissile[index], nextShotTime[index], 
                nextPartternTime[index], speed[index]));

            yield return new WaitForSeconds(partterns[index] != Parttern.Quadrant ? 
                nextPartternTime[index] + nextShotTime[index] * shotMissile[index] : 
                (nextPartternTime[index] + nextShotTime[index] * shotMissile[index]) * 2);
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(Think());
    }
}
