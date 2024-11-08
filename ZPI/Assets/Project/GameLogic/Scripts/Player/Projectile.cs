using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private bool _collided;
    public GameObject ImpactVFX;
    public bool universalBulletVfx = false;

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Bullet") && !collision.gameObject.CompareTag("Player") && !_collided)
        {
            _collided = true;
            var impact = Instantiate(ImpactVFX, collision.contacts[0].point, Quaternion.identity) as GameObject;

            if (universalBulletVfx)
            {
                foreach (Transform child in transform)
                {
                    child.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    Destroy(child.gameObject, 5f);
                    child.transform.parent = null; // Odłączenie dziecka od sfery
                }

            }

            Destroy(impact, 2);
            Destroy(gameObject);
        }
    }
}
