using UnityEngine;
using UnityEngine.UI;

public class BossHealthSimple : MonoBehaviour
{
    [Header("Health settings")]
    public float currenthealth;
    public float maxHealth;

    private bool activatePhase2 = false;
    private SpriteRenderer sprite;
    private Color orgColor;

    
    private CanvasGroup healthBarUI;
    private Slider healthSlider;
    private Image bossAvatar;
    private Image fillArea;
    public Sprite bossSpriteUI;

    private Camera mainCam;
    private Vector3 orgScale;

    private void Start()
    {
        orgScale = transform.localScale;
        transform.localScale = Vector3.zero;
        mainCam = Camera.main;
        float orthoOrgSize = Camera.main.orthographicSize;

        LeanTween.value(orthoOrgSize, orthoOrgSize * 1.5f, 1f).setEaseInExpo().setOnUpdate((float val) =>
        {
            mainCam.orthographicSize = val;
        });

        healthBarUI = GameObject.Find("BossHealthUI").GetComponent<CanvasGroup>();
        healthBarUI.alpha = 1f;

        healthSlider = GameObject.Find("BossSlider").GetComponent<Slider>();
        //bossAvatar = GameObject.Find("BossAvatar").GetComponent<Image>();

        //bossAvatar.sprite = bossSpriteUI;
        healthSlider.maxValue = maxHealth;

        LeanTween.value(0, maxHealth , 1.5f).setOnUpdate((float val) =>
        {
            healthSlider.value = val;
        });

        currenthealth = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
        orgColor = sprite.color;

        LeanTween.scale(gameObject, orgScale, 3f).setEaseOutElastic();
    }

    public void TakeDamageBoss()
    {
        float damage = 1f; //tijdelijk

        currenthealth -= damage;

        healthSlider.value -= damage;

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
