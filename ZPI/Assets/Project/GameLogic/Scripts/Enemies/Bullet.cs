using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletDamage = 10f;
    public bool isFriendly = false;

    private void OnCollisionEnter(Collision collision)
    {
        Transform hitTransform = collision.transform;
        if (isFriendly)
        {
            if (hitTransform.CompareTag("Enemy"))
            {
                hitTransform.GetComponent<Enemy>().TakeDamage((int)bulletDamage);
            }
        };
        if (hitTransform.CompareTag("Player"))
        {
            hitTransform.GetComponent<Health>().TakeDamage(bulletDamage);
        }
        Destroy(gameObject);
    }
}
