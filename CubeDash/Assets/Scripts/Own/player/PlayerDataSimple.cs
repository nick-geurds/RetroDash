
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataSimple : MonoBehaviour
{
    public static PlayerDataSimple Instance;
    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    public delegate void OnPlayerDataChange(float maxDistance,
                                            float dashInterval,
                                            float dashDuration,
                                            float maxHealth,
                                            float attackAmount,
                                            List<int> upgradeIndex,
                                            bool canShockWave,
                                            bool canShootProjectiles);

    public static event OnPlayerDataChange OnDataChange;



    [Header("DashSettings")]
    public float maxDistance;
    public float dashInterval;
    public float dashDuration;

    [Header("HealthSettings")]
    public float maxHealth;

    [Header("AttackSettings")]
    public float attackAmount;

    [Header("DefenseSettings")]
    public bool canShockWave = false;
    public bool canShootProjectiles = false;

    [Header("UpgradesUnlockedData")]
    public List<int> UpgradeIndex = new List<int>();
    public void SetPlayerData(float maxDistance, 
                                float dashInterval, 
                                float dashDuration, 
                                float maxHealth, 
                                float attackAmount,
                                List<int> upgradeIndex,
                                bool canShockWave,
                                bool canShootProjectiles)
    {
        this.maxDistance = maxDistance;
        this.dashInterval = dashInterval;
        this.dashDuration = dashDuration;
        this.maxHealth = maxHealth;
        this.attackAmount = attackAmount;
        this.canShockWave = canShockWave;
        this.canShootProjectiles = canShootProjectiles;
        
        UpgradeIndex = new List<int>(upgradeIndex);

        OnDataChange?.Invoke(maxDistance,
                             dashInterval,
                             dashDuration,
                             maxHealth,
                             attackAmount,
                             upgradeIndex,
                             canShockWave,
                             canShootProjectiles);
    }

}
