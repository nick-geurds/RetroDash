using System.Collections;
using UnityEngine;

public class SytheSlashAttack : MonoBehaviour, ISpawnHittable
{
    [Header("Timing")]
    public float anticipationTime = 0.4f;
    public float slashDuration = 0.15f;

    [Header("Rotatie-instellingen")]
    public float rotationStartAngle = 15f;
    public float rotationEndAngle = -15f;
    public bool allowRandomFlip = true;

    [Header("Debug/State")]
    public bool isHittable = false;          // Wordt true alleen als dit object hittable is
    public bool willBeHittable = false;      // Wordt gezet door de spawner, true alleen voor laatste sythe

    private bool flipped = false;
    private SpriteRenderer sprite;

    private BossHealth bosshealth;
    private PlayerStats playerstats;

    private Rigidbody2D rb;

    public void SetWillBeHittable(bool value)
    {
        willBeHittable = value;
    }

    public void SetHittable(bool value)
    {
        isHittable = value;
    }

    public bool IsHittable()
    {
        return isHittable;
    }

    // Deze methode is toegevoegd om de anticipatietijd in te stellen
    public void SetAnticipationTime(float value)
    {
        anticipationTime = value;
    }

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();

        bosshealth = GameObject.FindWithTag("Boss").GetComponent<BossHealth>();
        playerstats = GameObject.Find("Player").GetComponent<PlayerStats>();

        flipped = false;
        if (allowRandomFlip && Random.value > 0.5f)
        {
            FlipObject();
        }

        float startZ = flipped ? -rotationStartAngle : rotationStartAngle;
        transform.rotation = Quaternion.Euler(0f, 0f, startZ);

        // Zet hittable meteen aan (ook tijdens anticipation)
        isHittable = willBeHittable;
        if (isHittable)
        {
            Hittable();
        }

        StartCoroutine(SlashSequence());
    }

    private void FlipObject()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
        flipped = true;
    }

    private IEnumerator SlashSequence()
    {
        yield return new WaitForSeconds(anticipationTime);

        // Geen aanpassing hier meer nodig
        float startAngle = flipped ? -rotationStartAngle : rotationStartAngle;
        float endAngle = flipped ? -rotationEndAngle : rotationEndAngle;

        float elapsed = 0f;
        while (elapsed < slashDuration)
        {
            float zAngle = Mathf.Lerp(startAngle, endAngle, elapsed / slashDuration);
            transform.rotation = Quaternion.Euler(0f, 0f, zAngle);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, endAngle);

        isHittable = false;

        Destroy(gameObject, 0.5f);
    }

    private void Hittable()
    {
        if (sprite != null)
            sprite.color = Color.red;
    }

    // Verander dit naar OnTriggerEnter2D
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isHittable) return;

        if (collider.CompareTag("Player") && PlayerIsDashing(collider.gameObject) ||
            collider.CompareTag("PlayerBullets"))
        {
            if (bosshealth != null)
            {
                bosshealth.TakeDamage(playerstats.attackAmount);
                Debug.Log("Boss Took Damage via Sythe");
                if (bosshealth.hitParticle != null)
                    bosshealth.hitParticle.Play();
            }
        }
    }

    private bool PlayerIsDashing(GameObject player)
    {
        var movement = player.GetComponent<PlayerMovement>();
        return movement != null && movement.isDashing;
    }
}
