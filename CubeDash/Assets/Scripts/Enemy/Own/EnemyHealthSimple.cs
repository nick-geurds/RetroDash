using UnityEngine;

public class EnemyHealthSimple : MonoBehaviour
{
    [Header("HealthSettings")]
    public float currentHealth;
    public float maxHealth;

    [Header("EnemyType")]
    public bool isBoss = false;


    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage()
    {

    }
}
