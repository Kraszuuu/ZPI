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
        }
    }

    // Wywoływane w momencie wystrzału
    public void FireArrow()
    {
        if (currentArrow != null)
        {
            // Odłącz strzałę od dłoni
            currentArrow.transform.parent = null;

            // Przenieś strzałę na pozycję wystrzału (opcjonalne)
            // currentArrow.transform.position = firePoint.position;
            // currentArrow.transform.rotation = firePoint.rotation;

            // Dodaj siłę do strzały
            Rigidbody arrowRb = currentArrow.GetComponent<Rigidbody>();
            if (arrowRb != null)
            {
                arrowRb.isKinematic = false;
                arrowRb.AddForce(firePoint.forward * 1000f); // Dopasuj siłę do potrzeb
            }

            // Usuń referencję do bieżącej strzały
            currentArrow = null;
        }
    }
}
