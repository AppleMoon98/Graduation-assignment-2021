using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // #. 개인 설명 - 코루틴
    // 기본 => Use() : 메인루틴 > Swing() 서브루틴 > Use() 메인루틴
    // >> 즉 실행되면 개인적으로 따로따로 실행이 됨

    // 코루틴 => Use() 메인루틴 + Swing() 코루틴 (Co-op / Co-Routine)
    // >> 실행될 때 같이 실행 됨

    public Player player;
    public enum Type { Melee, Range, Special };
    public Type type;
    public int weaponindex;
    public enum Class { Normal, Rare, Epic, Legend };
    public Class weaponClass;
    public enum EffectType { Normal, Fire, Ice};
    public EffectType effectType;
    public enum AttackType { First, Second };
    public AttackType attackType;
    public int effectTime;
    public TrailRenderer meleeEffect;

    // public int itemCode;
    public int damage;                   // 데미지
    public float rate;                      // 공속
    public float reloadTime;            // 리로드 시간
    public int maxAmmo;               // 최대 탄창
    public int curAmmo;                 // 현재 탄창
    public BoxCollider meleeArea;    // 근접 공격 범위
    public int bulletSpeed;             // 원거리 공격 범위

    public int basicDamage; // 기초 데미지
    public float basicRate;    // 기초 공격 속도
    public float basicReloadTime;   // 기초 리로드 시간
    public int basicMaxAmmo;   // 기초 최대 탄창
    public Vector3 basicSize;  // 기초 근접 범위
    public Vector3 basicCenter;
    public Vector3 basicLocalScale;
    public Vector3 basicEffectScale;
    public int basicBulletSpeed;   // 기초 원거리 범위

    public float range;

    public TrailRenderer trailEffect;
    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;
    public AudioSource weaponSound;

    void Awake()
    {
        WeaponReload();
    }

    public void DataReload()
    {
        if (meleeArea != null)
        {
            basicSize = meleeArea.size;
            basicCenter = meleeArea.center;
        }
        basicLocalScale = gameObject.transform.localScale;
        if (meleeEffect != null)
            basicEffectScale = meleeEffect.transform.localScale;
    }

    public void Use()
    {
        if (type == Type.Melee)
        {
            switch (attackType)
            {
                case AttackType.First:
                    StopCoroutine(Swing());
                    StartCoroutine(Swing());
                    break;
                case AttackType.Second:
                    StopCoroutine(SideCut());
                    StartCoroutine(SideCut());
                    break;
            }
        }
        else if (type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine(Shot());
        }
    }

    public void WeaponReload()
    {
        float Tiar = weaponClass == Class.Normal ? 1 : weaponClass == Class.Rare ? 1.2f : weaponClass == Class.Epic ? 1.5f : 1.8f;
        damage = (int)((basicDamage * Tiar * (1 + (float)player.statusDamage / 100)));
        rate = ((basicRate * 100) / (1 + (float)player.statusRate / 100)) / 100;
        reloadTime = basicReloadTime / (1 + (float)player.statusReloadTime / 100);
        maxAmmo = (int)(basicMaxAmmo * (1 + (float)player.statusMaxAmmo / 100));
        range = (float)player.statusRange / 100;
        if (type == Type.Melee)
        {
            meleeArea.size = new Vector3(basicSize.x * (1 + range), basicSize.y * (1 + range), basicSize.z * (1 + range));
            meleeArea.center = new Vector3(basicCenter.x, basicCenter.y * (1 + range / 2), basicCenter.z);
            gameObject.transform.localScale = new Vector3(basicLocalScale.x * (1 + range), basicLocalScale.y * (1 + range), basicLocalScale.z * (1 + range));
        }
        bulletSpeed = (int)(basicBulletSpeed * (1 + range));
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f * rate);  // 0.1s 대기
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.2f * rate);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f * rate);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.2f * rate);
        trailEffect.enabled = false;

        yield break;
    }
    IEnumerator SideCut()
    {
        yield return new WaitForSeconds(0.2f * rate);
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.5f * rate);
        trailEffect.enabled = false;

        yield return new WaitForSeconds(0.1f * rate);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.1f * rate);

        yield break;
    }

    IEnumerator Shot()
    {
        // #. 총알 발사
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Bullet bulletData = intantBullet.GetComponent<Bullet>();
        bulletData.damage = damage;
        bulletData.effectType = effectType;
        bulletData.effectTime = effectTime;
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * bulletSpeed;

        yield return null;

        // #. 탄피 배출
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }
}
