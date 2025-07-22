using System.Collections;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BossHealth : EnemyHealth
{
    [Header("UI")]
    public GameObject healthBarUI;           // De hele healthbar (bijv. Panel)
    public Slider healthSlider;              // Slider component
    public Image sliderBG;
    public Image bossAvatar;
    public Image bossAvatarBG;
    private Tilemap tilemapBG;
    public Color bossColor;

    public int XpGain;

    public float durationSec = 4f;

    private Camera mainCam;

    public Sprite bossAvatarSprite;
    private bool isDead = false;
    private Color orgTileColor;

    private bool isPhase2 = false;
    public bool isPhase2TransitionActive = false;
    public bool isPhase2Ready = false;



    private void Start()
    {
        currentHealth = maxHealth;  //  deze moet eigenlijk NIET hier staan
        Debug.Log($"[BossHealth] Start: BEFORE setting UI, currentHealth = {currentHealth}");

        if (healthBarUI == null)
            healthBarUI = GameObject.Find("BossHealthUI"); // <-- vervang dit met de echte naam in je scene

        if (healthSlider == null)
            healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>(); // specifieker: zoek op tag of naam als nodig

        if (sliderBG == null)
            sliderBG = GameObject.Find("SliderBG")?.GetComponent<Image>();

        if (bossAvatar == null)
            bossAvatar = GameObject.Find("BossAvatar")?.GetComponent<Image>();

        if (bossAvatarBG == null)
            bossAvatarBG = GameObject.Find("BossAvatarBG")?.GetComponent<Image>();

        if (tilemapBG == null)
            tilemapBG = GameObject.Find("BGBackground")?.GetComponent<Tilemap>();

        CanvasGroup group = healthBarUI.GetComponent<CanvasGroup>();
        group.alpha = 1f;
        group.interactable = false;
        group.blocksRaycasts = false;


        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        tilemapBG = GameObject.Find("BGBackground").GetComponent<Tilemap>();

        orgTileColor = tilemapBG.color;

        if (sliderBG != null)
            sliderBG.color = bossColor;

        if (bossAvatarBG != null)
            bossAvatarBG.color = bossColor;

        if (bossAvatar != null)
            bossAvatar.sprite = bossAvatarSprite;

        if (tilemapBG != null)
            tilemapBG.color = (bossColor - new Color(0,0,0,0.9f));

        AnimateCameraZoom(10f, 15f, 0.8f);


    }



    public void UpdateHealthUI()
    {
        if (healthSlider != null)
            StartCoroutine(SmoothHealthChange(healthSlider.value, currentHealth));

        
    }

    private IEnumerator SmoothHealthChange(float from, float to)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            healthSlider.value = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        healthSlider.value = to;

        if (!isPhase2TransitionActive && !isPhase2Ready && currentHealth <= maxHealth * 0.5f && !readIsTweeingScale)
        {
            StartCoroutine(Phase2Transition());
        }
    }

    protected override void Die()
    {
        if (isDead) return; // Vermijd dubbele calls
        isDead = true;

        StopAllCoroutines(); // stopt bv. Phase2Transition()
        LeanTween.cancel(gameObject); // Stop eventuele shakes

        if (healthBarUI != null)
            healthBarUI.SetActive(false);

        AnimateCameraZoom(15f, 10f, 0.8f);
        tilemapBG.color = orgTileColor;

        CanvasGroup group = healthBarUI.GetComponent<CanvasGroup>();
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

        EXPManager.instance.AddEXP(XpGain);

        EnemySpawnManager.activeEnemies.Remove(gameObject);
        GameManager.Instance.RegisterEnemyKill();

        Destroy(gameObject);
    }

    private void AnimateCameraZoom(float from, float to, float duration)
    {
        LeanTween.value(from, to, duration).setOnUpdate((float val) =>
        {
            Camera.main.orthographicSize = val;
        });
    }

    private IEnumerator Phase2Transition()
    {
        if (isDead) yield break;

        isPhase2TransitionActive = true;

        SpriteRenderer bossSprite = gameObject.GetComponent<SpriteRenderer>();
        Color startColor = bossSprite.color;
        Color targetColor = new Color (1f, 0f, 0f, 0.2f);

        float duration = durationSec;
        float elapsed = 0f;

        // Start de shake op positie
        Vector3 originalPos = transform.position;
        LeanTween.cancel(gameObject); // Cancel eventuele andere tweens

        LeanTween.moveLocalX(gameObject, originalPos.x + 0.3f, 0.1f).setLoopPingPong(50).setEase(LeanTweenType.easeShake);
        LeanTween.moveLocalY(gameObject, originalPos.y + 0.3f, 0.1f).setLoopPingPong(50).setEase(LeanTweenType.easeShake);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Lerp kleur
            bossSprite.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        // Stop shake en reset positie
        LeanTween.cancel(gameObject);
        transform.position = originalPos;
        bossSprite.color = targetColor;

        // Hier evt. phase2 ready event triggeren
        yield return new WaitForSeconds(durationSec);

        isPhase2TransitionActive = false;
        isPhase2Ready = true;
    }

}
