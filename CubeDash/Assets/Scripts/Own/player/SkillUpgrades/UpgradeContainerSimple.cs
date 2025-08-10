using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class UpgradeContainerSimple : MonoBehaviour
{
    public List<UpgradesSimple> allUpgrades = new List<UpgradesSimple>();
    public List<UpgradesSimple> availableUpgrades = new List<UpgradesSimple>();
    public GameObject upgradePrefab;
    public GameObject prefabParent;

    private void Start()
    {
        SetAtStart();
    }

    public void SetAtStart()
    {
        foreach (UpgradesSimple upgrades in allUpgrades)
        {
            GameObject upgradeVisual = Instantiate(upgradePrefab);

            UpgradeVisualSimple upgradeSetter = upgradeVisual.GetComponent<UpgradeVisualSimple>();
            upgradeSetter.upgrade = upgrades;
            upgradeSetter.SetVisual();

            upgradeVisual.transform.parent = prefabParent.transform;
            upgradeVisual.transform.localScale = Vector3.one;
            upgradeVisual.transform.localPosition = upgrades.position;
        }
    }


    public void CheckUpgradeState()
    {
        foreach (UpgradesSimple upgrade in availableUpgrades)
        {
            upgrade.canBeUnlocked = true;
        }

        StartCoroutine(SetVisualUpgrade());
    }

    public IEnumerator SetVisualUpgrade()
    {
        yield return null;

        foreach (UpgradeVisualSimple upgrade in FindObjectsByType<UpgradeVisualSimple>(FindObjectsSortMode.None))
        {
            upgrade.SetVisual();
        }
    }
}
