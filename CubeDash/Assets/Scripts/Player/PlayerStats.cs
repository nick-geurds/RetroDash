using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static event Action OnPlayerStatsInitialized;

    public PlayerStatsScriptableObject playerStatsSO;

    [Header("PlayerStats")]
    public float attackAmount;
    public float maxHealth;
    public float dashDis;
    public float dashInterval;
    public float dashSpeed;

    [Header("Damage Projectile Settings")]
    [HideInInspector] public bool fireProjectilesOnDamage;
    [HideInInspector] public int damageProjectileCount;
    [HideInInspector] public GameObject damageProjectilePrefab;
    [HideInInspector] public float damageProjectileSpeed;

    [Header("Shockwave Settings")]
    public bool enableShockWave;
    public float shockwaveRadius;
    public float shockwaveStunDuration;
    public float stunDuration;

    private void Awake()
    {
        ResetToBaseStats();
        Debug.Log("[PlayerStats] Base stats set in Awake. maxHealth = " + maxHealth);
    }

    private void Start()
    {
        if (UnlockedUpgradeData.Instance != null)
        {
            UnlockedUpgradeData.Instance.ApplyTo(this);
            Debug.Log("[PlayerStats] Upgrades applied in Start. Final maxHealth = " + maxHealth);
        }
        else
        {
            Debug.LogWarning("[PlayerStats] UnlockedUpgradeData instance not found in Start()");
        }

        // Trigger event dat PlayerStats klaar zijn om gebruikt te worden
        OnPlayerStatsInitialized?.Invoke();
    }

    public void ResetToBaseStats()
    {
        attackAmount = playerStatsSO.attackAmount;
        maxHealth = playerStatsSO.maxHealth;
        dashDis = playerStatsSO.dashDis;
        dashInterval = playerStatsSO.dashInterval;
        dashSpeed = playerStatsSO.dashSpeed;

        fireProjectilesOnDamage = playerStatsSO.fireProjectilesOnDamage;
        damageProjectileCount = playerStatsSO.damageProjectileCount;
        damageProjectileSpeed = playerStatsSO.damageProjectileSpeed;
        damageProjectilePrefab = playerStatsSO.damageProjectilePrefab;

        stunDuration = playerStatsSO.stunDuration;
        shockwaveRadius = playerStatsSO.shockwaveRadius;
        shockwaveStunDuration = playerStatsSO.shockwaveStunDuration;
        Debug.Log("[PlayerStats] Stats reset to base: maxHealth = " + maxHealth);
    }
}
