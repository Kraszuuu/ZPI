using UnityEngine;

public class FireballScript : MonoBehaviour
{
    public Camera Camera;
    public GameObject FireballPrefab;
    public Transform FireballSpawnPoint;
    public float ProjectileSpeed = 30f;
    public float SpawnShake = 1;
    [Range(0, 0.2f)] public float DelayedFire = 0f;

    private CameraShake _cameraShake;
    private Vector3 _destination;

    private void Start()
    {

        _cameraShake = Camera.GetComponent<CameraShake>();
    }

    public void CastFireBallInDirection()
    {

        _cameraShake.Shake();

        Ray ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit))
            _destination = hit.point;
        else
            _destination = ray.GetPoint(1000);

        Invoke(nameof(InstantiateFireball), DelayedFire);
    }

    void InstantiateFireball()
    {
        // Tworzenie pocisku dokładnie w FirePoint
        var projectileObj = Instantiate(FireballPrefab, FireballSpawnPoint.position, Camera.main.transform.rotation);

        // Ustawienie kierunku pocisku
        Vector3 direction = (_destination - FireballSpawnPoint.position).normalized;
        projectileObj.GetComponent<Rigidbody>().velocity = direction * ProjectileSpeed;

        var projectileScript = projectileObj.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.DirectDamage = SpellManager.Instance.GetSpellData("Fireball");
            projectileScript.AreaDamage = SpellManager.Instance.GetSpellData("FireballAreaDamage");
        }

        // Opcjonalny efekt dla pocisku
        iTween.PunchPosition(projectileObj, new Vector3(Random.Range(-SpawnShake, SpawnShake), Random.Range(-SpawnShake, SpawnShake), 0), Random.Range(0.5f, 2));
    }
}
