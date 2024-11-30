using System.Collections;
using UnityEngine;

public class BowController : MonoBehaviour
{
    public GameObject arrowPrefab; // Prefab strzały
    public Transform arrowHolder; // Bone trzymający strzałę w dłoni
    // public Transform bowString;   // Bone cięciwy łuku
    public Transform firePoint;   // Miejsce, z którego strzała zostaje wystrzelona
    public float force = 500f;

    private GameObject currentArrow; // Strzała trzymana przez postać

    // Wywoływane na początku animacji wyciągania strzały
    public void DrawArrow()
    {
        if (currentArrow == null)
        {
            currentArrow = Instantiate(arrowPrefab, arrowHolder.position, arrowHolder.rotation, arrowHolder);
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

            currentArrow.transform.rotation = transform.rotation;

            // Dodaj siłę do strzały
            Rigidbody arrowRb = currentArrow.GetComponent<Rigidbody>();
            if (arrowRb != null)
            {
                arrowRb.isKinematic = false;
                arrowRb.AddForce(currentArrow.transform.forward * force); // Dopasuj siłę do potrzeb
            }

            Collider arrowCollider = currentArrow.GetComponent<Collider>();
            if (arrowCollider != null)
            {
                arrowCollider.enabled = true;
            }

            // Usuń referencję do bieżącej strzały
            currentArrow = null;
        }
    }
}
