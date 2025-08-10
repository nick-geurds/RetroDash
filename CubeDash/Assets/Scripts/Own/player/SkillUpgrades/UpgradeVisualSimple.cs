using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeVisualSimple : MonoBehaviour
{
    public UpgradesSimple upgrade;
    public Color upgradedColor;
    public Color notYetColor;

    public Button button;

    private Image iconImage;
    private ColorBlock buttonColorBlock;
    private UpgradeContainerSimple upgradeContainer;
    private PlayerDataSimple playerData;
    private void Start()
    {
        upgradeContainer = FindAnyObjectByType<UpgradeContainerSimple>();
        playerData = FindFirstObjectByType<PlayerDataSimple>();

        iconImage = GetComponent<Image>();
        iconImage.sprite = upgrade.iconSprite;

        buttonColorBlock = button.colors;
    }
    public void SetVisual()
    {
        if (upgrade.isUnlocked)
        {
            ColorBlock newColorBlock = button.colors;
            newColorBlock.disabledColor = upgradedColor;
            button.interactable = false;
            button.colors = newColorBlock;
        }
        else if (!upgrade.canBeUnlocked && !upgrade.isUnlocked)
        {
            ColorBlock newColorBlock = button.colors;
            newColorBlock.disabledColor = notYetColor;
            button.interactable = false;
            button.colors = newColorBlock;
            
        }
        else if(upgrade.canBeUnlocked && !upgrade.isUnlocked)
        {
            button.interactable = true;
        }
        

        if (upgrade.isRoot)
        {
            StartCoroutine(rootObject());
        }
    }

    IEnumerator rootObject()
    {
        yield return null;
        upgrade.Upgrade(playerData);
    }

    public void Upgrade()
    {
        upgrade.Upgrade(playerData);
    }
}
