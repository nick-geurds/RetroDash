using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public TMP_Text xpText;
    public SkillTreeManager skillTreeManager;
    private void OnEnable()
    {
        if (EXPManager.instance != null)
        {
            EXPManager.instance.OnXPChanged += UpdateXPText;
            UpdateXPText(EXPManager.instance.totalExp); // <- gebruik waarde uit memory
        }
        else
        {
            // Als EXPManager nog niet bestond (bijv. in editor direct geladen)
            int xp = PlayerPrefs.GetInt("PlayerTotalEXP", 0);
            UpdateXPText(xp);
        }
    }

    private void OnDisable()
    {
        if (EXPManager.instance != null)
            EXPManager.instance.OnXPChanged -= UpdateXPText;
    }

    private void UpdateXPText(int xp)
    {
        xpText.text = $"XP: {xp}";
    }

    public void ResetXpMain()
    {
        if (EXPManager.instance != null)
            EXPManager.instance.ResetAll();
    }
    public void ResetSkillTree()
    {
        GameResetHelper.FullReset(skillTreeManager);
    }
}
