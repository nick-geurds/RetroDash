using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public PlayerStatsScriptableObject playerStatsSO;

    [HideInInspector] public float attackAmount;
    [HideInInspector] public float maxHealth;
    [HideInInspector] public float dashDis;
    [HideInInspector] public float dashInterval;
    [HideInInspector] public float dashSpeed;

    private void Start()
    {
        attackAmount = playerStatsSO.attackAmount;
        maxHealth = playerStatsSO.maxHealth;
        dashDis = playerStatsSO.dashDis;
        dashInterval = playerStatsSO.dashInterval;
        dashSpeed = playerStatsSO.dashSpeed;
    }
}
