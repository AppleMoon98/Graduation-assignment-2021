using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRotate : Bullet
{
    public bool isNoHit;

    public float time;
    public float speed;
    public float rotateSpeed;
    public int missileCount;
    public int missileMaxCount;

    public Vector3 dirVec;
    public Rigidbody rigid;
    public Transform target;
    public bool isFlip;
    public bool isStop;
    public bool isDelete;
    public float stopTime;
    public float stopAndNextTime;
    public bool isCross;
    public float crossTime;

    Vector3 offSet;
    BoxCollider boxCollider;
    float basicSpeed;
    bool isStraight = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    void Start()
    {
        offSet = transform.position - target.position;
        basicSpeed = speed;

        if (isNoHit)
            boxCollider.enabled = !isNoHit;

        if(isStop)
            Invoke(nameof(StopSpeed), stopTime);

        if (isCross)
            Invoke(nameof(Cross), crossTime);
    }

    void StopSpeed()
    {
        speed = 0;
        if (isNoHit)
            boxCollider.enabled = isNoHit;

        if (rotateSpeed < 1 && rotateSpeed > -1)
            rotateSpeed = basicSpeed / 4;

        if(isDelete)
            Invoke(nameof(DeleteMe), stopAndNextTime);
        else
            Invoke(nameof(MoveMe), stopAndNextTime);
    }

    void DeleteMe()
    {
        Destroy(gameObject);
    }

    void MoveMe()
    {
        isStraight = true;
        rigid.AddForce(transform.forward * basicSpeed, ForceMode.Impulse);
    }

    void Cross()
    {
        isFlip = !isFlip;
        Invoke(nameof(Cross), crossTime);
    }

    void Update()
    {
        if (!isStraight)
        {
            time = Time.deltaTime;
            transform.position = target.position + dirVec.normalized * Time.deltaTime * speed + offSet;
            if (isFlip)
                transform.RotateAround(target.position,
                                   Vector3.up,
                                   6.5f * -rotateSpeed * Time.deltaTime);
            else
                transform.RotateAround(target.position,
                                       Vector3.up,
                                       6.5f * rotateSpeed * Time.deltaTime);
            offSet = transform.position - target.position;
        }
    }
}
