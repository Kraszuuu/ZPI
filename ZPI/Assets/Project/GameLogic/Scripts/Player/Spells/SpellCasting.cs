using DigitalRuby.PyroParticles;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.UI;
using Unity.VisualScripting;

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
    public GameObject lineRendererPrefab;
    private FireballScript fireballScript; 
    private GameObject lineRendererInstance;
    private LineRenderer lineRenderer;
    public ParticleSystem spellCastingParticleSystem;
    private PlayerVoiceCommands _playerVoiceCommands;

    private List<DollarPoint> _drawPoints = new List<DollarPoint>();
    public event Action<DollarPoint[]> OnDrawFinished;
    private RecognitionManager recognitionManager;


    [Header("--- Fireball ---")]
    public Image FireballImage;
    public float FireballCooldown = 0f;
    public bool IsFireballUnlocked;
    private FireballScript _fireballScript;

    [Header("--- Meteors ---")]
    public Image MeteorsImage;
    public float MeteorsCooldown = 0f;
    public bool IsMeteorsUnlocked = true;
    private MeteroAOEDamage _meteorsScript;

    [Header("--- Shield ---")]
    public Image ShieldImage;
    public float ShieldCooldown = 0f;
    public bool IsShieldUnlocked = false;
    private Shield _shieldScript;

    [Header("--- Lightning ---")]
    public Image LightningImage;
    public float LightningCooldown = 0f;
    public bool IsLightningUnlocked = true;
    private ChainLightningShoot _chainLightningShootScript;

    private int _strokeIndex;

    private AudioManager _audioManager;

    public Vector3 boxHalfExtents = new Vector3(0.001f, 0.001f, 0.001f);
    public LayerMask Layer;

    private void Start()
    {

        _meteorsScript = GetComponent<MeteroAOEDamage>();
        _fireballScript = GetComponent<FireballScript>();
        _shieldScript = GetComponent<Shield>();
        _chainLightningShootScript = GetComponent<ChainLightningShoot>();

        gameFreezer = FindObjectOfType<GameFreezer>();
        // Pobieramy referencj� do skryptu obracaj�cego kamer�
        inputManager = FindObjectOfType<InputManager>();
        lineRendererInstance = Instantiate(lineRendererPrefab);
        lineRenderer = lineRendererInstance.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        recognitionManager = new RecognitionManager();
        _playerVoiceCommands = GetComponent<PlayerVoiceCommands>();
        spellCastingParticleSystem.Stop();
        lineRenderer.sortingOrder = 1;
        spellCastingParticleSystem.GetComponent<Renderer>().sortingOrder = 0;
        _audioManager = GetComponent<AudioManager>();

        UnlockSpell("Fireball");


    }

    void Update()
    {
        if (GameState.Instance.IsGameOver || GameState.Instance.IsGamePaused || GameState.Instance.IsUpgrading) return;
        float rayLength = 1000f;
        Vector3 origin = transform.position;
        // Kierunek Raycasta (np. w przód od obiektu)
        Vector3 direction = transform.forward;

        // Zmienna, w której zapisujemy trafienie
        RaycastHit hit;

        // Wykonujemy Raycast
        if (Physics.Raycast(origin, direction, out hit, rayLength))
        {
            // Jeśli trafiono obiekt, rysujemy linię do punktu trafienia
            Debug.DrawLine(origin, hit.point, Color.red);

            // Opcjonalnie: wyświetl trafiony obiekt w logach
           // Debug.Log("Hit object: " + hit.collider.name);
        }
        else
        {
            // Jeśli Raycast nie trafił, rysujemy linię do maksymalnej odległości
            Debug.DrawLine(origin, origin + direction * rayLength, Color.green);
        }

        if (GameState.Instance.IsGameOver || GameState.Instance.IsGamePaused) return;

        // �ledzenie ruchu myszy, gdy przycisk jest wci�ni�ty
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

        // Gdy przycisk myszy zostanie puszczony, analizujemy gest
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
            if (_playerVoiceCommands.recognizedSpell != null)
            {
                Debug.LogError("Gratulacje, dziala, teraz ogarnac spelle, zle to rzucimy ify nizej i bedzie super");
            }

            if (name.Equals("I"))
            {
                if (FireballImage.fillAmount <= 0)
                {
                    CastFireball();
                    FireballImage.fillAmount = 1;
                    FireballCooldown = 3f;
                }
            }
            else if (name.Equals("Z"))
            {
                if (MeteorsImage.fillAmount <= 0)
                {
                    CastMeteorRain();
                    MeteorsImage.fillAmount = 1;
                    MeteorsCooldown = 5f;
                }
            }
            else if (name.Equals("Shield"))
            {
                if (MeteorsImage.fillAmount <= 0)
                {
                    //_shieldScript.;
                    ShieldImage.fillAmount = 1;
                    ShieldCooldown = 5f;
                }
            }
            else if (name.Equals("Lightning"))
            {
                if (MeteorsImage.fillAmount <= 0)
                {
                    //_chainLightningShootScript.StartShooting();
                    LightningImage.fillAmount = 1;
                    LightningCooldown = 5f;
                }
            }
            Debug.Log("Spell casted! Number of points: " + mousePositions.Count);
        }
        else
        {
            Debug.Log("Unrecognized spell pattern");
        }
        Debug.Log("Spell casted! Number of points: " + mousePositions.Count);
        _playerVoiceCommands.recognizedSpell = null;
    }
 
    public void CastFireball()
    {
        _fireballScript.CastFireBallInDirection();
    }

    public void CastMeteorRain()
    {
        _meteorsScript.CastMeteorRain();
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
        RecognizeSpell(result, points);
        mousePositions.Clear();
        lineRenderer.positionCount = 0;
        spellCastingParticleSystem.Stop();

        GameState.Instance.IsSpellCasting = false;
        gameFreezer.UpdateTimeScale();
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void updateCooldowns()
    {
        if (FireballImage.fillAmount > 0 && IsFireballUnlocked)
        {
            FireballImage.fillAmount -= 1 / FireballCooldown * Time.deltaTime;
        }
        if (MeteorsCooldown > 0 && IsMeteorsUnlocked)
        {
            MeteorsImage.fillAmount -= 1 / MeteorsCooldown * Time.deltaTime;
        }
        if (ShieldCooldown > 0 && IsShieldUnlocked)
        {
            ShieldImage.fillAmount -= 1 / ShieldCooldown* Time.deltaTime;
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
        if (spell.Equals("Meteros"))
        {
            MeteorsImage.fillAmount = 0;
            MeteorsImage.color = new Color(0.5f, 0f, 0.5f, 0.7f);
            IsMeteorsUnlocked = true;
        }
        if (spell.Equals("Shield"))
        {
            ShieldImage.fillAmount = 0;
            ShieldImage.color = new Color(0f, 0f, 0.55f, 0.7f);
            IsShieldUnlocked = true;
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
