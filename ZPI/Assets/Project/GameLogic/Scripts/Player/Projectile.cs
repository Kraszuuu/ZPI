using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool IsFriendly = false;

    [Header("Direct Damage Settings")]
    public float DirectDamage = 10f;

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

        if (hitTransform.CompareTag("Player"))
        {
            if (IsFriendly) return;
            else hitTransform.GetComponent<PlayerHealth>().TakeDamage(DirectDamage);
            // hitTransform.GetComponent<Enemy>().TakeDamage((int)SpellManager.Instance.GetSpellData("PrimaryAttack"));
        }
        else if (hitTransform.CompareTag("Enemy"))
        {
            hitTransform.GetComponent<Enemy>().TakeDamage((int)DirectDamage);
        }

        _collided = true;

        // Obrażenia obszarowe
        if (AreaDamage > 0)
        {
            LayerMask detectionLayer = LayerMask.GetMask("Enemy", "Player");
            LayerMask obstacleLayer = ~(1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Player"));
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, detectionLayer);

            Debug.Log("Number of hitColliders: " + hitColliders.Length);

            foreach (var hitCollider in hitColliders)
            {
                Vector3 directionToTarget = (hitCollider.transform.position - transform.position).normalized;
                float distanceToTarget = Vector3.Distance(transform.position, hitCollider.transform.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleLayer))
                {
                    // Jeśli nie ma przeszkód, zadaj obrażenia
                    if (IsFriendly && hitCollider.CompareTag("Enemy"))
                    {
                        hitCollider.GetComponent<Enemy>().TakeDamage((int)AreaDamage);
                        Debug.Log("Hit enemy at distance: " + distanceToTarget);
                    }
                    /*else if (!IsFriendly && hitCollider.CompareTag("Player"))
                    {
                        hitCollider.GetComponent<PlayerHealth>().TakeDamage(AreaDamage);
                        Debug.Log("Hit player at distance: " + distanceToTarget);
                    }*/
                }
                else
                {
                    Debug.Log("Target blocked by obstacle: " + hitCollider.name);
                }
            }
        }

        // Tworzenie efektu kolizji
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

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (ShowExplosionRadiusGizmo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
        }
    }
}
