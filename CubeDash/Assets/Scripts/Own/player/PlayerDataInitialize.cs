using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDataInitialize
{
    public float maxDistance;
    public float dashInterval;
    public float dashDuration;

    public float maxHealth;

    public float attackAmount;

    public List<int> UpgradeIndex = new List<int>();

    public bool canShockWave;
    public bool canShootProjectiles;
}
