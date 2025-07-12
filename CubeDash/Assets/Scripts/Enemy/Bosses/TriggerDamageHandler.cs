using UnityEngine;

public class TriggerDamageHandler : MonoBehaviour
{
    [Header("Settings")]
    public string targetTag = "Player";  // De tag van het object dat kan worden geraakt
    public bool isHittable = false;     // Bepaalt of het object in staat is om schade te doen

    private BossHealth bossHealth;      // De referentie naar de BossHealth component
    private PlayerStats playerStats;    // De referentie naar de PlayerStats component

    private Vector3 orgScale;

    private void Start()
    {
        // Zoek naar de relevante componenten
        bossHealth = GameObject.FindWithTag("Boss")?.GetComponent<BossHealth>();
        playerStats = GameObject.Find("Player")?.GetComponent<PlayerStats>();

        orgScale = transform.localScale;
    }

    // Deze methode kan extern worden aangeroepen om de hittable status in te stellen
    public void SetHittableStatus(bool value)
    {
        isHittable = value;
    }

    // Deze methode controleert of de speler dashes en of het object hittable is
    private bool PlayerIsDashing(GameObject player)
    {
        var movement = player.GetComponent<PlayerMovement>();
        return movement != null && movement.isDashing;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Controleer of het object hittable is en of de collider voldoet aan de voorwaarden
        if (!isHittable) return;

        if (collider.CompareTag(targetTag)) // Check of het de speler is
        {
            // Als de speler aan het dashen is of de collider een ander object is (bijv. projectielen)
            if (PlayerIsDashing(collider.gameObject) || collider.CompareTag("PlayerBullets"))
            {
                if (bossHealth != null && playerStats != null)
                {
                    LeanTween.scale(gameObject, orgScale * .3f, .5f).setEasePunch();
                    // Roep TakeDamage aan, gebaseerd op de player stats (bijv. aanvalsschade)
                    bossHealth.TakeDamage(playerStats.attackAmount);
                    Debug.Log("Boss Took Damage via TriggerDamageHandler");

                    // Als er een hit particle is, speel die dan af
                    if (bossHealth.hitParticle != null)
                    {
                        bossHealth.hitParticle.Play();
                    }
                }
            }
        }
    }
}
