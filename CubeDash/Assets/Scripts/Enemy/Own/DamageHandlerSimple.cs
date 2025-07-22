using UnityEngine;


public class DamageHandlerSimple : MonoBehaviour
{

    private EnemyHealthSimple enemyHealth;

    private Collider2D collider2d;

    private GameObject player;
    private PlayerMovement playerMov;
    private PlayerStats playerStats;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealthSimple>();
        player = GameObject.Find("Player");
        playerMov = player.GetComponent<PlayerMovement>();
        playerStats = player.GetComponent<PlayerStats>();   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (playerMov != null && playerMov.isDashing)
        {
            enemyHealth.TakeDamage(playerStats.attackAmount);
        }
    }
}
