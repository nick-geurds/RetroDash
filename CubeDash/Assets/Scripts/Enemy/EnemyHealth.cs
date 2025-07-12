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
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerStats playerStats;

    private Vector3 orgScale;



    private void Start()
    {
        player = GameObject.Find("Player");
        playerStats = player.GetComponent<PlayerStats>();
        playerMovement = player.GetComponent<PlayerMovement>();

        currentHealth = maxHealth;
        Debug.Log($"[EnemyHealth] Start: currentHealth set to {currentHealth}");

        orgScale = transform.localScale;

        EnemySpawnManager.activeEnemies.Add(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerMovement.isDashing == true && collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerBullets"))
        {
            TakeDamage(playerStats.attackAmount);
            hitParticle.Play();
        }
    }

    public void TakeDamage(float amuount)
    {
        currentHealth -= amuount;
        LeanTween.scale(gameObject, orgScale * scaleAmount, scaleDur).setEasePunch();

        BossHealth bossHealth = this as BossHealth;
        if (bossHealth != null)
            bossHealth.UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
            
        }
    }

    protected virtual void Die()
    {
        EnemySpawnManager.activeEnemies.Remove(gameObject);
        LeanTween.scale(gameObject, new Vector3(0, 0, 0), .3f).setEaseSpring().setOnComplete(() =>
        {
            GameManager.Instance.RegisterEnemyKill();
            Destroy(gameObject);
        });
    }
}
