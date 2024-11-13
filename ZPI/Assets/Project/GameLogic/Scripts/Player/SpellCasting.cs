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
    
    public float minDistance = 20f; // Minimalna odleg�o�� mi�dzy punktami, aby unikn�� nadmiernej liczby punkt�w
    public GameObject fireballPrefab; // Prefab, kt�ry zawiera FireBaseScript
    public float spellCastDistance = 10f; // Odleg�o��, na jak� rzucane jest zakl�cie
    public GameObject lineRendererPrefab;
    private List<GameObject> lineRendererInstances = new List<GameObject>();
    private LineRenderer currentLineRenderer;
    
    public ParticleSystem spellCastingParticleSystem;
    private PlayerVoiceCommands playerVoiceCommands;

    private List<DollarPoint> _drawPoints = new List<DollarPoint>();
    public event Action<DollarPoint[]> OnDrawFinished;
    private RecognitionManager recognitionManager;


    [Header("--- Fireball ---")]
    public Image FireballImage;
    public float FireballCooldown = 0f;
    public bool IsFireballUnlocked;
    private FireballScript fireballScript;

    [Header("--- Extermination ---")]
    public Image ExterminationImage;
    public float ExterminationCooldown = 0f;
    public bool IsExterminationUnlocked = true;
    public MeteroAOEDamage meteorsScript;

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

    private int _strokeIndex = 0;

    public Vector3 boxHalfExtents = new Vector3(0.001f, 0.001f, 0.001f);
    public LayerMask Layer;

    private void Start()
    {

        meteorsScript = GetComponent<MeteroAOEDamage>();
        fireballScript = GetComponent<FireballScript>();
        gameFreezer = FindObjectOfType<GameFreezer>();
        // Pobieramy referencj� do skryptu obracaj�cego kamer�
        inputManager = FindObjectOfType<InputManager>();
        recognitionManager = new RecognitionManager();
        playerVoiceCommands = GetComponent<PlayerVoiceCommands>();
        

        UnlockSpell("Fireball");
        UnlockSpell("Extermination");


    }

    void Update()
    {
        if (GameState.Instance.IsGameOver || GameState.Instance.IsGamePaused || GameState.Instance.IsUpgrading) return;
       
        if (GameOverManager.isGameOver) return;

        // �ledzenie ruchu myszy, gdy przycisk jest wci�ni�ty
        if (Input.GetMouseButton(1)) // Prawy przycisk myszy
        {
            if (!inputManager.isCastSpelling)
            {
                inputManager.isCastSpelling = true;
                gameFreezer.SetIsCastSpelling(true);
                Cursor.lockState = CursorLockMode.Confined;
            }
            if (Input.GetMouseButtonDown(0))
            {
                StartNewStroke();
            }
            if (Input.GetMouseButton(0))
            {
                HandleMouseInput();
                
            }
            
            
        }

        // Gdy przycisk myszy zostanie puszczony, analizujemy gest
        if (Input.GetMouseButtonUp(1))
        {
            FinalizeSpellCasting();
            _strokeIndex = 0;
        }

        updateCooldowns();
    }

    private void StartNewStroke()
    {
        _strokeIndex++;
        mousePositions.Clear(); // Czyścimy punkty dla nowej linii

        // Tworzymy nowy LineRenderer dla nowego gestu
        CreateNewLineRenderer();
        Debug.Log("New Stroke index: " + _strokeIndex);
    }

    private void CreateNewLineRenderer()
    {
        spellCastingParticleSystem.Stop();
        spellCastingParticleSystem.GetComponent<Renderer>().sortingOrder = 0;
        spellCastingParticleSystem.Play();
        // Usunięcie poprzedniego LineRenderer, aby nie zaśmiecać sceny
        GameObject newLineRendererInstance = Instantiate(lineRendererPrefab);
        lineRendererInstances.Add(newLineRendererInstance);

        currentLineRenderer = newLineRendererInstance.GetComponent<LineRenderer>();
        currentLineRenderer.positionCount = 0;
        currentLineRenderer.sortingOrder = 1;
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

            if (name.Equals("Fireball"))
            {
                if (FireballImage.fillAmount <= 0)
                {
                    CastFireball();
                    FireballImage.fillAmount = 1;
                    FireballCooldown = 3f;
                }
            }
            else if (name.Equals("Meteors"))
            {
                if (ExterminationImage.fillAmount <= 0)
                {
                    meteorsScript.CastMeteorRain();
                    ExterminationImage.fillAmount = 1;
                    ExterminationCooldown = 5f;
                }
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

    void CastSpell(string spellName)
    {
        Debug.Log("Casting spell: " + spellName);
        // Tu dodaj logik� odpowiedniego zakl�cia
    }

    public void CastFireball()
    {
        fireballScript.CastFireBallInDirection();
    }

    private void HandleMouseInput()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0.8f; // Odległość od kamery (aby przekształcić do przestrzeni świata)
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        if (mousePositions.Count == 0 || Vector3.Distance(mousePositions[mousePositions.Count - 1], mousePos) > minDistance)
        {
            mousePositions.Add(mousePos);
            _drawPoints.Add(new DollarPoint() { Point = new Vector2(mousePos.x, mousePos.y), StrokeIndex = _strokeIndex });

            currentLineRenderer.positionCount = mousePositions.Count;
            currentLineRenderer.SetPosition(mousePositions.Count - 1, worldPos);

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

        // Usuwamy wszystkie LineRenderery
        foreach (var line in lineRendererInstances)
        {
            Destroy(line);
        }
        lineRendererInstances.Clear();

        spellCastingParticleSystem.Stop();
        gameFreezer.SetIsCastSpelling(false);
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
            ExterminationImage.fillAmount -= 1 / ExterminationCooldown * Time.deltaTime;
        }
        if (FreezingCooldown > 0 && IsFreezeUnlocked)
        {
            FreezingImage.fillAmount -= 1 / FreezingCooldown* Time.deltaTime;
        }
        if (GreenballCooldown > 0 && IsGreenballUnlocked)
        {
            GreenBallImage.fillAmount -= 1 / GreenballCooldown * Time.deltaTime;
        }
        if (LightningCooldown > 0 && IsLightningUnlocked)
        {
            LightningImage.fillAmount -= 1 / LightningCooldown * Time.deltaTime;
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
}
