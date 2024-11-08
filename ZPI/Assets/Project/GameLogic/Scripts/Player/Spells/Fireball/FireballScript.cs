using DigitalRuby.PyroParticles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballScript : MonoBehaviour
{
    public GameObject fireballPrefab;
    public CameraShake cameraShake;
    public Transform fireballSpawnPoint;
    private GameObject fireball;


    public void CastFireBallInDirection()
    {
        fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Camera.main.transform.rotation);

        cameraShake.Shake();
        FireProjectileScript projectileScript = fireball.GetComponentInChildren<FireProjectileScript>();
        if (projectileScript != null)
        {
            int fireLayerMask = LayerMask.GetMask("FireLayer");
            int handsLayerMask = LayerMask.GetMask("Hands");
            int playerLayerMask = LayerMask.GetMask("Player");

            projectileScript.ProjectileCollisionLayers &= ~(fireLayerMask | handsLayerMask | playerLayerMask);

        }
    }
}
