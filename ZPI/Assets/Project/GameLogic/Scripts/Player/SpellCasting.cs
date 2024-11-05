using DigitalRuby.PyroParticles;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.UI;

public class SpellCasting : MonoBehaviour
{
    public CameraShake cameraShake;
    private List<Vector3> mousePositions = new List<Vector3>();
    private GameFreezer gameFreezer;
    private FireBaseScript prefabScript;
    private GameObject fireball;
    public float minDistance = 20f; // Minimalna odleg�o�� mi�dzy punktami, aby unikn�� nadmiernej liczby punkt�w
    public GameObject fireballPrefab; // Prefab, kt�ry zawiera FireBaseScript
    public float spellCastDistance = 10f; // Odleg�o��, na jak� rzucane jest zakl�cie
    public GameObject lineRendererPrefab;
    private GameObject lineRendererInstance;
    private LineRenderer lineRenderer;
    public ParticleSystem spellCastingParticleSystem;
    private PlayerVoiceCommands playerVoiceCommands;

    private List<DollarPoint> _drawPoints = new List<DollarPoint>();
    public event Action<DollarPoint[]> OnDrawFinished;
    private RecognitionManager recognitionManager;

    [Header("--- Fireball ---")]
    public Image FireballImage;
    public float FireballCooldown = 0f;
    public bool IsFireballUnlocked;

    [Header("--- Extermination ---")]
    public Image ExterminationImage;
    public float ExterminationCooldown = 0f;
    public bool IsExterminationUnlocked = true;
    public GameObject meteorPrefab;

    [Header("--- Freeze ---")]
    public Image FreezingImage;
    public float FreezingCooldown = 0f;
    public bool IsFreezeUnlocked = false;

    [Header("--- Green ball ---")]
    public Image GreenBallImage;
    public float GreenballCooldown = 0f;
    public bool IsGreenballUnlocked = true;

    [Header("--- Lightning ---")]
    public Image LightningImage;
    public float LightningCooldown = 0f;
    public bool IsLightningUnlocked = true;

    private int _strokeIndex;

    public Vector3 boxHalfExtents = new Vector3(0.01f, 0.01f, 0.01f);

    private void Start()
    {
        gameFreezer = FindObjectOfType<GameFreezer>();
        lineRendererInstance = Instantiate(lineRendererPrefab);
        lineRenderer = lineRendererInstance.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        recognitionManager = new RecognitionManager();
        playerVoiceCommands = GetComponent<PlayerVoiceCommands>();
        spellCastingParticleSystem.Stop();
        lineRenderer.sortingOrder = 1;
        spellCastingParticleSystem.GetComponent<Renderer>().sortingOrder = 0;

        UnlockSpell("Fireball");
    }

    void Update()
    {
        if (GameState.Instance.IsGameOver || GameState.Instance.IsGamePaused || GameState.Instance.IsUpgrading) return;

        if (Input.GetMouseButton(0)) // Lewy przycisk myszy
        {
            if (!GameState.Instance.IsSpellCasting)
            {
                GameState.Instance.IsSpellCasting = true;
                gameFreezer.UpdateTimeScale();
                Cursor.lockState = CursorLockMode.Confined;
            }
            HandleMouseInput();
        }

        if (Input.GetMouseButtonUp(0))
        {
            FinalizeSpellCasting();
        }

        updateCooldowns();
    }

    void RecognizeSpell(string name, float distance)
    {
        // Przyk�ad: proste rozpoznawanie linii pionowej
        if (name != null || distance > 2f)
        {
            if (playerVoiceCommands.recognizedSpell != null)
            {
                Debug.LogError("Gratulacje, dziala, teraz ogarnac spelle, zle to rzucimy ify nizej i bedzie super");
            }

            if (name.Equals("I"))
            {
                if (FireballImage.fillAmount <= 0)
                {
                    CastFireSpellInDirection();
                    FireballImage.fillAmount = 1;
                    FireballCooldown = 3f;
                }
            }
            else if (name.Equals("Z"))
            {
                CastMeteorRain();
                CastSpell("Meteor Rain!!!");
            }
            Debug.Log("Spell casted! Number of points: " + mousePositions.Count);
        }
        else
        {
            Debug.Log("Unrecognized spell pattern");
        }
        Debug.Log("Spell casted! Number of points: " + mousePositions.Count);
        playerVoiceCommands.recognizedSpell = null;
    }

