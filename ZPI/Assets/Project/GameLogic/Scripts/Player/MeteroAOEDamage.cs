using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteroAOEDamage : MonoBehaviour
{
    public int damage = 1; // Obra�enia zadawane przez meteoryt
    public float radius = 0.01f; // Promie� obra�e� obszarowych
    private Vector3 hitPoint; // Punkt ataku, wok� kt�rego zadajemy obra�enia

    // Inicjalizacja meteoru z punktem ataku
    public void Initialize(Vector3 attackPoint)
    {
        hitPoint = attackPoint;
    }

    // Wykrycie kolizji cz�stek z ziemi� lub innymi obiektami
    void OnParticleCollision(GameObject other)
    { 
        // Zadawanie obra�e� przeciwnikom w promieniu hitPoint
        Debug.Log("Object hit: " + other.name);
        Debug.Log("HitPoint: " + hitPoint.ToString());
        DealDamageToEnemiesInRadius(hitPoint);
    }

    // Funkcja, kt�ra zadaje obra�enia wszystkim przeciwnikom w promieniu wok� hitPoint
    void DealDamageToEnemiesInRadius(Vector3 explosionCenter)
    {
        // Znajd� wszystkie obiekty w promieniu od punktu ataku
        Collider[] hitColliders = Physics.OverlapSphere(explosionCenter, 0.01f);

        foreach (var hitCollider in hitColliders)
        {
            // Sprawd�, czy obiekt jest przeciwnikiem
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Zadaj obra�enia przeciwnikowi
                enemy.TakeDamage(1);
                Debug.Log("Przeciwnik " + enemy.name + " otrzyma� obra�enia: " + damage);
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        // Ustaw kolor sfery na p�przezroczysty czerwony
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        // Narysuj sfer� wok� explosionCenter o promieniu radius
        Gizmos.DrawWireSphere(hitPoint, 0.01f);
    }
}
