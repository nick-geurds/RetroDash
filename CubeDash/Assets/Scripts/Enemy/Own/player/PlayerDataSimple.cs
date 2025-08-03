using UnityEngine;

public class PlayerDataSimple : MonoBehaviour
{
    public static PlayerDataSimple Instance;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    [Header("DashSettings")]
    public float maxDistance;
    public float dashInterval;
    public float dashDuration;

    [Header("HealthSettings")]
    public float maxHealth;

    [Header("AttackSettings")]
    public float attackAmount;
}
