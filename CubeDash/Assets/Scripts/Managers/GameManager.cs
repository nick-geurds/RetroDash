using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
            Instance = this;
        
    }

    public EnemySpawnManager enemySpawnManager;
    private GameObject player;
    private PlayerHealth playerHealth;

    [Header("TextSettings")]
    public TMP_Text waveText;
    public float scaleWavDuration;
    public LeanTweenType waveStart = LeanTweenType.easeInElastic;
    public LeanTweenType waveEnd = LeanTweenType.easeOutElastic;
    private Vector3 startWaveScale = new Vector3(0.2f, 0.2f, 0);
    private Vector3 orgWaveScale = new Vector3(1, 1, 1);

    [Header("GameOverScreenSettings")]
    public TMP_Text waveTextFinal;
    public GameObject GameOverPanel;
    public TMP_Text playtimeFinal;

    

    private float playTime;
    private int enemiesKilled;

    private bool gameIsOver = false;

    private void Start()
    {
        waveText.text = $"WAVE " + (enemySpawnManager.currentWaveIndex + 1).ToString();
        player = GameObject.Find("Player");
        playerHealth = player.GetComponent<PlayerHealth>();

        waveText.enabled = false;

        playTime = 0f;
        enemiesKilled = 0;

        StartCoroutine(ShowWaveText(3));
        
    }

    private void Update()
    {

        if (playerHealth != null && playerHealth.currentHealth > 0)
        {
            playTime += Time.deltaTime;
        }

        if (!gameIsOver && playerHealth != null && playerHealth.currentHealth <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        if (gameIsOver) return;

        int totalSeconds = Mathf.FloorToInt(playTime); // afronden naar beneden

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        string formattedTime = string.Format("{0:D2}:{1:D2}", minutes, seconds);

        gameIsOver = true;
        
        GameOverPanel.SetActive(true);

        waveTextFinal.GetComponent<TMPTagAnimator>().SetAnimatedText("{shake}" + waveText.text + "{/shake}");

        int timeScore = Mathf.FloorToInt(playTime / 5f); // 1 EXP per 5 sec
        int killScore = enemiesKilled * 10;              // 10 EXP per kill

        playtimeFinal.GetComponent<TMPTagAnimator>().SetAnimatedText("{wave}" + formattedTime + "{/wave}");

        int totalEarned = timeScore + killScore;

        EXPManager.instance.AddEXP(totalEarned);
        Debug.Log($"Game Over! Gained EXP: {totalEarned}");

        Time.timeScale = 0f;
    }



    public IEnumerator ShowWaveText(float TimeToShow)
    {
        waveText.enabled = true;
        waveText.transform.localScale = startWaveScale;


        LeanTween.cancel(waveText.gameObject);
        LeanTween.scale(waveText.gameObject, orgWaveScale, scaleWavDuration).setEase(waveStart);

        waveText.text = $"WAVE " + (enemySpawnManager.currentWaveIndex + 1).ToString();


        yield return new WaitForSeconds(TimeToShow);
        

        LeanTween.cancel(waveText.gameObject);
        LeanTween.scale(waveText.gameObject, new Vector3(0,0,0), scaleWavDuration).setEase(waveEnd);

        yield return new WaitForSeconds(scaleWavDuration);
        waveText.enabled = false;
    }

    public void RegisterEnemyKill()
    {
        enemiesKilled++;
        Debug.Log($"Enemy killed. Total: {enemiesKilled}");
    }
}
