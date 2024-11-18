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
    public float minDistance = 20f; // Minimalna odleg�o�� mi�dzy punktami, aby unikn�� nadmiernej liczby punkt�w
    public GameObject fireballPrefab; // Prefab, kt�ry zawiera FireBaseScript
    public float spellCastDistance = 10f; // Odleg�o��, na jak� rzucane jest zakl�cie
    public GameObject lineRendererPrefab;
    private FireballScript fireballScript; 
    //private GameObject lineRendererInstance;
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
    private List<LineRenderer> _lineRenderers = new List<LineRenderer>();

    public Vector3 boxHalfExtents = new Vector3(0.001f, 0.001f, 0.001f);
    public LayerMask Layer;

    private void Start()
    {

        _meteorsScript = GetComponent<MeteroAOEDamage>();
        _fireballScript = GetComponent<FireballScript>();
        _shieldScript = GetComponent<Shield>();
        _chainLightningShootScript = GetComponent<ChainLightningShoot>();

        gameFreezer = FindObjectOfType<GameFreezer>();
        recognitionManager = new RecognitionManager();
        _playerVoiceCommands = GetComponent<PlayerVoiceCommands>();
        spellCastingParticleSystem.Stop();
        spellCastingParticleSystem.GetComponent<Renderer>().sortingOrder = 0;
        _audioManager = GetComponent<AudioManager>();

        UnlockSpell("Fireball");
        UnlockSpell("Meteors");
        UnlockSpell("Shield");
        UnlockSpell("Lightning");

    }

    void Update()
    {
        if (GameState.Instance.IsGameOver || GameState.Instance.IsGamePaused || GameState.Instance.IsUpgrading) return;

        if (GameState.Instance.IsGameOver || GameState.Instance.IsGamePaused) return;

        if (Input.GetMouseButton(1) && !GameState.Instance.IsSpellCasting && !GameState.Instance.IsGamePaused)
        {
            GameState.Instance.IsSpellCasting = true;
            gameFreezer.UpdateTimeScale();
            Cursor.lockState = CursorLockMode.Confined;
        }

        if (Input.GetMouseButtonDown(0) && GameState.Instance.IsSpellCasting)
        {
            lineRenderer = CreateNewLineRenderer();
        }

        if (Input.GetMouseButton(0) && GameState.Instance.IsSpellCasting)
        {
            HandleMouseInput();
        }

        if (Input.GetMouseButtonUp(0) && GameState.Instance.IsSpellCasting)
        {
            _strokeIndex++;
        }

        if (Input.GetMouseButtonUp(1) && GameState.Instance.IsSpellCasting)
        {
            FinalizeSpellCasting();
            _strokeIndex = 0;
        }

        updateCooldowns();
    }

    void RecognizeSpell(string name, float distance)
    {
        if (name != null || distance > 2f)
        {
            if (name.Equals("Fireball") && ( (_playerVoiceCommands.recognizedSpell == "Fireball" && _playerVoiceCommands.isOn) || !_playerVoiceCommands.isOn))
            {
                if (FireballImage.fillAmount <= 0)
                {
                    CastFireball();
                    FireballImage.fillAmount = 1;
                    FireballCooldown = 3f;
                }
            }
            else if (name.Equals("Meteors") && ((_playerVoiceCommands.recognizedSpell == "Meteors" && _playerVoiceCommands.isOn) || !_playerVoiceCommands.isOn))
            {
                if (MeteorsImage.fillAmount <= 0)
                {
                    CastMeteorRain();
                    MeteorsImage.fillAmount = 1;
                    MeteorsCooldown = 5f;
                }
            }
            else if (name.Equals("Shield") && ((_playerVoiceCommands.recognizedSpell == "Shield" && _playerVoiceCommands.isOn) || !_playerVoiceCommands.isOn))
            {
                if (ShieldImage.fillAmount <= 0)
                {
                    _shieldScript.activateShield();
                    ShieldImage.fillAmount = 1;
                    ShieldCooldown = 5f;
                }
            }
            else if (name.Equals("Lightning") && ((_playerVoiceCommands.recognizedSpell == "Lightning" && _playerVoiceCommands.isOn) || !_playerVoiceCommands.isOn))
            {
                if (MeteorsImage.fillAmount <= 0)
                {
                    _chainLightningShootScript.StartShooting();
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
        _playerVoiceCommands.recognizedSpell = null;
    }

    public void CastFireball()
    {
        _fireballScript.CastFireBallInDirection();
    }

    public void CastMeteorRain()
    {
        _meteorsScript.CastMeteorRain();
        _audioManager.PlayMeteorRainSound();
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

            lineRenderer.positionCount = mousePositions.Count;
            lineRenderer.SetPosition(mousePositions.Count - 1, worldPos);

            spellCastingParticleSystem.transform.position = worldPos;

            if (!spellCastingParticleSystem.isPlaying)
            {
                spellCastingParticleSystem.Play();
            }
        }
    }

    private LineRenderer CreateNewLineRenderer()
    {
        GameObject lineRendererInstance = Instantiate(lineRendererPrefab);
        LineRenderer lineRenderer = lineRendererInstance.GetComponent<LineRenderer>();
        mousePositions.Clear();
        lineRenderer.positionCount = 0;
        lineRenderer.sortingOrder = 1;
        _lineRenderers.Add(lineRenderer);
        spellCastingParticleSystem.Stop();
        return lineRenderer;
    }


    private void FinalizeSpellCasting()
    {
        (string result, float points) = recognitionManager.OnDrawFinished(_drawPoints.ToArray());
        _drawPoints.Clear();
        RecognizeSpell(result, points);
        mousePositions.Clear();
        ClearLineRenderers();
        //spellCastingParticleSystem.Stop();
        spellCastingParticleSystem.Stop();
        GameState.Instance.IsSpellCasting = false;
        gameFreezer.UpdateTimeScale();
        Cursor.lockState = CursorLockMode.Locked;

        _strokeIndex = 0;
    }

    private void ClearLineRenderers()
    {
        foreach (var lineRenderer in _lineRenderers)
        {
            Debug.Log(lineRenderer);
            lineRenderer.positionCount = 0;
            Destroy(lineRenderer.gameObject);
        }
        _lineRenderers.Clear();
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
            ShieldImage.fillAmount -= 1 / ShieldCooldown * Time.deltaTime;
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
        if (spell.Equals("Meteors"))
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
}
