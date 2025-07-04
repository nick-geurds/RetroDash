using UnityEngine;

public class SkillTreeBootstrap : MonoBehaviour
{
    void Start()
    {
        if (SkillTreeManager.Instance != null)
        {
            SkillTreeManager.Instance.Initialize();
            Debug.Log("[SkillTreeBootstrap] SkillTreeManager initialized.");

            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.SyncWithSkillTree(SkillTreeManager.Instance);
                Debug.Log("[SkillTreeBootstrap] UpgradeManager synced with SkillTree.");
            }
        }
        else
        {
            Debug.LogWarning("[SkillTreeBootstrap] No SkillTreeManager instance found.");
        }
    }
}
