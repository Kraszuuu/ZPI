using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteroAOEDamage : MonoBehaviour
{
    public int damage = 1; // Obra�enia zadawane przez meteoryt
    public float radius = 0.01f; // Promie� obra�e� obszarowych
    private Vector3 _hitPoint; // Punkt ataku, wok� kt�rego zadajemy obra�enia
    public GameObject meteorPrefab;
    public LayerMask Layer;
    private float _damageRadius = 5f;

    // Inicjalizacja meteoru z punktem ataku
    public void Initialize(Vector3 attackPoint)
    {
        _hitPoint = attackPoint;
    }

    // Wykrycie kolizji cz�stek z ziemi� lub innymi obiektami
    void OnParticleCollision(GameObject other)
    {
        // Zadawanie obra�e� przeciwnikom w promieniu hitPoint
        Debug.Log("Object hit: " + other.name);
        Debug.Log("HitPoint: " + _hitPoint.ToString());
        DealDamageToEnemiesInRadius(_hitPoint);
    }

    // Funkcja, kt�ra zadaje obra�enia wszystkim przeciwnikom w promieniu wok� hitPoint
    void DealDamageToEnemiesInRadius(Vector3 explosionCenter)
    {
        // Znajd� wszystkie obiekty w promieniu od punktu ataku
        Collider[] hitColliders = Physics.OverlapSphere(explosionCenter, _damageRadius);

        foreach (var hitCollider in hitColliders)
        {
            // Sprawd�, czy obiekt jest przeciwnikiem
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Zadaj obra�enia przeciwnikowi
                enemy.TakeDamage((int)SpellManager.Instance.GetSpellData("Meteors"));
                Debug.Log("Przeciwnik " + enemy.name + " otrzyma� obra�enia: " + damage);
            }
        }
    }


    public void CastMeteorRain()
    {
        // Pozycja i kierunek patrzenia kamery
        Camera playerCamera = Camera.main; // Zak�adamy, �e g��wna kamera to kamera gracza
        Vector3 rayOrigin = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;

        Ray ray = new Ray(rayOrigin, rayDirection);
        RaycastHit hit;

        // Wykonanie BoxCast z praktycznie niesko�czonym zasi�giem

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, Layer))
        {
            SpawnMeteor(hit.point);
            Debug.Log("Hit: " + hit.collider.name);

        }
    }

    void SpawnMeteor(Vector3 hitPoint)
    {
        // Tworzenie instancji meteorytu
        GameObject meteor = Instantiate(meteorPrefab, new Vector3(hitPoint.x, 0.01f, hitPoint.z), Quaternion.identity);
        MeteroAOEDamage meteroAOE = meteor.transform.GetChild(0).GetComponent<MeteroAOEDamage>();
        meteroAOE.Initialize(hitPoint);

    }
    void OnDrawGizmosSelected()
    {
        // Ustaw kolor sfery na p�przezroczysty czerwony
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        // Narysuj sfer� wok� explosionCenter o promieniu radius
        Gizmos.DrawWireSphere(_hitPoint, 0.01f);
    }
}

