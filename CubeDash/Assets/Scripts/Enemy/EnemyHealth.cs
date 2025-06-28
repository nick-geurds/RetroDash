using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("HealthSettings")]
    public float currentHealth;
    public float maxHealth;
    public ParticleSystem hitParticle;

    [Header("hit anim settings")]
    public float scaleAmount = .9f;
    public float scaleDur = .3f;


    private GameObject player;
    private PlayerMovement playerMovement;
    private PlayerStats playerStats;

    private Vector3 orgScale;

    private void Start()
    {
        player = GameObject.Find("Player");
        playerStats = player.GetComponent<PlayerStats>();
        playerMovement = player.GetComponent<PlayerMovement>();

        currentHealth = maxHealth;

        orgScale = transform.localScale;

        EnemySpawnManager.activeEnemies.Add(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerMovement.isDashing == true)
        {
            TakeDamage(playerStats.attackAmount);
            hitParticle.Play();
        }
    }

    public void TakeDamage(float amuount)
    {
        currentHealth -= amuount;
        LeanTween.scale(gameObject, orgScale * scaleAmount, scaleDur).setEasePunch();

        if (currentHealth <= 0)
        {
            Die();
            
        }
    }

    void Die()
    {
        LeanTween.scale(gameObject, new Vector3(0, 0, 0), .3f).setEaseSpring().setOnComplete(() =>
        {
            EnemySpawnManager.activeEnemies.Remove(gameObject);
            Destroy(gameObject);
        });
        
    }
}
