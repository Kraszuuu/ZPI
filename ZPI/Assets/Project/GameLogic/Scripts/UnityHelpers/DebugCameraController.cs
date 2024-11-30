using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;       // Prędkość ruchu kamery
    public float rotationSpeed = 150f; // Prędkość obracania kamery
    public Transform target;           // Obiekt do śledzenia
    public Vector3 offset = new Vector3(0, 5, -10); // Offset względem obiektu
    public float followSpeed = 5f;     // Prędkość płynnego przejścia
    private bool isFollowing = false;  // Czy kamera ma śledzić obiekt

    void Update()
    {
        // Przełączanie śledzenia obiektu klawiszem E
        if (Input.GetKeyDown(KeyCode.E))
        {
            isFollowing = !isFollowing;
        }

        // Ruch kamery (gdy nie śledzi obiektu)
        if (!isFollowing)
        {
            HandleMovement();
        }
        else
        {
            FollowTargetWithRotation(); // Śledzenie obiektu z przypięciem do rotacji
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // Klawisze A i D
        float vertical = Input.GetAxis("Vertical");     // Klawisze W i S

        Vector3 movement = new Vector3(horizontal, 0, vertical);
        movement = transform.TransformDirection(movement); // Przesuwanie względem orientacji kamery
        transform.position += movement * moveSpeed * Time.deltaTime;

        // Obrót kamery
        if (Input.GetMouseButton(1)) // Prawy przycisk myszy
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Obrót w poziomie (lewo-prawo)
            transform.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime, Space.World);

            // Obrót w pionie (góra-dół)
            transform.Rotate(Vector3.right, -mouseY * rotationSpeed * Time.deltaTime);
        }
    }

    void FollowTargetWithRotation()
    {
        if (target == null) return;

        // Oblicz docelową pozycję z offsetem względem obiektu
        Vector3 targetPosition = target.TransformPoint(offset);

        // Płynne przejście do pozycji obiektu
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Płynne przejście do rotacji obiektu
        Quaternion targetRotation = target.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, followSpeed * Time.deltaTime);
    }
}
