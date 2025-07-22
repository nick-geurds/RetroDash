using System;
using UnityEngine;

public class ShockwaveRing : MonoBehaviour
{
    public float expandDuration = 1f;

    private PlayerStats playerStats;
    private BossHealth bossHealth;

    private Action onComplete;
    private Vector3 originalScale;
    private bool hasDealtDamage = false;

    private TriggerDamageHandler triggerDamageHandler;

    void Awake()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
        bossHealth = GameObject.FindGameObjectWithTag("Boss")?.GetComponent<BossHealth>();
        triggerDamageHandler = GetComponent<TriggerDamageHandler>();

        originalScale = transform.localScale;
        transform.localScale = Vector3.zero; // begin als onzichtbare shockwave
    }

    public void Initialize(float radius, Action onFinished = null)
    {
        onComplete = onFinished;

        Vector3 targetScale = Vector3.one * radius * 2f; // diameter
        LeanTween.scale(gameObject, targetScale, expandDuration)
            .setEaseOutQuad()
            .setOnComplete(() =>
            {
                onComplete?.Invoke();
                Destroy(gameObject);
            });
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Collider2D colliders = other.GetComponent<Collider2D>();
        colliders.enabled = false;
       
    }
}
