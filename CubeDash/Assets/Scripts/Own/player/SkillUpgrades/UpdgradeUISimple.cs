using UnityEngine;

public class UpdgradeUISimple : MonoBehaviour
{
    private PlayerDataSimple playerData;
    private void Start()
    {
        playerData = FindAnyObjectByType<PlayerDataSimple>();
    }

    public void ApplyUpgrade(UpgradesSimple upgrades)
    {
        upgrades.Upgrade(playerData);
    }
}
