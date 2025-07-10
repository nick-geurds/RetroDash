using System.Collections;
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
    public Tilemap tilemapBG;
    public Color bossColor;

    private Camera mainCam;

    public Sprite bossAvatarSprite;
    private bool isDead = false;



    private void Start()
    {
        currentHealth = maxHealth;  //  deze moet eigenlijk NIET hier staan
        Debug.Log($"[BossHealth] Start: BEFORE setting UI, currentHealth = {currentHealth}");

        if (healthBarUI != null)
            healthBarUI.SetActive(true);

        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

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
    }

    protected override void Die()
    {
        if (healthBarUI != null)
            healthBarUI.SetActive(false);

        AnimateCameraZoom(15f, 10f, 0.8f);

        base.Die();
    }

    private void AnimateCameraZoom(float from, float to, float duration)
    {
        LeanTween.value(from, to, duration).setOnUpdate((float val) =>
        {
            Camera.main.orthographicSize = val;
        });
    }

}
