using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public SkillTreeManager skillTreeManager;
    private EXPManager expManager;
    public void ResetXpMain()
    {
        expManager = GameObject.FindWithTag("ExpManager").GetComponent<EXPManager>();

        if (expManager != null)
           expManager.ResetEXP();
    }

    public void ResetSkillTree()
    {
        GameResetHelper.FullReset(skillTreeManager);
    }
}
