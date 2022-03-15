using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D };
    public Type enemyType;
    public float maxHealth;
    public float curHealth;
    public int score;
    public GameManager manager;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public GameObject[] coins;
    public bool isChase;
    public bool isAttack;
    public bool isDamaged = false;
    public bool isDead;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;
    public AudioSource ondamage;

    public GameObject Effect_Fire;
    public bool isFireEffect;
    public bool isIceEffect;

    public float tickDamage;
    public int tickTime;
    public int iceTickTime;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if (enemyType != Type.D)
            Invoke(nameof(ChaseStart), 2f);
    }

    void Start()
    {
        if (manager.stage <= 5)
            curHealth *= 1 + (manager.stage * 0.025f);
        else if (manager.stage <= 10)
            curHealth *= 1 + (manager.stage * 0.05f);
        else if (manager.stage <= 15)
            curHealth *= 1 + (manager.stage * 0.075f);
        else if (manager.stage <= 20)
            curHealth *= 1 + (manager.stage * 0.1f);
        else if (manager.stage <= 30)
            curHealth *= 1 + (manager.stage * 0.125f);
        else if (manager.stage <= 40)
            curHealth *= 1 + (manager.stage * 0.15f);
        else if (manager.stage <= 50)
            curHealth *= 1 + (manager.stage * 0.175f);
        else if (manager.stage <= 60)
            curHealth *= 1 + (manager.stage * 0.2f);
        else if (manager.stage <= 70)
            curHealth *= 1 + (manager.stage * 0.225f);
        else if (manager.stage <= 80)
            curHealth *= 1 + (manager.stage * 0.25f);
        else
            curHealth *= 1 + (manager.stage * 0.3f);
        maxHealth = curHealth;
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {
        if (nav.enabled && enemyType != Type.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Targerting()
    {
        if (!isDead && enemyType != Type.D)
        {
            float targetRadius = 0;
            float targetRange = 0;

            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
                case Type.B:
                    targetRadius = 1f;
                    if (manager.stage > 10)
                        targetRange = 20f;
                    else
                        targetRange = 12f;
                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;
            }

            RaycastHit[] rayHits =
                Physics.SphereCastAll(transform.position,
                                            targetRadius,
                                            transform.forward,
                                            targetRange,
                                            LayerMask.GetMask("Player"));

            if (rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }


    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetTrigger("doAttack");
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);

                if (manager.stage <= 10)
                {
                    rigid.AddForce(transform.forward * 30, ForceMode.Impulse);
                    meleeArea.enabled = true;

                    yield return new WaitForSeconds(0.5f);
                    rigid.velocity = Vector3.zero;
                    meleeArea.enabled = false;

                    yield return new WaitForSeconds(2f);
                }
                else
                {
                    anim.SetBool("isWalk", false);
                    for (float index = 0; index < 8; index++)
                    {
                        GameObject instantMissile = Instantiate(bullet, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
                        Rigidbody rigid = instantMissile.GetComponent<Rigidbody>();
                        Vector3 dirVec = new Vector3(Mathf.Cos(Mathf.PI * 2 * index / 8),
                                                                0,
                                                                Mathf.Sin(Mathf.PI * 2 * index / 8));
                        rigid.AddForce(dirVec.normalized * 20, ForceMode.Impulse);

                        Vector3 rotVec = Vector3.down * 360 * index / 8 + Vector3.up * 90;
                        instantMissile.transform.Rotate(rotVec);
                    }
                    yield return new WaitForSeconds(3f);
                    anim.SetBool("isWalk", true);
                }
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                if (manager.stage <= 10)
                {
                    GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                    Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                    rigidBullet.velocity = transform.forward * 20;
                }
                else
                {
                    for (int index = 0; index < 3; index++)
                    {
                        GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                        Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                        rigidBullet.velocity = transform.forward * 30;
                        anim.SetTrigger("doAttack");
                        yield return new WaitForSeconds(0.5f);
                    }
                }

                yield return new WaitForSeconds(2f);
                break;
        }

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    void FixedUpdate()
    {
        Targerting();
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Melee")
        {
            if (!isDamaged)
            {
                isDamaged = true;
                Weapon weapon = other.GetComponent<Weapon>();
                curHealth -= weapon.damage;

                if (weapon.effectType == Weapon.EffectType.Fire)
                {
                    CancelInvoke(nameof(StatusEffect_Fire));
                    tickDamage = weapon.damage * 0.2f;
                    tickTime = weapon.effectTime;
                    StatusEffect_Fire();
                }
                else if (weapon.effectType == Weapon.EffectType.Ice)
                {
                    StopCoroutine(StatusEffect_Ice());
                    iceTickTime = weapon.effectTime;
                    StartCoroutine(StatusEffect_Ice());
                }

                ondamage.Play();
                Vector3 reactVec = transform.position - other.transform.position;

                StopCoroutine(OnDamage(reactVec, false));
                StartCoroutine(OnDamage(reactVec, false));

                Invoke(nameof(isDamagedFalse), 0.9f * manager.player.equipWeapon.rate);
            }
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;

            if (bullet.effectType == Weapon.EffectType.Fire)
            {
                CancelInvoke(nameof(StatusEffect_Fire));
                tickDamage = bullet.damage * 0.2f;
                tickTime = bullet.effectTime;
                StatusEffect_Fire();
            }
            else if (bullet.effectType == Weapon.EffectType.Ice)
            {
                StopCoroutine(StatusEffect_Ice());
                iceTickTime = bullet.effectTime;
                StartCoroutine(StatusEffect_Ice());
            }

            ondamage.Play();
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StopCoroutine(OnDamage(reactVec, false));
            StartCoroutine(OnDamage(reactVec, false));
        }
        else if (other.tag == "BossBullet")
        {
            BossMissile bossMissile = other.GetComponent<BossMissile>();
            if (bossMissile.isSelfDestruct)
            {
                curHealth -= maxHealth * 0.05f;
                ondamage.Play();
                Vector3 reactVec = transform.position - other.transform.position;
                Destroy(other.gameObject);

                StopCoroutine(OnDamage(reactVec, false));
                StartCoroutine(OnDamage(reactVec, false));
            }
        }

    }

    void isDamagedFalse()
    {
        isDamaged = false;
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 250 * (1 + (manager.stage / 100));
        if (curHealth < 0)
            curHealth = 0;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;

        curHealth = curHealth < 0 ? 0 : curHealth;

        if (curHealth > 0)
        {
            yield return new WaitForSeconds(0.1f);
            if (!isFireEffect && !isIceEffect)
                foreach (MeshRenderer mesh in meshs)
                    mesh.material.color = Color.white;
        }
        else if (!isDead)
        {
            gameObject.layer = 14;
            isDead = true;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");

            Player player = target.GetComponent<Player>();
            player.score += (int)(score * (1 + manager.stage * 0.1));
            int ranCoin = Random.Range(0, 3);
            Instantiate(coins[ranCoin], transform.position, Quaternion.identity);   // 아이템 드랍

            switch (enemyType)
            {
                case Type.A:
                    player.coin += (int)(Random.Range(manager.stage <= 10 ? 300 : 600, manager.stage <= 10 ? 600 : 1200) * (1 + manager.stage * 0.05) / manager.stage);
                    manager.enemyCutA--;
                    break;
                case Type.B:
                    player.coin += (int)(Random.Range(manager.stage <= 10 ? 400 : 800, manager.stage <= 10 ? 700 : 1400) * (1 + manager.stage * 0.05) / manager.stage);
                    manager.enemyCutB--;
                    break;
                case Type.C:
                    player.coin += (int)(Random.Range(manager.stage <= 10 ? 550 : 1100, manager.stage <= 10 ? 1000 : 2000) * (1 + manager.stage * 0.05) / manager.stage);
                    manager.enemyCutC--;
                    break;
                case Type.D:
                    player.coin += (int)(manager.stage <= 10 ? 2500 : 5000 * (1 + manager.stage * 0.05));
                    manager.enemyCutD--;
                    break;
            }

            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }

            isDie();
        }
    }

    void StatusEffect_Fire()
    {
        isFireEffect = true;
        curHealth -= (int)tickDamage;
        Effect_Fire.SetActive(true);
        StartCoroutine(OnDamage(new Vector3(0, 0, 0), false));
        ondamage.Play();
        tickTime--;
        if (tickTime > 0 && !isDead)
        {
            Invoke(nameof(StatusEffect_Fire), 0.5f);
        }
        else
        {
            Effect_Fire.SetActive(false);
            isFireEffect = false;
        }
    }

    IEnumerator StatusEffect_Ice()
    {
        isIceEffect = true;
        nav.enabled = false;
        FreezeVelocity();

        yield return new WaitForSeconds(0.2f);
        for (int index = 0; index < iceTickTime; index++)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.blue;

            isIceEffect = true;
            nav.enabled = false;
            FreezeVelocity();

            yield return new WaitForSeconds(0.1f);
        }

        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.white;

        nav.enabled = true;
        isIceEffect = false;
    }

    void isDie()
    {
        isFireEffect = false;
        if (enemyType == Type.A || (enemyType == Type.B && manager.stage <= 10))
            meleeArea.enabled = false;
        StopAllCoroutines();
        CancelInvoke();

        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.gray;

        Destroy(gameObject, 4);
    }
}
