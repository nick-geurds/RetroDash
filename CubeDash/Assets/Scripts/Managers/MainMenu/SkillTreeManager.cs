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
            unlockedUpgrades.Add(upgrade);
            unlockedIndices.Add(upgrade.upgradeIndex);
            availableUpgrades.Remove(upgrade);

            UpdateAvailableUpgrades();

            Debug.Log($"Unlocked upgrade: {upgrade.upgradeName} (Index: {upgrade.upgradeIndex})");

            SaveProgress();

            // Vraag zichtbare upgrades op
            var visibleUpgrades = GetVisibleUpgrades();

            // Update lijnen met zichtbaarheid
            foreach (var line in Object.FindObjectsByType<LineConnection>(FindObjectsSortMode.None))
            {
                line.UpdateVisual();
            }
            skillTreeUI?.UpdateUI();
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

        Initialize();
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

}
