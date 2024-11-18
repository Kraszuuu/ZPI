using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryAttack : MonoBehaviour
{
    public Camera Camera;
    public GameObject Projectile;
    public Transform FirePoint;
    public SmoothFollowPoint SmoothFollowPoint;
    public float ProjectileSpeed = 30f;
    public float FireRate = 4;
    public float ArcRange = 1;
    [Range(0, 0.2f)] public float DelayedFire = 0f;

    private Vector3 _destination;
    private float _timeToFire;

    public void ShootProjectile()
    {
        if (Time.time >= _timeToFire && SmoothFollowPoint._isWandEquipped && !GameState.Instance.IsSpellCasting && !GameState.Instance.IsGamePaused && !GameState.Instance.IsGameOver)
        {
            AudioManager.instance.PlayStupefySound();

            _timeToFire = Time.time + 1 / FireRate;

            Ray ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            // Określenie celu na podstawie trafienia lub punktu przed graczem
            if (Physics.Raycast(ray, out RaycastHit hit))
                _destination = hit.point;
            else
                _destination = ray.GetPoint(1000);

            SmoothFollowPoint.TogglePrimaryAttack();

            Invoke(nameof(InstantiateProjectile), DelayedFire); // delikatne opóźnienie
        }
    }

    void InstantiateProjectile()
    {
        // Tworzenie pocisku dokładnie w FirePoint
        var projectileObj = Instantiate(Projectile, FirePoint.position, Quaternion.identity) as GameObject;

        // Ustawienie kierunku pocisku
        Vector3 direction = (_destination - FirePoint.position).normalized;
        projectileObj.GetComponent<Rigidbody>().velocity = direction * ProjectileSpeed;

        // Pobranie obrażeń z SpellManager i przypisanie do pocisku
        var projectileScript = projectileObj.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.DirectDamage = SpellManager.Instance.GetSpellData("PrimaryAttack");
            projectileScript.AreaDamage = SpellManager.Instance.GetSpellData("PrimaryAttackAreaDamage");
        }

        // Opcjonalny efekt dla pocisku
        iTween.PunchPosition(projectileObj, new Vector3(Random.Range(-ArcRange, ArcRange), Random.Range(-ArcRange, ArcRange), 0), Random.Range(0.5f, 2));
    }
}
