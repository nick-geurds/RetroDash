using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private PlayerStats stats;

    [Header("health settings")]
    public float currentHealth;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    private PlayerMovement playerMovement;

    public float dmgAmount = 1f;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        stats = GetComponent<PlayerStats>(); //  Haal PlayerStats component op

        currentHealth = stats.maxHealth;
    }

    private void Update()
    {
        if (currentHealth > stats.maxHealth)
        {
            currentHealth = stats.maxHealth;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < stats.maxHealth)
            {
                float heartHP = currentHealth - i;

                if (heartHP >= 1f)
                    hearts[i].sprite = fullHeart;
                else if (heartHP >= .5f)
                    hearts[i].sprite = halfHeart;
                else
                    hearts[i].sprite = emptyHeart;

                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!playerMovement.isDashing && !playerMovement.cantKnockBack && playerMovement.canTakeDamage)
            {
                TakeDamage(dmgAmount);

                Vector2 direction = (transform.position - collision.transform.position).normalized;
                float force = playerMovement.knockBackForce;
                float duration = .2f;

                if (playerMovement.knockbackRoutine != null)
                    StopCoroutine(playerMovement.knockbackRoutine);

                playerMovement.knockbackRoutine = StartCoroutine(playerMovement.Knockback(direction, force, duration));
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Debug.Log("player died");
            playerMovement.enabled = false;
        }
    }
}
