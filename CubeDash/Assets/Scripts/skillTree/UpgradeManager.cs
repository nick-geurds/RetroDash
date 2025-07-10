using UnityEngine;
using System.Collections.Generic;
using static SkillUpgrade;
using UnityEngine.InputSystem;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    private List<SkillUpgrade> activeUpgrades = new List<SkillUpgrade>();

    public bool IsInitialized => activeUpgrades.Count > 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SyncWithSkillTree(SkillTreeManager skillTree)
    {
        activeUpgrades.Clear();
        activeUpgrades.AddRange(skillTree.GetUnlockedUpgrades());

        Debug.Log($"[UpgradeManager] Synced {activeUpgrades.Count} upgrades from SkillTreeManager");

        if (UnlockedUpgradeData.Instance != null)
        {
            UnlockedUpgradeData.Instance.SyncFromUpgrades(activeUpgrades);
        }

        foreach (var u in activeUpgrades)
        {
            Debug.Log($"[UpgradeManager] Loaded upgrade from saved data: {u.upgradeName}");
        }
    }



    public void ApplyUpgradesTo(PlayerStats stats)
    {
        stats.ResetToBaseStats(); // Zorg dat deze method bestaat

        foreach (var upgrade in activeUpgrades)
        {
            switch (upgrade.upgradeType)
            {
                case SkillUpgrade.SkillUpgradeType.IncreaseMaxHealth:
                    stats.maxHealth += upgrade.upgradeValue;
                    break;
                case SkillUpgrade.SkillUpgradeType.IncreaseAttack:
                    stats.attackAmount += upgrade.upgradeValue;
                    break;
                case SkillUpgrade.SkillUpgradeType.IncreaseDashDistance:
                    stats.dashDis += upgrade.upgradeValue;
                    break;
                case SkillUpgrade.SkillUpgradeType.ReduceDashCooldown:
                    stats.dashInterval -= upgrade.upgradeValue;
                    break;
                case SkillUpgrade.SkillUpgradeType.IncreaseDashSpeed:
                    stats.dashSpeed += upgrade.upgradeValue;
                    break;
                case SkillUpgrade.SkillUpgradeType.EnableFireProjectilesOnDamage:
                    stats.fireProjectilesOnDamage = upgrade.boolValue;
                    break;
                case SkillUpgrade.SkillUpgradeType.ProjectileCount:
                    stats.damageProjectileCount += upgrade.intValue;
                    break;
            }
        }
    }
}