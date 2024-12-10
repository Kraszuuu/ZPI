using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteroAOEDamage : MonoBehaviour
{

    public float radius = 0.01f; // Promie� obra�e� obszarowych
    private Vector3 _hitPoint; // Punkt ataku, wok� kt�rego zadajemy obra�enia
    public GameObject meteorPrefab;
    public LayerMask Layer;
    private float _damageRadius = 5f;

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
        }
    }

    void SpawnMeteor(Vector3 hitPoint)
    {
        // Tworzenie instancji meteorytu
        GameObject meteor = Instantiate(meteorPrefab, new Vector3(hitPoint.x, 0.01f, hitPoint.z), Quaternion.identity);
        MeteorsPrefabScirpt meteroAOE = meteor.GetComponent<MeteorsPrefabScirpt>();
        meteroAOE.Initialize(hitPoint);


        //Audio
        AudioSource _audioSource = meteor.AddComponent<AudioSource>();
        _audioSource.spatialBlend = 1.0f;
        AudioManager.instance.PlayMeteorRainSound(_audioSource);

        Destroy(meteor, 4f);

    }
    void OnDrawGizmosSelected()
    {
        // Ustaw kolor sfery na p�przezroczysty czerwony
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        // Narysuj sfer� wok� explosionCenter o promieniu radius
        Gizmos.DrawWireSphere(_hitPoint, 0.01f);
    }
}

