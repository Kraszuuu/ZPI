using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool IsFriendly = false;
    public bool IsCritical = false;

    [Header("Direct Damage Settings")]
    public float DirectDamage = 10f;
    public float ProjectileHitForce = 15f;

    [Header("Area Damage Settings")]
    public float AreaDamage = 0f;      // Obrażenia obszarowe
    public float ExplosionRadius = 5f; // Promień obrażeń obszarowych
    public bool ShowExplosionRadiusGizmo = false; // Opcja pokazywania strefy rażenia

    [Header("VFX Settings")]
    public GameObject ImpactVFX;
    public bool RetainEffectsAfterImpact = false;

    [Header("Scale Settings")]
    public bool ScaleWithTime = false;
    public float InitialScaleFactor = 1f;
    public float GrowthSpeed = 0f;

    private bool _collided;
    private Vector3 _targetScale;
    private float _scaleFactor;
    private readonly List<(Transform, ParticleSystem, float)> _childTransforms = new();

    void Start()
    {
        if (ScaleWithTime && !_collided)
        {
            _targetScale = transform.localScale;
            transform.localScale = _targetScale * InitialScaleFactor;
            _scaleFactor = InitialScaleFactor;

            foreach (Transform child in transform)
            {
                ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
                float initialParticleScale = particleSystem != null ? particleSystem.main.startSizeMultiplier : 1f;
                _childTransforms.Add((child, particleSystem, initialParticleScale));
            }
            ScaleChildren();
        }
    }

    void Update()
    {
        if (ScaleWithTime && !_collided)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, GrowthSpeed * Time.deltaTime);
            _scaleFactor = transform.localScale.x / _targetScale.x;
            ScaleChildren();
        }
    }

    private void ScaleChildren()
    {
        if (Mathf.Abs(_scaleFactor - 1f) < 0.001f) return;
        foreach (var (_, particleSystem, targetParticleScale) in _childTransforms)
        {
            if (particleSystem != null)
            {
                var mainModule = particleSystem.main;
                float calculatedParticleScale = _scaleFactor * targetParticleScale;
                mainModule.startSizeMultiplier = calculatedParticleScale;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_collided) return;

        Transform hitTransform = collision.transform;

        Vector3 hitForce = CalculateForceVector(hitTransform, collision.contacts[0].point);

        if (hitTransform.CompareTag("Player"))
        {
            hitTransform.GetComponent<PlayerHealth>().TakeDamage(DirectDamage);
        }
        else if (hitTransform.CompareTag("Enemy"))
        {
            hitTransform.GetComponent<Enemy>().TakeDamage((int)DirectDamage, hitForce, IsCritical);
        }

        _collided = true;

        HandleAreaDamage(collision);
        CreateImpactEffect(collision);
        Destroy(gameObject);
    }

    private void HandleAreaDamage(Collision collision)
    {
        // Obrażenia obszarowe
        if (AreaDamage > 0)
        {
            LayerMask detectionLayer = LayerMask.GetMask("Enemy", "Player");
            LayerMask obstacleLayer = ~LayerMask.GetMask("Enemy", "Player");
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, detectionLayer);

            foreach (var hitCollider in hitColliders)
            {
                Transform hitTransform = hitCollider.transform;

                // Wyklucz wroga, który otrzymał bezpośrednie obrażenia
                if (_collided && hitTransform == collision.transform)
                {
                    continue; // Pomijamy tego wroga
                }

                Vector3 hitForce = CalculateForceVector(hitTransform, transform.position);
                float distanceToTarget = Vector3.Distance(transform.position, hitTransform.position);

                // Sprawdzenie linii wzroku
                if (!Physics.Raycast(transform.position, hitForce, distanceToTarget, obstacleLayer))
                {
                    // Jeśli nie ma przeszkód, zadaj obrażenia
                    if (IsFriendly && hitCollider.CompareTag("Enemy"))
                    {
                        hitCollider.GetComponent<Enemy>().TakeDamage((int)AreaDamage, hitForce);
                    }
                }
            }
        }
    }

    private void CreateImpactEffect(Collision collision)
    {
        if (ImpactVFX != null)
        {
            var impact = Instantiate(ImpactVFX, collision.contacts[0].point, Quaternion.identity);
            if (ScaleWithTime)
            {
                impact.transform.localScale *= _scaleFactor;
                foreach (Transform child in impact.transform)
                {
                    if (child.TryGetComponent<ParticleSystem>(out var particleSystem))
                    {
                        var mainModule = particleSystem.main;
                        mainModule.startSizeMultiplier *= _scaleFactor;
                    }
                }
            }
            Destroy(impact, 4);
        }

        if (RetainEffectsAfterImpact)
        {
            foreach (var (child, particleSystem, _) in _childTransforms)
            {
                if (particleSystem != null)
                {
                    var mainModule = particleSystem.main;
                    mainModule.stopAction = ParticleSystemStopAction.Destroy;
                    particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    child.parent = null;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (ShowExplosionRadiusGizmo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
        }
    }

    private Vector3 CalculateForceVector(Transform hitTransform, Vector3 hitPoint)
    {
        Vector3 targetPoint = hitTransform.position;
        targetPoint.y = hitPoint.y;

        Vector3 hitDirection = (targetPoint - hitPoint).normalized;
        hitDirection.y = 0.9f;

        return hitDirection * ProjectileHitForce;
    }

    public void PlayFireballExplosionSound(AudioSource audioSource)
    {
        AudioManager.instance.PlayFireballExplosionSound(audioSource);
    }
}
