using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Create", menuName = "SkillTreeUpgrades", order = 11)]
public class UpgradesSimple : ScriptableObject
{
    public enum UpgradeTypeSimple 
    { 
        maxDistance, 
        dashInterval, 
        dashDuration, 
        maxHealth, 
        attackAmount,
        canShockWave,
        canShootProjectiles,
        none
    };
    
    public int UpgradeIndexNew;
    public Vector2 position;

    [Header("Visual Settings")]
    public float iconSize = 1f;
    public Sprite iconSprite;

    public UpgradeTypeSimple type;
    public float floatAmount;
    public bool boolValue;

    public List<UpgradesSimple> unlocksAfterThis;

    public bool canBeUnlocked = false;  

    public bool isUnlocked = false;

    public bool isRoot = false;
    
    public void Upgrade(PlayerDataSimple playerData)
    {
        isUnlocked = true;

        switch (type) 
        {
            case UpgradeTypeSimple.maxDistance:
                {
                    playerData.maxDistance += floatAmount;
                }
                break;
            case UpgradeTypeSimple.dashInterval:
                {
                    playerData.dashInterval -= floatAmount;
                }
                break;
            case UpgradeTypeSimple.dashDuration:
                {
                    playerData.dashDuration -= floatAmount;
                }
                break;
            case UpgradeTypeSimple.maxHealth:
                {
                    playerData.maxHealth += floatAmount;
                }
                break;
            case UpgradeTypeSimple.attackAmount:
                {
                    playerData.attackAmount += floatAmount;
                }
                break;
            case UpgradeTypeSimple.canShockWave:
                {
                    playerData.canShockWave = boolValue;
                }
                break;
            case UpgradeTypeSimple.canShootProjectiles:
                {
                    playerData.canShootProjectiles = boolValue;
                }
                break;
            case UpgradeTypeSimple.none:
                break;

           
        }

        playerData.UpgradeIndex.Add(UpgradeIndexNew);

        UpgradeContainerSimple upgrdeContainer = FindFirstObjectByType<UpgradeContainerSimple>();

        foreach (UpgradesSimple upgrade in unlocksAfterThis)
        {
            if (!upgrdeContainer.availableUpgrades.Contains(upgrade))
            {
                upgrdeContainer.availableUpgrades.Add(upgrade);
            }
        }

        upgrdeContainer.CheckUpgradeState();
    }



    public void IsUnlocked()
    {
        isUnlocked = true;
    }
}
