using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{
    public event Action OnUpgradeSelected;
    public PlayerSkills playerSkills;

    [System.Serializable]
    public class SkillButton
    {
        public PlayerSkills.SkillType skillType;
        public Button button;
    }

    public List<SkillButton> buttons;

    private Dictionary<PlayerSkills.SkillType, Button> skillButtonMap;

    private void Start()
    {
        skillButtonMap = new Dictionary<PlayerSkills.SkillType, Button>();

        foreach (var button in buttons)
        {
            skillButtonMap.Add(button.skillType, button.button);
        }

        UpdateSkillUI();
    }

    public void UpdateSkillUI()
    {
        foreach (var skill in skillButtonMap)
        {
            var skillType = skill.Key;
            var button = skill.Value;

            if (playerSkills.IsSkillUnlocked(skillType))
            {
                button.interactable = false;
                button.transform.Find("Border").gameObject.SetActive(true);
            }
            else if (playerSkills.CanUnlockSkill(skillType)) {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }
    }

    public void UnlockSkill(int skillTypeId)
    {
        playerSkills.UnlockSkill((PlayerSkills.SkillType)skillTypeId);
        OnUpgradeSelected?.Invoke();
        UpdateSkillUI();
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
