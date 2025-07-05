using UnityEngine;

public static class GameResetHelper
{
    public static void FullReset(SkillTreeManager skillTreeManager)
    {
        if (skillTreeManager == null)
        {
            Debug.LogWarning("[GameResetHelper] Geen SkillTreeManager gevonden!");
            return;
        }

        skillTreeManager.ResetProgress();

        Debug.Log("[GameResetHelper] Volledige reset uitgevoerd.");
    }
}
