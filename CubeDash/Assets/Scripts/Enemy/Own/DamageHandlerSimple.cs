using UnityEngine;


public class DamageHandlerSimple : MonoBehaviour
{
    [Header("Boss settings")]
    public bool isForBoss = false;
    public bool isHittable = false;

    private EnemyHealthSimple enemyHealth;

    private Collider2D collider2d;

    private GameObject player;
    private PlayerMovement playerMov;
    private PlayerStats playerStats;


    private GameObject boss;
    private BossHealthSimple bossHealth;

    private SpriteRenderer sprite;
    private Color orgColor;

    private void Start()

    {
        player = GameObject.Find("Player");
        playerMov = player.GetComponent<PlayerMovement>();
        playerStats = player.GetComponent<PlayerStats>();   

        sprite = GetComponent<SpriteRenderer>();
        orgColor = sprite.color;

        if (!isForBoss)
        {
            enemyHealth = GetComponent<EnemyHealthSimple>();
        }
        else
        {
            boss = GameObject.FindGameObjectWithTag("Boss");
            bossHealth = boss.GetComponent<BossHealthSimple>();

            if (isHittable)
            {
                sprite.color = Color.red;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (playerMov != null && playerMov.isDashing)
        {
            if (isForBoss)
            {
                if (isHittable)
                {
                    bossHealth.TakeDamageBoss();

                }
            }
            else
            {
                enemyHealth.TakeDamage(playerStats.attackAmount);
            }
        }
    }
}
