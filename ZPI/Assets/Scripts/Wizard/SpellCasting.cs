using DigitalRuby.PyroParticles;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class SpellCasting : MonoBehaviour
{
    private List<Vector3> mousePositions = new List<Vector3>();
    private InputManager inputManager;
    private FireBaseScript prefabScript;
    private GameObject fireball;
    public float minDistance = 20f; // Minimalna odleg³oœæ miêdzy punktami, aby unikn¹æ nadmiernej liczby punktów
    public GameObject fireballPrefab; // Prefab, który zawiera FireBaseScript
    public float spellCastDistance = 10f; // Odleg³oœæ, na jak¹ rzucane jest zaklêcie

    private void Start()
    {
        // Pobieramy referencjê do skryptu obracaj¹cego kamerê
        inputManager = FindObjectOfType<InputManager>();
    }

    void Update()
    {
        // Œledzenie ruchu myszy, gdy przycisk jest wciœniêty
        if (Input.GetMouseButton(0)) // Lewy przycisk myszy
        {
            inputManager.isCastSpelling = true;
            Vector3 screenPos = Input.mousePosition;
            Vector3 mousePosV2 = Input.mousePosition;

            if (mousePositions.Count == 0 || Vector3.Distance(mousePositions[mousePositions.Count - 1], mousePosV2) > minDistance)
            {
                if (mousePositions.Count > 0)
                {
                }
                mousePositions.Add(mousePosV2); // Dodajemy pozycjê myszy do listy, jeœli jest wystarczaj¹co daleko od ostatniego punktu
            }
        }

        // Gdy przycisk myszy zostanie puszczony, analizujemy gest
        if (Input.GetMouseButtonUp(0))
        {
            inputManager.isCastSpelling = false;
            RecognizeSpell(); // Funkcja do rozpoznawania zaklêcia
            mousePositions.Clear(); // Wyczyœæ listê punktów po analizie
        }
    }

    void RecognizeSpell()
    {
        // Przyk³ad: proste rozpoznawanie linii pionowej
        if (IsVerticalLine())
        {
            CastFireSpellInDirection();
        }
        else if (IsHorizontalLine())
        {
            CastSpell("IceBeam");
        }
        else if (IsZShape())
        {
            CastSpell("Z gowno");
        }
        else 
        {
            Debug.Log("Unrecognized spell pattern");
        }
        Debug.Log("Spell casted! Number of points: " + mousePositions.Count);
    }

    void CastFireSpellInDirection()
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

    bool IsVerticalLine()
    {
        // Proste sprawdzenie, czy œlad przypomina pionow¹ liniê
        if (mousePositions.Count < 2) return false;

        float previousX  = Math.Abs(mousePositions[0].x);
        float threshold = 6f; // Tolerowanie odchylenie dla kolejnych punktow

        foreach (var point in mousePositions)
        {
            //Debug.Log("POPRZEDNI: " + previousX);
            //Debug.Log(point.x);
            if (Math.Abs(previousX - Math.Abs(point.x)) > threshold)
            {
                return false;
            }
            previousX = Math.Abs(point.x);
        }
        return true;
    }

    bool IsHorizontalLine()
    {
        // Proste sprawdzenie, czy œlad przypomina pionow¹ liniê
        if (mousePositions.Count < 2) return false;

        float previousY = Math.Abs(mousePositions[0].y);
        float threshold = 6f; // Tolerowanie odchylenie dla kolejnych punktow

        foreach (var point in mousePositions)
        {
            if (Math.Abs(previousY - Math.Abs(point.y)) > threshold)
            {
                return false;
            }
            previousY = Math.Abs(point.y);
        }
        return true;
    }

    // Funkcja do porównywania kierunków z wzorcem "Z"
    bool IsZShape()
    {
        if (mousePositions.Count < 3) return false;

        // Analiza kierunków miêdzy punktami
        List<Vector2> directions = new List<Vector2>();

        for (int i = 1; i < mousePositions.Count; i++)
        {
            Vector2 direction = (mousePositions[i] - mousePositions[i - 1]).normalized;
            directions.Add(direction);
        }

        // SprawdŸ, czy ruch odpowiada wzorcowi litery Z
        bool firstLine = false;
        bool diagonalLine = false;
        bool secondLine = false;

        foreach (var dir in directions)
        {
            //Debug.Log(dir);
            //Debug.Log("WSPOLRZEDNA IKSOWA: " + dir.x);
            if (!firstLine && Mathf.Abs(dir.x) > Mathf.Abs(dir.y) && dir.x > 0) // Poziomy ruch w prawo
            {
                //Debug.Log("PIERWSZA LINIA");
                firstLine = true;
            }
            else if (firstLine && !diagonalLine && dir.x < 0 && dir.y < 0) // Ukoœny ruch w lewo-dó³
            {
                //Debug.Log("DRUGA LINIA");
                diagonalLine = true;
            }
            else if (diagonalLine && !secondLine && Mathf.Abs(dir.x) > Mathf.Abs(dir.y) && dir.x > 0) // Drugi poziomy ruch w prawo
            {
                secondLine = true;
                break; // Wzorzec rozpoznany, mo¿na zakoñczyæ pêtlê
            }
        }

        return firstLine && diagonalLine && secondLine;
    }

    void CastSpell(string spellName)
    {
        Debug.Log("Casting spell: " + spellName);
        // Tu dodaj logikê odpowiedniego zaklêcia
    }
}
