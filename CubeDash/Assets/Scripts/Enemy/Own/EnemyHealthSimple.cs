using System.Collections;
using UnityEngine;

public class EnemyHealthSimple : MonoBehaviour
{
    [Header("HealthSettings")]
    public float currentHealth;
    public float maxHealth;
    public ParticleSystem hitParticles;


    [Header("DeathSettings")]
    public float duration = .5f;
    public float shakeAmount = .15f;

    public bool animOnDamage = true;

    private AnimOnDamage animationHit;
    private EnemyProjectileShooter projectileShooter;
    private Vector3 orgPos;

    public bool isMainEnemie = false;


    private void Start()
    {
        currentHealth = maxHealth;

        animationHit = GetComponent<AnimOnDamage>();
        projectileShooter = GetComponent<EnemyProjectileShooter>();

        
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

            if (projectileShooter != null && projectileShooter.onlyExplodeOnDeath == true)
            {
               StartCoroutine(ExplodeOnDeath());
            }
            else
            {
                 Die();
            }
        }
    }



    private IEnumerator ExplodeOnDeath()
    {
        orgPos = transform.position;

        LeanTween.value(gameObject, 0, 1, duration).setOnUpdate((float val) =>
        {
            float x = Random.Range(-shakeAmount, shakeAmount);
            float y = Random.Range(-shakeAmount, shakeAmount);

            transform.position = orgPos + new Vector3(x, y, 0);
        })
            .setOnComplete(() =>
        {
            transform.position = orgPos;
            projectileShooter.Spawn();
        });

        yield return new WaitForSeconds(duration);

        Die();
    }

    private void Die()
    {
        GameManager.Instance.RegisterEnemyKill();

        if (projectileShooter != null)
        {
            projectileShooter.ReturnAllProjectiles();
        }

        if (isMainEnemie)
            SimpleWaveManager.activeEnemies.Remove(gameObject);

        Destroy(gameObject);
    }
}
