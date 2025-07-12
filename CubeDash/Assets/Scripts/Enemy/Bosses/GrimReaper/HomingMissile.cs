using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingMissile : MonoBehaviour , ISpawnHittable
{
    public float speed = 5f;
    public float turnSpeed = 200f; // graden per seconde
    public float lifetime = 5f;

    public SpriteRenderer sprite;
    private Rigidbody2D rb;
    private Transform player;

    private PlayerStats playerstats;
    private BossHealth bosshealth;

    public bool isHittable = false;          // Wordt true alleen als dit object hittable is
    public bool willBeHittable = false;      // Wordt gezet door de spawner, true alleen voor laatste sythe

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


    private void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        playerstats = playerObj.GetComponent<PlayerStats>();
        bosshealth = GameObject.FindWithTag("Boss").GetComponent<BossHealth>();
        rb = GetComponent<Rigidbody2D>();

        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log("Player transform found on homing");

            // Richting naar speler vanaf het begin
            Vector2 initialDirection = (player.position - transform.position).normalized;
            rb.linearVelocity = initialDirection * speed;
        }
        else
        {
            Debug.Log("No player found");
        }

        isHittable = willBeHittable;
        if (isHittable)
        {
            Hittable();
        }

        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        // Richting naar de speler
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        // Bepaal huidige richting (velocity of voorwaartse richting)
        Vector2 currentDirection = rb.linearVelocity.normalized;

        // Bereken hoek tussen huidige richting en doelrichting
        float angle = Vector2.SignedAngle(currentDirection, directionToPlayer);

        // Bepaal hoeveel graden we dit frame mogen draaien
        float maxRotationThisFrame = turnSpeed * Time.fixedDeltaTime;
        float clampedAngle = Mathf.Clamp(angle, -maxRotationThisFrame, maxRotationThisFrame);

        // Draai huidige richting
        Vector2 newDirection = Quaternion.Euler(0, 0, clampedAngle) * currentDirection;

        // Zet velocity in nieuwe richting
        rb.linearVelocity = newDirection.normalized * speed;

        // Optioneel: draai sprite naar richting
        float angleToRotate = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angleToRotate);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isHittable)
        {
            if ((collision.gameObject.CompareTag("Player") && PlayerIsDashing(collision.gameObject)) ||
                collision.gameObject.CompareTag("PlayerBullets"))
            {
                if (bosshealth != null)
                {
                    bosshealth.TakeDamage(playerstats.attackAmount);
                    Debug.Log("Boss Took Damage via Sythe");
                    if (bosshealth.hitParticle != null)
                        bosshealth.hitParticle.Play();
                    Destroy(gameObject);
                }
            }

        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Hittable()
    {
        if (sprite != null)
            sprite.color = Color.red;
    }

    private bool PlayerIsDashing(GameObject player)
    {
        var movement = player.GetComponent<PlayerMovement>();
        return movement != null && movement.isDashing;
    }
}
