using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject mashObj;
    public GameObject effectObj;
    public Rigidbody rigid;
    public AudioSource bombSound;

    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(2f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        bombSound.Play();
        mashObj.SetActive(false);
        effectObj.SetActive(true);

        RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position,
                                        15, Vector3.up, 0f,
                                        LayerMask.GetMask("Enemy"));

        foreach (RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);
    }
}
