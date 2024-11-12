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
        ELECTRIC_UNLOCK = 8
    }

    private List<SkillType> unlockedSkillTypeList;
    private Dictionary<SkillType, List<SkillType>> skillRequirements;

    private void Start()
    {
        unlockedSkillTypeList = new List<SkillType>();
        InitializeSkillRequirements();
    }

    public void UnlockSkill(SkillType skillType)
    {
        unlockedSkillTypeList.Add(skillType);
    }

    public bool IsSkillUnlocked(SkillType skillType)
    {
        return unlockedSkillTypeList.Contains(skillType);
    }

    public bool CanUnlockSkill(SkillType skillType)
    {
        if (!skillRequirements.ContainsKey(skillType))
        {
            return true;
        }
        foreach (var requiredSkill in skillRequirements[skillType])
        {
            if (!IsSkillUnlocked(requiredSkill))
            {
                return false;
            }
        }
        return true;
    }

    private void InitializeSkillRequirements()
    {
        skillRequirements = new Dictionary<SkillType, List<SkillType>>
        {
            { SkillType.FIREBALL_UNLOCK, new List<SkillType> { SkillType.BASE_BUFF } },
            { SkillType.SHIELD_UNLOCK, new List<SkillType> { SkillType.BASE_BUFF } },
            { SkillType.METEOR_UNLOCK, new List<SkillType> { SkillType.BASE_BUFF } },
            { SkillType.FIREBALL_BUFF, new List<SkillType>() { SkillType.FIREBALL_UNLOCK } },
            { SkillType.HEALTH_BUFF, new List<SkillType>() { SkillType.SHIELD_UNLOCK } },
            { SkillType.METEOR_BUFF, new List<SkillType>() { SkillType.METEOR_UNLOCK } },
            { SkillType.ELECTRIC_UNLOCK, new List<SkillType> { SkillType.FIREBALL_BUFF, SkillType.HEALTH_BUFF, SkillType.METEOR_BUFF } }
        };
    }
}
