using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };
    public Type type;
    public int value;

    Rigidbody rigid;
    SphereCollider sphereCollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        if (type == Type.Coin)
            StartCoroutine(getCoin());
        sphereCollider.enabled = false;

        Invoke(nameof(OnCollider), 0.5f);
    }

    void OnCollider()
    {
        sphereCollider.enabled = true;
    }

    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
    }

    IEnumerator getCoin()
    {
        rigid.AddForce(Vector3.up*20, ForceMode.Impulse);
        yield return new WaitForSeconds(2f);
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        Player player = playerObj.GetComponent<Player>();
        player.coin += value;
        Destroy(gameObject);
    }
}
