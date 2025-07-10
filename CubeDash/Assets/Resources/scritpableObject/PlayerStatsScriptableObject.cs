using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "New PlayerSo", menuName = "Player")]
public class PlayerStatsScriptableObject : ScriptableObject

{
    [Header("PlayerStats")]
    public float attackAmount = 1f;
    public float maxHealth = 5;
    public float dashDis = 8f;
    public float dashInterval = 1f;
    public float dashSpeed = 50f;

    [Header("Damage Projectile Settings")]
    public bool fireProjectilesOnDamage = false;
    public int damageProjectileCount = 8; // aantal projectiles rondom speler
    public GameObject damageProjectilePrefab;
    public float damageProjectileSpeed = 5f;

    [Header("Shockwave Settings")]
    public float shockwaveRadius = 2f;
    public float stunDuration = 2f;
    public float shockwaveStunDuration = 1f;
    public bool enableShockWave = false;
}
