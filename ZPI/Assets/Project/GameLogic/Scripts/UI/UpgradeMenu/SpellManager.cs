using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance { get; private set; }

    private Dictionary<string, float> spells = new Dictionary<string, float>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSpells();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSpells()
    {
        spells["PrimaryAttack"] = 10f;
        spells["Fireball"] = 30f;
        spells["Shield"] = 5f;
        spells["Lightning"] = 30f;
        spells["Meteors"] = 2f;
    }

    public float GetSpellData(string spellName)
    {
        return spells[spellName];
    }

    public void UpgradeSpell(string spellName)
    {
        if (spells.ContainsKey(spellName))
        {
            spells[spellName] *= 1.5f;
        }
    }
}
