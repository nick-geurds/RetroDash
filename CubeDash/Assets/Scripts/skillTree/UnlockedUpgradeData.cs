using System.Collections.Generic;
using UnityEngine;

public class UnlockedUpgradeData : MonoBehaviour
{
    public static UnlockedUpgradeData Instance { get; private set; }

    [Header("References")]
    public SkillTree skillTree;

    private HashSet<int> unlockedUpgradeIndices = new();

    [Header("Debug (runtime only)")]
    [SerializeField] private List<int> debugUnlockedUpgradeIndices = new(); // Alleen voor de Inspector

    public bool IsInitialized => unlockedUpgradeIndices.Count > 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (skillTree != null)
        {
            Debug.Log("[UnlockedUpgradeData] Initial skillTree sync on Start");
            SyncExistingUpgradesFromSkillTree();
        }
        else
        {
            Debug.LogWarning("[UnlockedUpgradeData] Geen SkillTree gekoppeld tijdens Start – kon niet synchroniseren");
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadFromPlayerPrefs();
    }

    public void LoadFromPlayerPrefs()
    {
        unlockedUpgradeIndices.Clear();
        debugUnlockedUpgradeIndices.Clear(); // Clear de debuglijst

        for (int i = 0; i < 100; i++)
        {
            if (PlayerPrefs.GetInt("Upgrade_" + i, 0) == 1)
            {
                unlockedUpgradeIndices.Add(i);
                debugUnlockedUpgradeIndices.Add(i); // Synchroniseer met debuglijst
            }
        }

        Debug.Log($"[UnlockedUpgradeData] Loaded {unlockedUpgradeIndices.Count} unlocked upgrades.");
    }

    public bool HasUnlocked(int upgradeIndex)
    {
        return unlockedUpgradeIndices.Contains(upgradeIndex);
    }

    public HashSet<int> GetUnlockedIndices()
    {
        return new HashSet<int>(unlockedUpgradeIndices);
    }

    public void ApplyTo(PlayerStats stats)
    {
        stats.ResetToBaseStats();

        foreach (var upgrade in skillTree.allUpgrades)
        {
            if (HasUnlocked(upgrade.upgradeIndex))
            {
                Debug.Log($"Applying {upgrade.upgradeName} ({upgrade.upgradeType}) with value {upgrade.upgradeValue}");
                upgrade.ApplyUpgrade(stats);
            }
        }

        Debug.Log($"[UnlockedUpgradeData] Applied upgrades to player stats. Final MaxHealth = {stats.maxHealth}");
    }

    public void AddUnlockedUpgrade(SkillUpgrade upgrade)
    {
        if (!unlockedUpgradeIndices.Contains(upgrade.upgradeIndex))
        {
            unlockedUpgradeIndices.Add(upgrade.upgradeIndex);
            debugUnlockedUpgradeIndices.Add(upgrade.upgradeIndex);
            PlayerPrefs.SetInt("Upgrade_" + upgrade.upgradeIndex, 1);
            PlayerPrefs.Save();

            Debug.Log($"[UnlockedUpgradeData] Upgrade '{upgrade.upgradeName}' toegevoegd (index {upgrade.upgradeIndex})");
        }
    }

    public void SyncFromUpgrades(List<SkillUpgrade> upgrades)
    {
        unlockedUpgradeIndices.Clear();
        debugUnlockedUpgradeIndices.Clear();

        for (int i = 0; i < 100; i++)
        {
            PlayerPrefs.DeleteKey("Upgrade_" + i);
        }

        foreach (var upgrade in upgrades)
        {
            AddUnlockedUpgrade(upgrade);
        }

        PlayerPrefs.Save();

        Debug.Log($"[UnlockedUpgradeData] Gesynchroniseerd met {upgrades.Count} upgrades.");
    }


    private void SyncExistingUpgradesFromSkillTree()
    {
        foreach (var upgrade in skillTree.allUpgrades)
        {
            if (PlayerPrefs.GetInt("Upgrade_" + upgrade.upgradeIndex, 0) == 1)
            {
                AddUnlockedUpgrade(upgrade); // Zorgt dat upgrade nogmaals wordt toegevoegd aan lijst & PlayerPrefs indien nodig
            }
        }
    }

}
