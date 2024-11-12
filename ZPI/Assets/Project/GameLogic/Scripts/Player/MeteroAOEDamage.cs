using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteroAOEDamage : MonoBehaviour
{
    public int damage = 1; // Obra¿enia zadawane przez meteoryt
    public float radius = 0.01f; // Promieñ obra¿eñ obszarowych
    private Vector3 hitPoint; // Punkt ataku, wokó³ którego zadajemy obra¿enia

    // Inicjalizacja meteoru z punktem ataku
    public void Initialize(Vector3 attackPoint)
    {
        hitPoint = attackPoint;
    }

    // Wykrycie kolizji cz¹stek z ziemi¹ lub innymi obiektami
    void OnParticleCollision(GameObject other)
    { 
        // Zadawanie obra¿eñ przeciwnikom w promieniu hitPoint
        Debug.Log("Object hit: " + other.name);
        Debug.Log("HitPoint: " + hitPoint.ToString());
        DealDamageToEnemiesInRadius(hitPoint);
    }

    // Funkcja, która zadaje obra¿enia wszystkim przeciwnikom w promieniu wokó³ hitPoint
    void DealDamageToEnemiesInRadius(Vector3 explosionCenter)
    {
        // ZnajdŸ wszystkie obiekty w promieniu od punktu ataku
        Collider[] hitColliders = Physics.OverlapSphere(explosionCenter, 0.01f);

        foreach (var hitCollider in hitColliders)
        {
            // SprawdŸ, czy obiekt jest przeciwnikiem
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Zadaj obra¿enia przeciwnikowi
                enemy.TakeDamage(1);
                Debug.Log("Przeciwnik " + enemy.name + " otrzyma³ obra¿enia: " + damage);
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        // Ustaw kolor sfery na pó³przezroczysty czerwony
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        // Narysuj sferê wokó³ explosionCenter o promieniu radius
        Gizmos.DrawWireSphere(hitPoint, 0.01f);
    }
}
