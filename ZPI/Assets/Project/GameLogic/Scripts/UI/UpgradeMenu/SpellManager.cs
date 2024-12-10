using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance { get; private set; }

    private Dictionary<string, float> spellData = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        InitializeSpells();
    }

    private void InitializeSpells()
    {
        spellData["PrimaryAttack"] = 4f;
        spellData["PrimaryAttackAreaDamage"] = 0f;
        spellData["Fireball"] = 50f;
        spellData["FireballAreaDamage"] = 20f;
        spellData["Shield"] = 6f;
        spellData["Lightning"] = 40f;
        spellData["Meteors"] = 2f;
    }

    public float GetSpellData(string spellName)
    {
        if (spellData.TryGetValue(spellName, out float value))
        {
            return value;
        }
        Debug.LogWarning($"Spell data for {spellName} not found!");
        return 0;
    }

    public void SetSpellData(string spellName, float value)
    {
        if (spellData.ContainsKey(spellName))
        {
            spellData[spellName] = value;
        }
        else
        {
            spellData.Add(spellName, value);
        }
    }

    public void UpgradeSpell(string spellName)
    {
        if (spellData.ContainsKey(spellName))
        {
            spellData[spellName] *= 1.5f;
        }
    }
}
