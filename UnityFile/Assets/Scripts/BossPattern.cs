using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossPattern : Enemy
{
    public enum Parttern { RockShot, HomingMissile, TornadoShot, WideArea, DiamondArea, 
        Quadrant, WideAreaTornadoXShot, WideAreaTornadoShot, WideAreaTornadoXFlipShot, 
        WideAreaTornadoGradationShot, Test };

    public Transform missilePortA;
    public Transform missilePortB;
    public bool isLook;
    public int test;

    protected IEnumerator PartternSelect(Parttern parttern, GameObject missile, int shotMissile, int maxMissile, float nextShotTime, float nextPartternTime, float speed)
    {
        yield return new WaitForSeconds(0.1f);

        rigid.velocity = Vector3.zero;
        switch (parttern)
        {
            case Parttern.RockShot:
                StartCoroutine(RockShot(missile, nextPartternTime));
                break;
            case Parttern.HomingMissile:
                StartCoroutine(HomingMissile(missile, nextPartternTime));
                break;
            case Parttern.TornadoShot:
                StartCoroutine(TornadoShot(missile, shotMissile, maxMissile, nextShotTime, nextPartternTime, speed));
                break;
            case Parttern.WideArea:
                StartCoroutine(WideArea(missile, shotMissile, maxMissile, nextShotTime, nextPartternTime, speed));
                break;
            case Parttern.DiamondArea:
                StartCoroutine(DiamondArea(missile, shotMissile, maxMissile, nextShotTime, nextPartternTime, speed));
                break;
            case Parttern.Quadrant:
                StartCoroutine(Quadrant(missile, shotMissile, maxMissile, nextShotTime, nextPartternTime, speed));
                break;
            case Parttern.WideAreaTornadoXShot:
                StartCoroutine(WideAreaTornadoXShot(missile, shotMissile, maxMissile, nextShotTime, nextPartternTime, speed));
                break;
            case Parttern.WideAreaTornadoShot:
                StartCoroutine(WideAreaTornadoShot(missile, shotMissile, maxMissile, nextShotTime, nextPartternTime, speed));
                break;
            case Parttern.WideAreaTornadoXFlipShot:
                StartCoroutine(WideAreaTornadoXFlipShot(missile, shotMissile, maxMissile, nextShotTime, nextPartternTime, speed));
                break;
            case Parttern.WideAreaTornadoGradationShot:
                StartCoroutine(WideAreaTornadoGradationShot(missile, shotMissile, maxMissile, nextShotTime, nextPartternTime, speed));
                break;
            case Parttern.Test:
                StartCoroutine(Test(missile, nextPartternTime));
                break;
        }
    }

    IEnumerator RockShot(GameObject rock, float nextPartternTime)
    {
        // 돌 뱉기 패턴
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(rock, transform.position, transform.rotation);

        yield return new WaitForSeconds(nextPartternTime);
        isLook = true;
    }

    IEnumerator HomingMissile(GameObject missile, float nextPartternTime)
    {
        // 호밍 미사일 패턴
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f); // 0.2s
        GameObject instantMissileA = Instantiate(missile, new Vector3(missilePortA.position.x, 0.5f, missilePortA.position.z), missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);    // 0.3s
        GameObject instantMissileB = Instantiate(missile, new Vector3(missilePortB.position.x, 0.5f, missilePortB.position.z), missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(nextPartternTime);
    }

    IEnumerator TornadoShot(GameObject objMissile, int shotCount, int missileCount, float nextShotTime, float nextPartternTime, float speed)
    {
        anim.SetTrigger("doBigShot");
        // 토네이도 미사일
        for (float turn = 0; turn < shotCount; turn++)
        {
            for (float index = 0; index < missileCount; index++)
            {
                GameObject instantMissile = Instantiate(objMissile, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
                Rigidbody rigid = instantMissile.GetComponent<Rigidbody>();

                // 2PIr = 원의 둘레
                Vector3 dirVec = new Vector3(Mathf.Cos(Mathf.PI * 2 * (index + turn / 10) / missileCount),
                                                        0,
                                                        Mathf.Sin(Mathf.PI * 2 * (index + turn / 10) / missileCount));
                rigid.AddForce(dirVec.normalized * speed, ForceMode.Impulse);

                Vector3 rotVec = Vector3.down * 360 * (index + turn / 10) / missileCount + Vector3.up * 90;
                instantMissile.transform.Rotate(rotVec);
            }
            yield return new WaitForSeconds(nextShotTime);
        }
        yield return new WaitForSeconds(nextPartternTime);
    }

    IEnumerator WideArea(GameObject objMissile, int shotCount, int missileCount, float nextShotTime, float nextPartternTime, float speed)
    {
        for (float turn = 0; turn < shotCount; turn++)
        {
            anim.SetTrigger("doTaunt");
            for (float index = 0; index < missileCount; index++)
            {
                GameObject instantMissile = Instantiate(objMissile, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
                Rigidbody rigid = instantMissile.GetComponent<Rigidbody>();
                Vector3 dirVec = new Vector3(Mathf.Cos(Mathf.PI * 2 * (index + (turn % 2) / 2) / missileCount),
                                                        0,
                                                        Mathf.Sin(Mathf.PI * 2 * (index + (turn % 2) / 2) / missileCount));
                rigid.AddForce(dirVec.normalized * speed, ForceMode.Impulse);

                Vector3 rotVec = Vector3.down * 360 * (index + (turn % 2) / 2) / missileCount + Vector3.up * 90;
                instantMissile.transform.Rotate(rotVec);
            }
            yield return new WaitForSeconds(nextShotTime);
        }

        yield return new WaitForSeconds(nextPartternTime);
    }

    IEnumerator DiamondArea(GameObject missile, int shotMissile, int maxMissile, float nextShotTime, float nextPartternTime, float speed) 
    { 
        for (float turn = 0; turn < shotMissile; turn++)
        {
            for (float index = 0; index < maxMissile; index++)
            {
                GameObject instantMissile = Instantiate(missile, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
                Rigidbody rigid = instantMissile.GetComponent<Rigidbody>();
                Vector3 dirVec = new Vector3(-Mathf.Cos(Mathf.PI * 0.5f * index / maxMissile),
                                                        0,
                                                        -Mathf.Sin(Mathf.PI * 0.5f * index / maxMissile));
                rigid.AddForce(dirVec.normalized * speed, ForceMode.Impulse);

                Vector3 rotVec = Vector3.down * 90 * index / maxMissile + Vector3.up * 270;
                instantMissile.transform.Rotate(rotVec);
            }

            for (float index = 0; index < maxMissile; index++)
            {
                GameObject instantMissile = Instantiate(missile, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
                Rigidbody rigid = instantMissile.GetComponent<Rigidbody>();
                Vector3 dirVec = new Vector3(Mathf.Cos(Mathf.PI * 0.5f * index / maxMissile),
                                                        0,
                                                        Mathf.Sin(Mathf.PI * 0.5f * index / maxMissile));
                rigid.AddForce(dirVec.normalized * 15, ForceMode.Impulse);

                Vector3 rotVec = Vector3.down * 90 * index / maxMissile + Vector3.up * 90;
                instantMissile.transform.Rotate(rotVec);
            }
            yield return new WaitForSeconds(nextShotTime);
        }

        yield return new WaitForSeconds(nextPartternTime);
    }

    IEnumerator Quadrant(GameObject missile, int shotMissile, int maxMissile, float nextShotTime, float nextPartternTime, float speed)
    {
        for (float turn = 0; turn < shotMissile; turn++)
        {
            for (float index = 0; index < maxMissile; index++)
            {
                if (index <= maxMissile / 4 || index > maxMissile / 2 && index <= maxMissile / 1.5f)
                    continue;

                GameObject instantMissile = Instantiate(missile, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
                Rigidbody rigid = instantMissile.GetComponent<Rigidbody>();
                Vector3 dirVec = new Vector3(Mathf.Cos(Mathf.PI * 2f * index / maxMissile),
                                                        0,
                                                        Mathf.Sin(Mathf.PI * 2f * index / maxMissile));
                rigid.AddForce(dirVec.normalized * speed, ForceMode.Impulse);

                Vector3 rotVec = Vector3.down * 360 * index / maxMissile + Vector3.up * 90;
                instantMissile.transform.Rotate(rotVec);
            }
            yield return new WaitForSeconds(nextShotTime);
        }

        yield return new WaitForSeconds(nextPartternTime);

        maxMissile /= 4;

        for (float turn = 0; turn < shotMissile; turn++)
        {
            for (float index = 0; index < maxMissile; index++)
            {
                GameObject instantMissile = Instantiate(missile, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
                Rigidbody rigid = instantMissile.GetComponent<Rigidbody>();
                Vector3 dirVec = new Vector3(-Mathf.Cos(Mathf.PI * 0.5f * index / maxMissile),
                                                        0,
                                                        -Mathf.Sin(Mathf.PI * 0.5f * index / maxMissile));
                rigid.AddForce(dirVec.normalized * speed, ForceMode.Impulse);

                Vector3 rotVec = Vector3.down * 90 * index / maxMissile + Vector3.up * 270;
                instantMissile.transform.Rotate(rotVec);
            }

            for (float index = 0; index < maxMissile; index++)
            {
                GameObject instantMissile = Instantiate(missile, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
                Rigidbody rigid = instantMissile.GetComponent<Rigidbody>();
                Vector3 dirVec = new Vector3(Mathf.Cos(Mathf.PI * 0.5f * index / maxMissile),
                                                        0,
                                                        Mathf.Sin(Mathf.PI * 0.5f * index / maxMissile));
                rigid.AddForce(dirVec.normalized * speed, ForceMode.Impulse);

                Vector3 rotVec = Vector3.down * 90 * index / maxMissile + Vector3.up * 90;
                instantMissile.transform.Rotate(rotVec);
            }
            yield return new WaitForSeconds(nextShotTime);
        }

        yield return new WaitForSeconds(nextPartternTime);
    }

    IEnumerator WideAreaTornadoXShot(GameObject missile, int shotMissile, int maxMissile, float nextShotTime, float nextPartternTime, float speed)
    {
        for (float turn = 0; turn < shotMissile; turn++)
        {
            anim.SetTrigger("doShot");
            for (int index = 0; index < maxMissile; index++)
            {
                GameObject instantMissile = Instantiate(missile, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
                BulletRotate bulletRotate = instantMissile.GetComponent<BulletRotate>();
                bulletRotate.dirVec = new Vector3(Mathf.Cos(Mathf.PI * 2 * index / maxMissile),
                                                        0,
                                                        Mathf.Sin(Mathf.PI * 2 * index / maxMissile));
                bulletRotate.target = transform;
                if (index % 2 == 0)
                    bulletRotate.isFlip = true;
                bulletRotate.speed = speed;

                Vector3 rotVec = Vector3.down * 360 * index / maxMissile + Vector3.up * 90;
                instantMissile.transform.Rotate(rotVec);
            }

            yield return new WaitForSeconds(nextShotTime);
        }

        yield return new WaitForSeconds(nextPartternTime);
    }

    IEnumerator WideAreaTornadoShot(GameObject missile, int shotMissile, int maxMissile, float nextShotTime, float nextPartternTime, float speed)
    {
        for (float turn = 0; turn < shotMissile; turn++)
        {
            anim.SetTrigger("doShot");
            for (int index = 0; index < maxMissile; index++)
            {
                GameObject instantMissile = Instantiate(missile, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
                BulletRotate bulletRotate = instantMissile.GetComponent<BulletRotate>();
                bulletRotate.dirVec = new Vector3(Mathf.Cos(Mathf.PI * 2 * index / maxMissile),
                                                        0,
                                                        Mathf.Sin(Mathf.PI * 2 * index / maxMissile));
                bulletRotate.target = transform;
                bulletRotate.speed = speed;

                Vector3 rotVec = Vector3.down * 360 * index / maxMissile + Vector3.up * 90;
                instantMissile.transform.Rotate(rotVec);
            }

            yield return new WaitForSeconds(nextShotTime);
        }

        yield return new WaitForSeconds(nextPartternTime);
    }

    IEnumerator WideAreaTornadoXFlipShot(GameObject missile, int shotMissile, int maxMissile, float nextShotTime, float nextPartternTime, float speed)
    {
        for (float turn = 0; turn < shotMissile; turn++)
        {
            anim.SetTrigger("doShot");
            for (int index = 0; index < maxMissile; index++)
            {
                GameObject instantMissile = Instantiate(missile, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
                BulletRotate bulletRotate = instantMissile.GetComponent<BulletRotate>();
                bulletRotate.dirVec = new Vector3(Mathf.Cos(Mathf.PI * 2 * index / maxMissile),
                                                        0,
                                                        Mathf.Sin(Mathf.PI * 2 * index / maxMissile));
                bulletRotate.target = transform;
                bulletRotate.isFlip = true;
                bulletRotate.speed = speed;

                Vector3 rotVec = Vector3.down * 360 * index / maxMissile + Vector3.up * 90;
                instantMissile.transform.Rotate(rotVec);
            }

            yield return new WaitForSeconds(nextShotTime);
        }

        yield return new WaitForSeconds(nextPartternTime);
    }

    IEnumerator WideAreaTornadoGradationShot(GameObject missile, int shotMissile, int maxMissile, float nextShotTime, float nextPartternTime, float speed)
    {
        for (float turn = 0; turn < shotMissile; turn++)
        {
            anim.SetTrigger("doTaunt");
            for (int index = 0; index < maxMissile; index++)
            {
                GameObject instantMissile = Instantiate(missile, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
                BulletRotate bulletRotate = instantMissile.GetComponent<BulletRotate>();
                bulletRotate.dirVec = new Vector3(Mathf.Cos(Mathf.PI * 2 * (index + (turn % 2) / 2) / maxMissile),
                                                        0,
                                                        Mathf.Sin(Mathf.PI * 2 * (index + (turn % 2) / 2) / maxMissile));
                bulletRotate.target = transform;
                bulletRotate.speed = speed * (1 - turn * 0.1f);
                if (turn % 2 == 1)
                    bulletRotate.isFlip = true;

                Vector3 rotVec = Vector3.down * 360 * (index + (turn % 2) / 2) / maxMissile + Vector3.up * 90;
                instantMissile.transform.Rotate(rotVec);
            }

            yield return new WaitForSeconds(nextShotTime);
        }

        yield return new WaitForSeconds(nextPartternTime);
    }

    protected IEnumerator Test(GameObject missile, float nextPartternTime)
    {
        int maxMissile = 30;

        for (int index = 0; index < maxMissile; index++)
        {
            GameObject instantMissile = Instantiate(missile, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
            BulletRotate bulletRotate = instantMissile.GetComponent<BulletRotate>();
            bulletRotate.dirVec = new Vector3(Mathf.Cos(Mathf.PI * 2 * index / maxMissile),
                                                    0,
                                                    Mathf.Sin(Mathf.PI * 2 * index / maxMissile));
            bulletRotate.target = transform;
            if (index % 2 == 0)
                bulletRotate.isFlip = true;

            Vector3 rotVec = Vector3.down * 360 * index / maxMissile + Vector3.up * 90;
            instantMissile.transform.Rotate(rotVec);
        }

        yield return new WaitForSeconds(nextPartternTime);
    }
}