    public void CastMeteorRain()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Wykonanie BoxCast z praktycznie nieskończonym zasięgiem
        if (Physics.BoxCast(ray.origin, boxHalfExtents, ray.direction, out hit, Quaternion.identity, Mathf.Infinity))
        {
            DrawBoxCast(ray.origin, ray.direction);
            // Jeśli wykryto przeszkodę, możemy uzyskać szczegóły trafienia
            Debug.Log("Hit: " + hit.collider.name);
            SpawnMeteor(hit.point, hit.collider); // Materializowanie meteorytu na napotkanym obiekcie
        }
    }

    void DrawBoxCast(Vector3 position, Vector3 direction)
    {
        // Oblicz krawędzie boxa
        Vector3 frontBottomLeft = position + Quaternion.LookRotation(direction) * new Vector3(-boxHalfExtents.x, -boxHalfExtents.y, 0);
        Vector3 frontBottomRight = position + Quaternion.LookRotation(direction) * new Vector3(boxHalfExtents.x, -boxHalfExtents.y, 0);
        Vector3 frontTopLeft = position + Quaternion.LookRotation(direction) * new Vector3(-boxHalfExtents.x, boxHalfExtents.y, 0);
        Vector3 frontTopRight = position + Quaternion.LookRotation(direction) * new Vector3(boxHalfExtents.x, boxHalfExtents.y, 0);

        Vector3 backBottomLeft = frontBottomLeft + direction.normalized * 300f;
        Vector3 backBottomRight = frontBottomRight + direction.normalized * 300f;
        Vector3 backTopLeft = frontTopLeft + direction.normalized * 300f;
        Vector3 backTopRight = frontTopRight + direction.normalized * 300f;

        // Rysowanie krawędzi boxa
        Debug.DrawLine(frontBottomLeft, frontBottomRight, Color.red);
        Debug.DrawLine(frontBottomLeft, frontTopLeft, Color.red);
        Debug.DrawLine(frontBottomRight, frontTopRight, Color.red);
        Debug.DrawLine(backBottomLeft, backBottomRight, Color.red);
        Debug.DrawLine(backBottomLeft, backTopLeft, Color.red);
        Debug.DrawLine(backBottomRight, backTopRight, Color.red);
        Debug.DrawLine(frontBottomLeft, backBottomLeft, Color.red);
        Debug.DrawLine(frontBottomRight, backBottomRight, Color.red);
        Debug.DrawLine(frontTopLeft, backTopLeft, Color.red);
        Debug.DrawLine(frontTopRight, backTopRight, Color.red);
    }

    void SpawnMeteor(Vector3 hitPoint, Collider collider)
    {
        // Możesz ustawić spawnPosition na konkretnej wartości
        Vector3 spawnPosition = new Vector3(hitPoint.x, 0f, hitPoint.z);

        // Tworzenie instancji meteorytu
        GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);


    }

    public void CastFireSpellInDirection()
    {
        fireball = null;
        prefabScript = null;
        Vector3 pos;
        float yRot = transform.rotation.eulerAngles.y;
        Vector3 forwardY = Quaternion.Euler(0.0f, yRot, 0.0f) * Vector3.forward;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;
        Quaternion rotation = Quaternion.identity;

        cameraShake.Shake();

        // Instancjonowanie prefabrykatu ognistej kuli
        fireball = Instantiate(fireballPrefab);
        prefabScript = fireball.GetComponent<FireConstantBaseScript>();

        if (prefabScript == null)
        {
            // temporary effect, like a fireball
            prefabScript = fireball.GetComponent<FireBaseScript>();
            if (prefabScript.IsProjectile)
            {
                // set the start point near the player
                rotation = Camera.main.transform.rotation;
                pos = transform.position + forward + up;
            }
            else
            {
                // set the start point in front of the player a ways
                pos = transform.position + (forwardY * 10.0f);
            }
        }
        else
        {
            // set the start point in front of the player a ways, rotated the same way as the player
            pos = transform.position + (forwardY * 5.0f);
            rotation = transform.rotation;
            pos.y = 0.0f;
        }

        FireProjectileScript projectileScript = fireball.GetComponentInChildren<FireProjectileScript>();
        if (projectileScript != null)
        {
            // make sure we don't collide with other fire layers
            projectileScript.ProjectileCollisionLayers &= (~UnityEngine.LayerMask.NameToLayer("FireLayer"));
        }

        fireball.transform.position = pos;
        fireball.transform.rotation = rotation;
    }

    

    void CastSpell(string spellName)
    {
        Debug.Log("Casting spell: " + spellName);
        // Tu dodaj logik� odpowiedniego zakl�cia
    }

    private void HandleMouseInput()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0.8f; // Odległość od kamery (aby przekształcić do przestrzeni świata)
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        if (mousePositions.Count == 0 || Vector3.Distance(mousePositions[mousePositions.Count - 1], mousePos) > minDistance)
        {
            mousePositions.Add(mousePos);
            _drawPoints.Add(new DollarPoint() { Point = new Vector2(mousePos.x, mousePos.y), StrokeIndex = 1 });
            lineRenderer.positionCount = mousePositions.Count;
            lineRenderer.SetPosition(mousePositions.Count - 1, worldPos);
            // Przenoszenie Particle System na aktualną pozycję myszy
            spellCastingParticleSystem.transform.position = worldPos;

            if (!spellCastingParticleSystem.isPlaying)
            {
                spellCastingParticleSystem.Play();
            }
        }
    }

    private void FinalizeSpellCasting()
    {
        (string result, float points) = recognitionManager.OnDrawFinished(_drawPoints.ToArray());
        _drawPoints.Clear();
        GameState.Instance.IsSpellCasting = false;
        RecognizeSpell(result, points);
        mousePositions.Clear();
        lineRenderer.positionCount = 0;
        spellCastingParticleSystem.Stop();

        gameFreezer.UpdateTimeScale();
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void updateCooldowns()
    {
        if (FireballImage.fillAmount > 0 && IsFireballUnlocked)
        {
            FireballImage.fillAmount -= 1 / FireballCooldown * Time.deltaTime;
        }
        if (ExterminationCooldown > 0 && IsExterminationUnlocked)
        {
            FireballImage.fillAmount -= 1 / FireballCooldown * Time.deltaTime;
        }
        if (FreezingCooldown > 0 && IsFreezeUnlocked)
        {
            FireballImage.fillAmount -= 1 / FireballCooldown * Time.deltaTime;
        }
        if (GreenballCooldown > 0 && IsGreenballUnlocked)
        {
            FireballImage.fillAmount -= 1 / FireballCooldown * Time.deltaTime;
        }
        if (LightningCooldown > 0 && IsLightningUnlocked)
        {
            FireballImage.fillAmount -= 1 / FireballCooldown * Time.deltaTime;
        }
    }

    public void UnlockSpell(string spell)
    {
        if (spell.Equals("Fireball"))
        {
            FireballImage.fillAmount = 0;
            FireballImage.color = new Color(1f, 0.5f, 0f, 0.7f);
            IsFireballUnlocked = true;
        }
        if (spell.Equals("Extermination"))
        {
            ExterminationImage.fillAmount = 0;
            ExterminationImage.color = new Color(0.5f, 0f, 0.5f, 0.7f);
            IsExterminationUnlocked = true;
        }
        if (spell.Equals("Freezing"))
        {
            FreezingImage.fillAmount = 0;
            FreezingImage.color = new Color(0f, 0f, 0.55f, 0.7f);
            IsFreezeUnlocked = true;
        }
        if (spell.Equals("Greenball"))
        {
            GreenBallImage.fillAmount = 0;
            GreenBallImage.color = new Color(0f, 1f, 0f, 0.7f);
            IsGreenballUnlocked = true;
        }
        if (spell.Equals("Lightning"))
        {
            LightningImage.fillAmount = 0;
            LightningImage.color = new Color(0.68f, 0.85f, 0.9f, 0.7f);
            IsLightningUnlocked = true;
        }
    }











    //bool IsVerticalLine()
    //{
    //    // Proste sprawdzenie, czy �lad przypomina pionow� lini�
    //    if (mousePositions.Count < 2) return false;

    //    float previousX = Math.Abs(mousePositions[0].x);
    //    float threshold = 6f; // Tolerowanie odchylenie dla kolejnych punktow

    //    foreach (var point in mousePositions)
    //    {
    //        //Debug.Log("POPRZEDNI: " + previousX);
    //        //Debug.Log(point.x);
    //        if (Math.Abs(previousX - Math.Abs(point.x)) > threshold)
    //        {
    //            return false;
    //        }
    //        previousX = Math.Abs(point.x);
    //    }
    //    return true;
    //}

    //bool IsHorizontalLine()
    //{
    //    // Proste sprawdzenie, czy �lad przypomina pionow� lini�
    //    if (mousePositions.Count < 2) return false;

    //    float previousY = Math.Abs(mousePositions[0].y);
    //    float threshold = 6f; // Tolerowanie odchylenie dla kolejnych punktow

    //    foreach (var point in mousePositions)
    //    {
    //        if (Math.Abs(previousY - Math.Abs(point.y)) > threshold)
    //        {
    //            return false;
    //        }
    //        previousY = Math.Abs(point.y);
    //    }
    //    return true;
    //}

    //// Funkcja do por�wnywania kierunk�w z wzorcem "Z"
    //bool IsZShape()
    //{
    //    if (mousePositions.Count < 3) return false;

    //    // Analiza kierunk�w mi�dzy punktami
    //    List<Vector2> directions = new List<Vector2>();

    //    for (int i = 1; i < mousePositions.Count; i++)
    //    {
    //        Vector2 direction = (mousePositions[i] - mousePositions[i - 1]).normalized;
    //        directions.Add(direction);
    //    }

    //    // Sprawd�, czy ruch odpowiada wzorcowi litery Z
    //    bool firstLine = false;
    //    bool diagonalLine = false;
    //    bool secondLine = false;

    //    foreach (var dir in directions)
    //    {
    //        //Debug.Log(dir);
    //        //Debug.Log("WSPOLRZEDNA IKSOWA: " + dir.x);
    //        if (!firstLine && Mathf.Abs(dir.x) > Mathf.Abs(dir.y) && dir.x > 0) // Poziomy ruch w prawo
    //        {
    //            //Debug.Log("PIERWSZA LINIA");
    //            firstLine = true;
    //        }
    //        else if (firstLine && !diagonalLine && dir.x < 0 && dir.y < 0) // Uko�ny ruch w lewo-d�
    //        {
    //            //Debug.Log("DRUGA LINIA");
    //            diagonalLine = true;
    //        }
    //        else if (diagonalLine && !secondLine && Mathf.Abs(dir.x) > Mathf.Abs(dir.y) && dir.x > 0) // Drugi poziomy ruch w prawo
    //        {
    //            secondLine = true;
    //            break; // Wzorzec rozpoznany, mo�na zako�czy� p�tl�
    //        }
    //    }

    //    return firstLine && diagonalLine && secondLine;
    //}
}
