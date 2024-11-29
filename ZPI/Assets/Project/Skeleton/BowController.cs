using System.Collections;
using UnityEngine;

public class BowController : MonoBehaviour
{
    public GameObject arrowPrefab; // Prefab strzały
    public Transform arrowHolder; // Bone trzymający strzałę w dłoni
    // public Transform bowString;   // Bone cięciwy łuku
    public Transform firePoint;   // Miejsce, z którego strzała zostaje wystrzelona

    private GameObject currentArrow; // Strzała trzymana przez postać

    // Wywoływane na początku animacji wyciągania strzały
    public void DrawArrow()
    {
        if (currentArrow == null)
        {
            currentArrow = Instantiate(arrowPrefab, arrowHolder.position, arrowHolder.rotation, arrowHolder);
            // currentArrow.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        }
    }

    // Wywoływane w momencie wystrzału
    public void FireArrow()
    {
        if (currentArrow != null)
        {
            // Odłącz strzałę od dłoni
            currentArrow.transform.parent = null;
            Arrow arrowScript = currentArrow.GetComponent<Arrow>();
            if (arrowScript != null)
            {
                arrowScript.Initialize(gameObject);
                arrowScript.isHeld = false;
            }

            // Przenieś strzałę na pozycję wystrzału (opcjonalne)
            // currentArrow.transform.position = firePoint.position;
            // currentArrow.transform.rotation = firePoint.rotation;

            // Dodaj siłę do strzały
            Rigidbody arrowRb = currentArrow.GetComponent<Rigidbody>();
            if (arrowRb != null)
            {
                arrowRb.isKinematic = false;
                arrowRb.AddForce(firePoint.forward * 2000f); // Dopasuj siłę do potrzeb
            }

            Collider arrowCollider = currentArrow.GetComponent<Collider>();
            if (arrowCollider != null)
            {
                StartCoroutine(EnableColliderAfterDelay(arrowCollider, 0.05f));
            }

            // Usuń referencję do bieżącej strzały
            currentArrow = null;
        }
    }

    private IEnumerator EnableColliderAfterDelay(Collider collider, float delay)
    {
        yield return new WaitForSeconds(delay);
        collider.enabled = true; // Włącz collider po opóźnieniu
    }
}
