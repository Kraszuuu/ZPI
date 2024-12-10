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
    [Range(0, 1f)] public float CriticalChance = 0.1f;

    // Lista systemów cząsteczek dla efektów
    public List<ParticleSystem> ParticleSystems;

    // Lista systemów trail z czasem działania
    [System.Serializable]
    public struct TrailParticle
    {
        public ParticleSystem TrailSystem;
        public float EmitTime;
    }
    public List<TrailParticle> TrailParticleSystems;

    private Vector3 _destination;
    private float _timeToFire;

    public void ShootProjectile()
    {
        if (Time.time >= _timeToFire && GameState.Instance.IsWandEquipped && !GameState.Instance.IsSpellCasting && !GameState.Instance.IsGamePaused && !GameState.Instance.IsGameOver && GameState.Instance.IsPrimaryAttackEnabled)
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
        var projectileObj = Instantiate(Projectile, FirePoint.position, Quaternion.identity);

        // Wywołanie efektu cząsteczek
        PlaySpellEffects();

        // Ustawienie kierunku pocisku
        Vector3 direction = (_destination - FirePoint.position).normalized;
        projectileObj.GetComponent<Rigidbody>().velocity = direction * ProjectileSpeed;

        // Pobranie obrażeń z SpellManager i przypisanie do pocisku
        var projectileScript = projectileObj.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.DirectDamage = SpellManager.Instance.GetSpellData("PrimaryAttack");
            projectileScript.AreaDamage = SpellManager.Instance.GetSpellData("PrimaryAttackAreaDamage");
            bool isCritical = Random.value <= CriticalChance;
            projectileScript.IsCritical = isCritical;
        }

        // Opcjonalny efekt dla pocisku
        iTween.PunchPosition(projectileObj, new Vector3(Random.Range(-ArcRange, ArcRange), Random.Range(-ArcRange, ArcRange), 0), Random.Range(0.5f, 2));
    }

    void PlaySpellEffects()
    {
        // Uruchomienie zwykłych systemów cząsteczek
        foreach (var particleSystem in ParticleSystems)
        {
            if (particleSystem != null)
            {
                if (particleSystem.isPlaying)
                {
                    particleSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                    particleSystem.Play();
                }
                else
                {
                    particleSystem.Play();
                }
            }
        }

        // Uruchomienie systemów trail z czasem emitowania
        foreach (var trail in TrailParticleSystems)
        {
            if (trail.TrailSystem != null)
            {
                StartCoroutine(EmitTrailParticles(trail.TrailSystem, trail.EmitTime));
            }
        }
    }

    private IEnumerator EmitTrailParticles(ParticleSystem trailSystem, float duration)
    {
        if (trailSystem.isPlaying)
        {
            trailSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }

        trailSystem.Play();

        yield return new WaitForSeconds(duration);

        // Po czasie wyłącz emisję, ale pozostaw istniejące cząsteczki
        trailSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
    }

    public void StopSpellEffects()
    {
        // Zatrzymanie zwykłych systemów cząsteczek
        foreach (var particleSystem in ParticleSystems)
        {
            if (particleSystem != null && particleSystem.isPlaying)
            {
                particleSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        // Zatrzymanie systemów trail
        foreach (var trail in TrailParticleSystems)
        {
            if (trail.TrailSystem != null && trail.TrailSystem.isPlaying)
            {
                trail.TrailSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }
}
