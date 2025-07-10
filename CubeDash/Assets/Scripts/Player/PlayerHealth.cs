using System.Collections;
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
    private SpriteRenderer sprite;
    private Color orgColor;


    

    private PlayerMovement playerMovement;
    

    public float dmgAmount = 1f;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        playerMovement = GetComponent<PlayerMovement>();

        

        sprite = GetComponent<SpriteRenderer>();
        orgColor = sprite.color;
        
    }

    private void OnEnable()
    {
        PlayerStats.OnPlayerStatsInitialized += InitializeHealth;
    }

    private void OnDisable()
    {
        PlayerStats.OnPlayerStatsInitialized -= InitializeHealth;
    }

    private void InitializeHealth()
    {
        currentHealth = stats.maxHealth;
        Debug.Log("[PlayerHealth] currentHealth initialized to stats.maxHealth = " + currentHealth);
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
        StartCoroutine(ColorChange());
        currentHealth -= amount;

       

        if (currentHealth <= 0)
        {
            Debug.Log("player died");
            playerMovement.enabled = false;
        }
    }

    IEnumerator ColorChange()
    {
        yield return new WaitForSeconds(0.05f);
        sprite.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        sprite.color = orgColor;
    }
}
