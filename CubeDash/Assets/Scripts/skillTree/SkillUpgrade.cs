using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

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
        IncreaseDashSpeed,

        //Fire projectiles when taken damage
        EnableFireProjectilesOnDamage,
        ProjectileCount,

        //shockwave when taken damage
        EnableShosckWave,
        ShockwaveRadius

    }

    public string upgradeName;
    public string description;

    public int upgradeCost = 0;
    public Sprite upgradeSymbol;
    public Sprite background;

    public bool isUnlockedAtStart;

    // Welke upgrades komen vrij als deze wordt gekocht
    public List<SkillUpgrade> unlocksAfterThis;

    // Voor visualisatie (optioneel)
    public Vector2 uiPosition;

    public SkillUpgradeType upgradeType;
    public bool boolValue = true;
    public int intValue = 1;
    public float upgradeValue = 1f;

    public Sprite icon;
    public Image iconHolder;
    public Color iconColor;
    public string Description;

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
            case SkillUpgradeType.EnableFireProjectilesOnDamage:
                stats.fireProjectilesOnDamage = boolValue;
                break;
            case SkillUpgradeType.ProjectileCount:
                stats.damageProjectileCount += intValue;
                break;
            case SkillUpgradeType.EnableShosckWave:
                stats.enableShockWave = boolValue;
                break;
            case SkillUpgradeType.ShockwaveRadius:
                stats.shockwaveRadius += upgradeValue;
                break;
        }
    }
}