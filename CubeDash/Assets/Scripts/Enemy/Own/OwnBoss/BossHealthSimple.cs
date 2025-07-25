using UnityEngine;

public class BossHealthSimple : MonoBehaviour
{
    [Header("Health settings")]
    public float currenthealth;
    public float maxHealth;

    private bool activatePhase2 = false;
    private SpriteRenderer sprite;
    private Color orgColor;

    private void Start()
    {
        currenthealth = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
        orgColor = sprite.color;
    }

    public void TakeDamageBoss()
    {
        float damage = 1f; //tijdelijk

        currenthealth -= damage;

        if (currenthealth <= (maxHealth / 2))
        {
            activatePhase2 = true;
            sprite.color = Color.red;
        }

        if (currenthealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
