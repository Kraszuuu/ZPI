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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire2") && Time.time >= _timeToFire)
        {
            _timeToFire = Time.time + 1/FireRate;
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        Ray ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            _destination = hit.point;
        else
            _destination = ray.GetPoint(1000);

        InstantiateProjectile(FirePoint);
    }

    void InstantiateProjectile(Transform firePoint)
    {
        var projectileObj = Instantiate(Projectile, firePoint.position, Quaternion.identity) as GameObject;
        projectileObj.GetComponent<Rigidbody>().velocity = (_destination - firePoint.position).normalized * ProjectileSpeed;
        iTween.PunchPosition(projectileObj, new Vector3(Random.Range(-ArcRange, ArcRange), Random.Range(-ArcRange, ArcRange), 0), Random.Range(0.5f, 2));
    }
}
