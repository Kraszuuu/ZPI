using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    public enum SkillType
    {
        BASE_BUFF = 1,
        FIREBALL_UNLOCK = 2,
        SHIELD_UNLOCK = 3,
        METEOR_UNLOCK = 4,
        FIREBALL_BUFF = 5,
        HEALTH_BUFF = 6,
        METEOR_BUFF = 7,
        ELECTRIC_UNLOCK = 8,
        HEAL_1 = 9,
        HEAL_2 = 10,
        HEAL_3 = 11
    }

    private List<SkillType> _unlockedSkillTypeList;
    private Dictionary<SkillType, List<SkillType>> _skillRequirements;

    private void Start()
    {
        _unlockedSkillTypeList = new List<SkillType>();
        InitializeSkillRequirements();
    }

    public void UnlockSkill(SkillType skillType)
    {
        _unlockedSkillTypeList.Add(skillType);
    }

    public bool IsSkillUnlocked(SkillType skillType)
    {
        return _unlockedSkillTypeList.Contains(skillType);
    }

    public bool CanUnlockSkill(SkillType skillType)
    {
        if (!_skillRequirements.ContainsKey(skillType))
        {
            return true;
        }
        foreach (var requiredSkill in _skillRequirements[skillType])
        {
            if (!IsSkillUnlocked(requiredSkill))
            {
                return false;
            }
        }
        return true;
    }

    public bool IsEverySkillUnlocked()
    {
        return System.Enum.GetValues(typeof(SkillType)).Length == _unlockedSkillTypeList.Count;
    }

    private void InitializeSkillRequirements()
    {
        _skillRequirements = new Dictionary<SkillType, List<SkillType>>
        {
            { SkillType.FIREBALL_UNLOCK, new List<SkillType> { SkillType.BASE_BUFF } },
            { SkillType.SHIELD_UNLOCK, new List<SkillType> { SkillType.BASE_BUFF } },
            { SkillType.METEOR_UNLOCK, new List<SkillType> { SkillType.BASE_BUFF } },
            { SkillType.FIREBALL_BUFF, new List<SkillType>() { SkillType.FIREBALL_UNLOCK } },
            { SkillType.HEALTH_BUFF, new List<SkillType>() { SkillType.SHIELD_UNLOCK } },
            { SkillType.METEOR_BUFF, new List<SkillType>() { SkillType.METEOR_UNLOCK } },
            { SkillType.ELECTRIC_UNLOCK, new List<SkillType> { SkillType.FIREBALL_BUFF, SkillType.HEALTH_BUFF, SkillType.METEOR_BUFF } },
            { SkillType.HEAL_2, new List<SkillType>() { SkillType.HEAL_1 } },
            { SkillType.HEAL_3, new List<SkillType>() { SkillType.HEAL_2 } }
        };
    }
}
