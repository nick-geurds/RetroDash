using System.Collections;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private JsonSaveFile jsonSaveFile;
    private UpgradeContainerSimple upgradeContainer;
    void Start()
    {
        upgradeContainer = GetComponent<UpgradeContainerSimple>();
        LoadInGame();
    }

    public void LoadInGame()
    {
        JsonSaveFile saveFile = FindFirstObjectByType<JsonSaveFile>();
        saveFile.LoadGame();

        upgradeContainer.CheckUpgradeState();
    }
}
