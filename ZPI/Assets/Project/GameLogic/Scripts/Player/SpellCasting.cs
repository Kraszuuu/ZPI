using DigitalRuby.PyroParticles;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class SpellCasting : MonoBehaviour
{
    public CameraShake cameraShake;
    private GameFreezer gameFreezer;
    private List<Vector3> mousePositions = new List<Vector3>();
    private InputManager inputManager;
    private FireBaseScript prefabScript;
    private GameObject fireball;
    public float minDistance = 20f; // Minimalna odleg�o�� mi�dzy punktami, aby unikn�� nadmiernej liczby punkt�w
    public GameObject fireballPrefab; // Prefab, kt�ry zawiera FireBaseScript
    public float spellCastDistance = 10f; // Odleg�o��, na jak� rzucane jest zakl�cie
    public LineRenderer lineRenderer;
    public ParticleSystem spellCastingParticleSystem;
    private PlayerVoiceCommands playerVoiceCommands;

    private List<DollarPoint> _drawPoints = new List<DollarPoint>();
    public event Action<DollarPoint[]> OnDrawFinished;
    private RecognitionManager recognitionManager;

    private int _strokeIndex;

    private void Start()
    {
        gameFreezer = FindObjectOfType<GameFreezer>();
        // Pobieramy referencj� do skryptu obracaj�cego kamer�
        inputManager = FindObjectOfType<InputManager>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        recognitionManager = new RecognitionManager();
        playerVoiceCommands = GetComponent<PlayerVoiceCommands>();
        spellCastingParticleSystem.Stop();
        lineRenderer.sortingOrder = 1;
        spellCastingParticleSystem.GetComponent<Renderer>().sortingOrder = 0;
    }

    void Update()
    {
        if (GameOverManager.isGameOver) return;

        // �ledzenie ruchu myszy, gdy przycisk jest wci�ni�ty
        if (Input.GetMouseButton(0)) // Lewy przycisk myszy
        {
            if (!inputManager.isCastSpelling)
            {
                inputManager.isCastSpelling = true;
                gameFreezer.SetIsCastSpelling(true);
                Cursor.lockState = CursorLockMode.Confined;
            }
            HandleMouseInput();
        }

        // Gdy przycisk myszy zostanie puszczony, analizujemy gest
        if (Input.GetMouseButtonUp(0))
        {
            FinalizeSpellCasting();
        }
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
                CastFireSpellInDirection();
            }
            else if (name.Equals("Z"))
            {
                CastSpell("IceBeam");
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
        inputManager.isCastSpelling = false;
        RecognizeSpell(result, points);
        mousePositions.Clear();
        lineRenderer.positionCount = 0;
        spellCastingParticleSystem.Stop();

        // Przywr�cenie normalnego up�ywu czasu
        gameFreezer.SetIsCastSpelling(false);
        Cursor.lockState = CursorLockMode.Locked;
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
