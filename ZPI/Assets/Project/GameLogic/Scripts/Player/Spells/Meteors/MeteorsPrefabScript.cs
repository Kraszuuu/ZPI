using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorsPrefabScirpt : MonoBehaviour
{
    public float radius = 0.01f; // Promie� obra�e� obszarowych
    private Vector3 _hitPoint; // Punkt ataku, wok� kt�rego zadajemy obra�enia
    private float _damageRadius = 5f;

    public void Initialize(Vector3 attackPoint)
    {
        _hitPoint = attackPoint;
    }

    void OnParticleCollision(GameObject other)
    {
        DealDamageToEnemiesInRadius(_hitPoint);
    }


    void DealDamageToEnemiesInRadius(Vector3 explosionCenter)
    {

        Collider[] hitColliders = Physics.OverlapSphere(explosionCenter, _damageRadius);

        foreach (var hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)SpellManager.Instance.GetSpellData("Meteors"), Vector3.down * 10f);
            }
        }
    }
}
