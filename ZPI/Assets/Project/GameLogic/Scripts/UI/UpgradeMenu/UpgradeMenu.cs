using Radishmouse;
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

    [System.Serializable]
    public class SkillPath
    {
        public PlayerSkills.SkillType fromSkill;
        public PlayerSkills.SkillType toSkill;
        public UILineRenderer pathRenderer;
    }

    public List<SkillButton> buttons;
    public List<SkillPath> paths;

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

        foreach (var path in paths)
        {
            bool isUnlocked = playerSkills.IsSkillUnlocked(path.toSkill);
            path.pathRenderer.SetUnlocked(isUnlocked);
        }
    }

    public void UnlockSkill(int skillTypeId)
    {
        playerSkills.UnlockSkill((PlayerSkills.SkillType)skillTypeId);
        OnUpgradeSelected?.Invoke();
        UpdateSkillUI();
        Cursor.lockState = CursorLockMode.Locked;
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
