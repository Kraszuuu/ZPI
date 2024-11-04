using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryAttack : MonoBehaviour
{
    public Camera Camera;
    private Vector3 _destination;
    public GameObject Projectile;
    public Transform FirePoint;
    public float ProjectileSpeed = 30f;
    private float _timeToFire;
    public float FireRate = 4;
    public float ArcRange = 1;

    void Update()
    {
        if (Input.GetButton("Fire2") && Time.time >= _timeToFire)
        {
            _timeToFire = Time.time + 1 / FireRate;
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        Ray ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Określenie celu na podstawie trafienia lub punktu przed graczem
        if (Physics.Raycast(ray, out hit))
            _destination = hit.point;
        else
            _destination = ray.GetPoint(1000);

        InstantiateProjectile();
    }

    void InstantiateProjectile()
    {
        // Tworzenie pocisku dokładnie w FirePoint
        var projectileObj = Instantiate(Projectile, FirePoint.position, Quaternion.identity) as GameObject;

        // Ustawienie kierunku pocisku
        Vector3 direction = (_destination - FirePoint.position).normalized;
        projectileObj.GetComponent<Rigidbody>().velocity = direction * ProjectileSpeed;

        // Opcjonalny efekt dla pocisku
        iTween.PunchPosition(projectileObj, new Vector3(Random.Range(-ArcRange, ArcRange), Random.Range(-ArcRange, ArcRange), 0), Random.Range(0.5f, 2));
    }
}
