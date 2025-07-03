using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonUI : MonoBehaviour
{
    public SkillUpgrade upgrade;
    public Button button;
    public Image backgroundImage;
    public TextMeshProUGUI nameText;

    public void Setup(SkillUpgrade upgradeData, SkillTreeUI treeUI)
    {
        upgrade = upgradeData;
        nameText.text = upgrade.upgradeName;
        backgroundImage.sprite = upgrade.symbol;
        button.onClick.AddListener(() => treeUI.OnUpgradeClicked(upgrade));
        transform.localPosition = upgrade.uiPosition;
    }

    public void SetColor(Color c)
    {
        backgroundImage.color = c;
    }

    public void SetInteractable(bool isUnlocked, bool isAvailable)
    {
        if (isUnlocked)
        {
            button.interactable = false;  // al unlocked, niet meer klikbaar
        }
        else if (isAvailable)
        {
            button.interactable = true;   // beschikbaar om unlocked te worden
        }
        else
        {
            button.interactable = false;  // niet unlockable, niet klikbaar
        }
    }
}


