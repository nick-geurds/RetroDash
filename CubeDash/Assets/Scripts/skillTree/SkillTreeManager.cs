using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SkillTreeManager : MonoBehaviour
{

    public SkillTree skillTree;        // ScriptableObject met alle upgrades
    public SkillTreeUI skillTreeUI;    // UI controller

    private HashSet<SkillUpgrade> unlockedUpgrades = new HashSet<SkillUpgrade>();
    private HashSet<SkillUpgrade> availableUpgrades = new HashSet<SkillUpgrade>();
    private HashSet<int> unlockedIndices = new HashSet<int>();

    private const string PlayerPrefsKey = "SkillTreeUnlockedIndices";



    // Wordt aangeroepen bij scene load om progress te laden
    public void Initialize()
    {
        unlockedUpgrades.Clear();
        availableUpgrades.Clear();
        unlockedIndices.Clear();

        string saved = PlayerPrefs.GetString(PlayerPrefsKey, "");
        HashSet<int> savedIndices = new HashSet<int>();

        if (!string.IsNullOrEmpty(saved))
        {
            string[] parts = saved.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in parts)
            {
                if (int.TryParse(p, out int idx))
                    savedIndices.Add(idx);
            }
        }

        // Voeg alle upgrades toe die unlocked at start zijn, of die opgeslagen zijn
        foreach (var upgrade in skillTree.allUpgrades)
        {
            if (upgrade.isUnlockedAtStart || savedIndices.Contains(upgrade.upgradeIndex))
            {
                unlockedUpgrades.Add(upgrade);
                unlockedIndices.Add(upgrade.upgradeIndex);
                Debug.Log($"Loaded unlocked upgrade: {upgrade.upgradeName} (Index: {upgrade.upgradeIndex})");
            }
        }

        UpdateAvailableUpgrades();

        skillTreeUI?.UpdateUI();

        if (skillTreeUI != null && skillTreeUI.gameObject.scene.isLoaded)
        {
            skillTreeUI.UpdateUI();
        }
        else
        {
            Debug.Log("[SkillTreeManager] UI not present in this scene. Skipping UpdateUI.");
            skillTreeUI = null; // voorkom verdere crashes
        }
    }

    // Berekent welke upgrades beschikbaar zijn om te unlocken, gebaseerd op unlocked upgrades
    private void UpdateAvailableUpgrades()
    {
        availableUpgrades.Clear();

        foreach (var unlocked in unlockedUpgrades)
        {
            if (unlocked.unlocksAfterThis != null)
            {
                foreach (var next in unlocked.unlocksAfterThis)
                {
                    if (!unlockedUpgrades.Contains(next))
                        availableUpgrades.Add(next);
                }
            }
        }
    }

    // Check of een upgrade unlockbaar is
    public bool CanUnlock(SkillUpgrade upgrade)
    {
        if (IsUnlocked(upgrade))
            return false;

        if (upgrade.isUnlockedAtStart)
            return true;

        return availableUpgrades.Contains(upgrade);
    }

    // Unlock een upgrade en sla de progress op
    public void Unlock(SkillUpgrade upgrade)
    {
        if ((availableUpgrades.Contains(upgrade) || upgrade.isUnlockedAtStart) && !unlockedUpgrades.Contains(upgrade))
        {
            if (EXPManager.instance == null)
            {
                Debug.LogWarning("EXPManager instance not found.");
                return;
            }

            if (EXPManager.instance.totalExp < upgrade.upgradeCost)
            {
                Debug.Log($"Niet genoeg XP om upgrade '{upgrade.upgradeName}' te kopen (kost {upgrade.upgradeCost}, je hebt {EXPManager.instance.totalExp}).");
                return;
            }

            // Trek de XP af
            EXPManager.instance.AddEXP(-upgrade.upgradeCost);

            //  Markeer als unlocked (intern & in prefs)
            unlockedUpgrades.Add(upgrade);
            unlockedIndices.Add(upgrade.upgradeIndex);
            availableUpgrades.Remove(upgrade);

            //  Voeg toe aan centrale data-opslag zodat PlayerStats het later kan gebruiken
            if (UnlockedUpgradeData.Instance != null)
            {
                UnlockedUpgradeData.Instance.AddUnlockedUpgrade(upgrade);
            }
            else
            {
                Debug.LogWarning("UnlockedUpgradeData.Instance is null — kan upgrade niet registreren.");
            }

            UpdateAvailableUpgrades();

            SaveProgress();

            //  Visual update
            foreach (var line in Object.FindObjectsByType<LineConnection>(FindObjectsSortMode.None))
            {
                line.UpdateVisual();
            }

            skillTreeUI?.UpdateUI();

            Debug.Log($"Unlocked upgrade: {upgrade.upgradeName} (kostte {upgrade.upgradeCost} XP)");
        }
    }

    // Sla unlock progress op als een lijst met indices
    public void SaveProgress()
    {
        string saveString = string.Join(",", unlockedIndices);
        PlayerPrefs.SetString(PlayerPrefsKey, saveString);
        PlayerPrefs.Save();

        Debug.Log($"Saved progress indices: {saveString}");
    }

    // Check of een upgrade al unlocked is
    public bool IsUnlocked(SkillUpgrade upgrade) => unlockedUpgrades.Contains(upgrade);

    // Retourneert alle unlockbare upgrades (beschikbaar en nog niet unlocked)
    public List<SkillUpgrade> GetUnlockableUpgrades()
    {
        List<SkillUpgrade> unlockable = new List<SkillUpgrade>();

        foreach (var upgrade in skillTree.allUpgrades)
        {
            if (!IsUnlocked(upgrade) && CanUnlock(upgrade))
                unlockable.Add(upgrade);
        }

        return unlockable;
    }

    // Check of upgrade beschikbaar is om te unlocken
    public bool IsAvailable(SkillUpgrade upgrade)
    {
        return availableUpgrades.Contains(upgrade);
    }

    // Reset unlock progress (verwijder opgeslagen data)
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(PlayerPrefsKey);

        unlockedUpgrades.Clear();
        availableUpgrades.Clear();
        unlockedIndices.Clear();

        Debug.Log("[SkillTreeManager] SkillTree progress gewist.");

        // Sync UnlockedUpgradeData met lege lijst = wis alles daar ook
        if (UnlockedUpgradeData.Instance != null)
        {
            UnlockedUpgradeData.Instance.SyncFromUpgrades(new List<SkillUpgrade>());
        }

        Initialize();

        // Sync UpgradeManager met nieuwe state
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.SyncWithSkillTree(this);
        }
    }



    public HashSet<SkillUpgrade> GetVisibleUpgrades(int maxDepth = 2)
    {
        HashSet<SkillUpgrade> visible = new HashSet<SkillUpgrade>();
        Queue<(SkillUpgrade upgrade, int depth)> queue = new Queue<(SkillUpgrade, int)>();

        foreach (var unlocked in unlockedUpgrades)
        {
            queue.Enqueue((unlocked, 0));
        }

        while (queue.Count > 0)
        {
            var (current, depth) = queue.Dequeue();
            if (depth > maxDepth) continue;

            if (!visible.Contains(current))
                visible.Add(current);

            if (current.unlocksAfterThis != null && depth < maxDepth)
            {
                foreach (var next in current.unlocksAfterThis)
                {
                    if (!visible.Contains(next))
                        queue.Enqueue((next, depth + 1));
                }
            }
        }
        return visible;
    }

    public bool IsVisible(SkillUpgrade upgrade)
    {
        // Zichtbaar als unlocked of beschikbaar (je kan hier je eigen logic uitbreiden)
        return unlockedUpgrades.Contains(upgrade) || availableUpgrades.Contains(upgrade);
    }

    public IEnumerable<SkillUpgrade> GetUnlockedUpgrades()
    {
        return unlockedUpgrades;
    }

    public void SetSkillTreeUI(SkillTreeUI ui)
    {
        skillTreeUI = ui;
    }
}