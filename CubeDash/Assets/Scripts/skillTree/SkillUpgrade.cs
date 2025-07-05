using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Skill Upgrade", menuName = "Skill Tree/Upgrade")]
public class SkillUpgrade : ScriptableObject
{
    public int upgradeIndex;
    public enum SkillUpgradeType
    {
        None,
        IncreaseMaxHealth,
        IncreaseAttack,
        IncreaseDashDistance,
        ReduceDashCooldown,
        IncreaseDashSpeed
    }

    public string upgradeName;
    public string description;

    public Sprite symbol;

    public bool isUnlockedAtStart;

    // Welke upgrades komen vrij als deze wordt gekocht
    public List<SkillUpgrade> unlocksAfterThis;

    // Voor visualisatie (optioneel)
    public Vector2 uiPosition;

    public SkillUpgradeType upgradeType;
    public float upgradeValue = 1f;

    public bool IsUnlocked => PlayerPrefs.GetInt("Upgrade_" + upgradeIndex, 0) == 1;

    public void Unlock(PlayerStats playerStats)
    {
        if (IsUnlocked) return;

        ApplyUpgrade(playerStats);
        PlayerPrefs.SetInt("Upgrade_" + upgradeIndex, 1);
        PlayerPrefs.Save();
    }

    public void ApplyUpgrade(PlayerStats stats)
    {
        switch (upgradeType)
        {
            case SkillUpgradeType.IncreaseMaxHealth:
                stats.maxHealth += upgradeValue;
                break;
            case SkillUpgradeType.IncreaseAttack:
                stats.attackAmount += upgradeValue;
                break;
            case SkillUpgradeType.IncreaseDashDistance:
                stats.dashDis += upgradeValue;
                break;
            case SkillUpgradeType.ReduceDashCooldown:
                stats.dashInterval -= upgradeValue;
                break;
            case SkillUpgradeType.IncreaseDashSpeed:
                stats.dashSpeed += upgradeValue;
                break;
        }
    }
}