using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellCasting : MonoBehaviour
{
    public CameraShake CameraShake;
    public float MinDistanceBetweenPoints = 20f;
    public float SpellCastDistance = 10f;
    public GameObject LineRendererPrefab;
    public ParticleSystem SpellCastingParticleSystem;
    public event Action<DollarPoint[]> OnDrawFinished;

    private List<Vector3> _mousePositions = new();
    private List<DollarPoint> _drawPoints = new();
    private GameFreezer _gameFreezer;
    private LineRenderer _lineRenderer;
    private PlayerVoiceCommands _playerVoiceCommands;
    private RecognitionManager _recognitionManager;

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

    private List<LineRenderer> _lineRenderers = new List<LineRenderer>();

    public Vector3 boxHalfExtents = new Vector3(0.001f, 0.001f, 0.001f);
    public LayerMask Layer;

    private void Start()
    {

        _meteorsScript = GetComponent<MeteroAOEDamage>();
        _fireballScript = GetComponent<FireballScript>();
        _shieldScript = GetComponent<Shield>();
        _chainLightningShootScript = GetComponent<ChainLightningShoot>();

        _gameFreezer = FindObjectOfType<GameFreezer>();
        _recognitionManager = new RecognitionManager();
        _playerVoiceCommands = GetComponent<PlayerVoiceCommands>();
        SpellCastingParticleSystem.Stop();
        SpellCastingParticleSystem.GetComponent<Renderer>().sortingOrder = 0;
    }

    void Update()
    {
        if (GameState.Instance.IsGameOver || GameState.Instance.IsGamePaused || GameState.Instance.IsUpgrading || !GameState.Instance.IsWandEquipped) return;

        if (GameState.Instance.IsGameOver || GameState.Instance.IsGamePaused) return;

        if (Input.GetMouseButton(1) && !GameState.Instance.IsSpellCasting && !GameState.Instance.IsGamePaused)
        {
            GameState.Instance.IsSpellCasting = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        if (Input.GetMouseButtonDown(0) && GameState.Instance.IsSpellCasting)
        {
            _lineRenderer = CreateNewLineRenderer();
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
            if (name.Equals("Fireball") && ((_playerVoiceCommands.recognizedSpell == "Fireball" && GameState.IsSpeechRecognitionEnabled) || !GameState.IsSpeechRecognitionEnabled))
            {
                if (FireballImage.fillAmount <= 0)
                {
                    CastFireball();
                    FireballImage.fillAmount = 1;
                    FireballCooldown = 3f;
                }
            }
            else if (name.Equals("Meteors") && ((_playerVoiceCommands.recognizedSpell == "Meteors" && GameState.IsSpeechRecognitionEnabled) || !GameState.IsSpeechRecognitionEnabled))
            {
                if (MeteorsImage.fillAmount <= 0)
                {
                    CastMeteorRain();
                    MeteorsImage.fillAmount = 1;
                    MeteorsCooldown = 5f;
                }
            }
            else if (name.Equals("Shield") && ((_playerVoiceCommands.recognizedSpell == "Shield" && GameState.IsSpeechRecognitionEnabled) || !GameState.IsSpeechRecognitionEnabled))
            {
                if (ShieldImage.fillAmount <= 0)
                {
                    _shieldScript.activateShield();
                    ShieldImage.fillAmount = 1;
                    ShieldCooldown = 5f;
                }
            }
            else if (name.Equals("Lightning") && ((_playerVoiceCommands.recognizedSpell == "Lightning" && GameState.IsSpeechRecognitionEnabled) || !GameState.IsSpeechRecognitionEnabled))
            {
                if (LightningImage.fillAmount <= 0)
                {
                    _chainLightningShootScript.StartShooting();
                    LightningImage.fillAmount = 1;
                    LightningCooldown = 5f;
                }
            }
            Debug.Log("Spell casted! Number of points: " + _mousePositions.Count);
            _playerVoiceCommands.recognizedSpell = null;
            _playerVoiceCommands.recognizedWord = null;
            _playerVoiceCommands.recognizedWordText.text = null;
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
    }


    private void HandleMouseInput()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0.8f; // Odległość od kamery (aby przekształcić do przestrzeni świata)
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        if (_mousePositions.Count == 0 || Vector3.Distance(_mousePositions[_mousePositions.Count - 1], mousePos) > MinDistanceBetweenPoints)
        {
            _mousePositions.Add(mousePos);
            _drawPoints.Add(new DollarPoint() { Point = new Vector2(mousePos.x, mousePos.y), StrokeIndex = _strokeIndex });

            _lineRenderer.positionCount = _mousePositions.Count;
            _lineRenderer.SetPosition(_mousePositions.Count - 1, worldPos);

            SpellCastingParticleSystem.transform.position = worldPos;

            if (!SpellCastingParticleSystem.isPlaying)
            {
                SpellCastingParticleSystem.Play();
            }
        }
    }

    private LineRenderer CreateNewLineRenderer()
    {
        GameObject lineRendererInstance = Instantiate(LineRendererPrefab);
        LineRenderer lineRenderer = lineRendererInstance.GetComponent<LineRenderer>();
        _mousePositions.Clear();
        lineRenderer.positionCount = 0;
        lineRenderer.sortingOrder = 1;
        _lineRenderers.Add(lineRenderer);
        SpellCastingParticleSystem.Stop();
        return lineRenderer;
    }


    private void FinalizeSpellCasting()
    {
        (string result, float points) = _recognitionManager.OnDrawFinished(_drawPoints.ToArray());
        _drawPoints.Clear();
        RecognizeSpell(result, points);
        _mousePositions.Clear();
        ClearLineRenderers();
        SpellCastingParticleSystem.Stop();
        GameState.Instance.IsSpellCasting = false;
        // _gameFreezer.UpdateTimeScaleCoroutine();
        Cursor.lockState = CursorLockMode.Locked;

        _strokeIndex = 0;
    }

    private void ClearLineRenderers()
    {
        foreach (var lineRenderer in _lineRenderers)
        {
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
