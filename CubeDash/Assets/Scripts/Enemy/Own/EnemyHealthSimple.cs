using UnityEngine;

public class EnemyHealthSimple : MonoBehaviour
{
    [Header("HealthSettings")]
    public float currentHealth;
    public float maxHealth;
    public ParticleSystem hitParticles;

    [Header("EnemyType")]
    public bool isBoss = false;

    public bool animOnDamage = true;

    private AnimOnDamage animationHit;



    private void Start()
    {
        currentHealth = maxHealth;

        animationHit = GetComponent<AnimOnDamage>();

        EnemySpawnManager.activeEnemies.Add(gameObject);
    }

    public void TakeDamage(float damageAmount)
    {
        if (animOnDamage)
        {
            animationHit.ScaleOnDamage();
        }

        if (hitParticles != null)
        {
            hitParticles.Play();
        }

        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            EnemySpawnManager.activeEnemies.Remove(gameObject);
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
