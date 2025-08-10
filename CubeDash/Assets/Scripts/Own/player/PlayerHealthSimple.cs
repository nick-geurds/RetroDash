using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSimple : MonoBehaviour
{
    public bool debugMode;

    public float currentHp;
    public float maxHp;

    public ParticleSystem partSystem;

    public Image[] hearts;
    public Sprite FullHeart, halfHeart, EmptyHeart;

    private Collider2D coll2D;
    private PlayerMovementSimple mMovement;
    private KnockBack knockBack;
    private SimpleShockWave shockWave;
    private SimpleCamareShake simpleCameraShake;


    private bool isAnimating = false;   

    private void Start()
    {
        mMovement = GetComponent<PlayerMovementSimple>();
        knockBack = GetComponent<KnockBack>();
        coll2D = GetComponent<Collider2D>();
        shockWave = GetComponent<SimpleShockWave>();

        simpleCameraShake = FindFirstObjectByType<SimpleCamareShake>();

        StartCoroutine(InitializePlayerData());
    }

    IEnumerator InitializePlayerData()
    {
        yield return null;
        maxHp = PlayerDataSimple.Instance.maxHealth;
        currentHp = maxHp;
        UpdateUI();
    }

    private void Update()
    {
        if (!debugMode)
            return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(.1f);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (mMovement.isDashing == false)
            {
                Vector2 direction = transform.position - collision.transform.position;

                TakeDamage(1);
                StartCoroutine(knockBack.DoKnockBack(direction));
            }
        }
    }

    private void TakeDamage(float amount)
    {

        currentHp -= amount;

        partSystem.Play();

        StartCoroutine(Iframes());

        if (simpleCameraShake != null)
        {
            StartCoroutine(simpleCameraShake.QuickShake(15f, .2f));
        }

        if (shockWave != null && shockWave.canShockWave == true)
        {
            StartCoroutine(shockWave.SchokWave());
        }
        else
        {
            Debug.LogWarning("Shockwave isnt active yet");
        }

        if (!isAnimating)
        {
            AnimateOnDamage();
        }

        UpdateUI();

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void AnimateOnDamage()
    {
        isAnimating = true;
        Vector3 orgscale = transform.localScale;

        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, orgscale * .5f, .5f).setEasePunch().setOnComplete(() =>
        {
            transform.localScale = orgscale;
            isAnimating = false;
        });
    }

    private void Die()
    {
        GameManager.Instance.GameOver();
    }

    private IEnumerator Iframes()
    {
        coll2D.enabled = false;
        yield return new WaitForSeconds(.5f);
        coll2D.enabled = true;
    }

    private void UpdateUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < maxHp)
            {
                float heartHP = currentHp - i;

                if (heartHP <= 0)
                {
                    hearts[i].sprite = EmptyHeart;
                }
                else if (heartHP <= .5f)
                {
                    hearts[i].sprite = halfHeart;
                }
                else if (heartHP >= .5f)
                {
                    hearts[i].sprite = FullHeart;
                }
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}
