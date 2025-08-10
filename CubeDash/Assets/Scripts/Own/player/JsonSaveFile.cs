using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonSaveFile : MonoBehaviour
{
    public static JsonSaveFile instance;


    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public List<UpgradesSimple> AllUnlockedUpgrades;
    private PlayerDataSimple playerDataSimple;

    private void Start()
    {
        playerDataSimple = PlayerDataSimple.Instance;
        LoadGame();
    }
    public void SaveGame()
    {
        PlayerDataInitialize saveData = new PlayerDataInitialize
        {
            maxDistance = playerDataSimple.maxDistance,
            dashInterval = playerDataSimple.dashInterval,
            dashDuration = playerDataSimple.dashDuration,
            maxHealth = playerDataSimple.maxHealth,
            attackAmount = playerDataSimple.attackAmount,
            UpgradeIndex = new List<int>(playerDataSimple.UpgradeIndex)
        };


        string json = JsonUtility.ToJson(saveData, true);
        Debug.Log("Saving JSON: " + json);

        string path = Path.Combine(Application.persistentDataPath, "SaveDataRetroDash.json");
        Debug.Log("Save path: " + path);
        File.WriteAllText(path, json);

    }

    public void ResetProgress()
    {
        string path = Path.Combine(Application.persistentDataPath, "SaveDataRetroDash.json");

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted. Progress reset.");
        }

        playerDataSimple.SetPlayerData(
            maxDistance: 30f,
            dashInterval: 1f,
            dashDuration: 0.2f,
            maxHealth: 5f,
            attackAmount: 1f,
            upgradeIndex: new List<int>(),
            canShockWave: false,
            canShootProjectiles: false
        );

        UpgradeContainerSimple upgradeContainer = FindFirstObjectByType<UpgradeContainerSimple>();

        if ( upgradeContainer != null)
        {
            foreach (var upgrade in upgradeContainer.allUpgrades)
            {
                upgrade.isUnlocked = false;
                upgrade.canBeUnlocked = false;

                if (upgrade.isRoot)
                {
                    upgrade.isUnlocked = true;
                }
            }

            upgradeContainer.availableUpgrades.Clear();
            StartCoroutine(upgradeContainer.SetVisualUpgrade());
        }

        SaveGame();
    }

    public void LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "SaveDataRetroDash.json");
        if (!File.Exists(path))
        {
            Debug.LogWarning("Save file not found: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        PlayerDataInitialize data = JsonUtility.FromJson<PlayerDataInitialize>(json);

        PlayerDataSimple.Instance.SetPlayerData(
            data.maxDistance,
            data.dashInterval,
            data.dashDuration,
            data.maxHealth,
            data.attackAmount,
            data.UpgradeIndex,
            data.canShockWave,
            data.canShootProjectiles
        );
    }
}
