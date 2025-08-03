using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
        
    }

    private SimpleWaveManager waveManager;
    private GameObject player;
    private PlayerHealthSimple playerHealth;

    [Header("TextSettings")]
    public TMP_Text waveText;
    public float scaleWavDuration;
    public LeanTweenType waveStart = LeanTweenType.easeInElastic;
    public LeanTweenType waveEnd = LeanTweenType.easeOutElastic;
    private Vector3 startWaveScale = new Vector3(0.2f, 0.2f, 0);
    private Vector3 orgWaveScale = new Vector3(1, 1, 1);

    [Header("GameOverScreenSettings")]
    public GameObject GameOverPanel;
    public TMP_Text enemiesKO;
    public TMP_Text playtimeFinal;
    public TMP_Text waveReached;
    public TMP_Text totalScoreText;
    public TMP_Text BossesKO;

    

    private float playTime;
    private int enemiesKilled;
    private int bossKilled;

    private bool gameIsOver = false;

    [HideInInspector] public bool isGameOver = false;

    
    private IEnumerator Start()
    {
        waveManager = FindFirstObjectByType<SimpleWaveManager>();
        waveText.text = $"WAVE " + (waveManager.currentWaveIndex + 1).ToString();
        player = GameObject.Find("Player");

        // Wacht 1 frame zodat alle componenten zijn geïnitialiseerd
        yield return null;

        playerHealth = player.GetComponent<PlayerHealthSimple>();

        waveText.enabled = false;
        playTime = 0f;
        enemiesKilled = 0;
        bossKilled = 0;

        StartCoroutine(ShowWaveText(3));

        Time.timeScale = 1f;
    }

    private void Update()
    {

        if (playerHealth != null && playerHealth.currentHp > 0)
        {
            playTime += Time.deltaTime;
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        int totalSeconds = Mathf.FloorToInt(playTime); // afronden naar beneden

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        string formattedTime = string.Format("{0:D2}:{1:D2}", minutes, seconds);

        gameIsOver = true;
        
        GameOverPanel.SetActive(true);


        int timeScore = Mathf.FloorToInt(playTime / 5f); // 1 EXP per 5 sec
        int killScore = enemiesKilled * 10;              // 10 EXP per kill


        int totalEarned = timeScore + killScore;

        BossesKO.text = bossKilled.ToString();
        totalScoreText.text = totalEarned.ToString();
        waveReached.text = (waveManager.currentWaveIndex + 1).ToString();
        playtimeFinal.text = formattedTime.ToString();
        enemiesKO.text = enemiesKilled.ToString();

        EXPManager.instance.AddEXP(totalEarned);
        Debug.Log($"Game Over! Gained EXP: {totalEarned}");

        Time.timeScale = 0f;
    }



    public IEnumerator ShowWaveText(float TimeToShow)
    {
        waveText.enabled = true;
        waveText.transform.localScale = startWaveScale;
        waveText.text = $"WAVE " + (waveManager.currentWaveIndex + 1).ToString();


        LeanTween.cancel(waveText.gameObject);
        LeanTween.scale(waveText.gameObject, orgWaveScale, scaleWavDuration).setEase(waveStart);



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

    public void RegisterBossKill()
    {
        bossKilled++;
        Debug.Log($"Enemy killed. Total: {enemiesKilled}");
    }
}
