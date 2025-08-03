using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class SimpleWaves 
{
    public GameObject[] enemiesThatCanSpawn;
    public int numberOfEnemiesToSpawn;
    public float timeBetweenSpawns;
    public float timeInBetweenWaves;

    public bool isBossWave;
    
}
public class SimpleWaveManager : MonoBehaviour
{
    public SimpleWaves[] waves;
    public static List<GameObject> activeEnemies = new List<GameObject>();

    private SimpleWaves simpleWaves;
    public int currentWaveIndex = 0;

    //spawn logica
    private bool canSpawn;
    private float spawnTimer;
    private float TimeBetweenWaveTimer;

    private int enemiesLeftToSpawn;
    private bool spawnNewWave = true;

    private GameObject player;
    private BoxCollider2D mapBounds;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        player = GameObject.Find("Player");
        mapBounds = GameObject.Find("MapBounds").GetComponent<BoxCollider2D>();

        activeEnemies.Clear();

        ResetWaves();
    }

    private void ResetWaves()
    {
        currentWaveIndex = 0;
        spawnTimer = 0;
        TimeBetweenWaveTimer = 0;

        StartWave(currentWaveIndex);
    }

    private void StartWave(int index)
    {
        if (index < 0 || index >= waves.Length)
        {
            Debug.Log("no more waves");
            canSpawn = false;
            return;
        }

        simpleWaves = waves[index];
        enemiesLeftToSpawn = simpleWaves.numberOfEnemiesToSpawn;
        canSpawn = true;
        spawnTimer = 0;
        TimeBetweenWaveTimer = 0;
    }

    private void Update()
    {
        if (currentWaveIndex < 0 || currentWaveIndex > waves.Length)
            return;

        if (canSpawn)
        {
            TimeBetweenWaveTimer += Time.deltaTime;

            if (TimeBetweenWaveTimer > simpleWaves.timeInBetweenWaves)
                StartCoroutine(SpawnWave());
        }
    }

    private IEnumerator SpawnWave()
    {
        if (GameManager.Instance.isGameOver)
            yield break;

        canSpawn = false;

        spawnTimer += Time.deltaTime;

        for (int i = 0; i < enemiesLeftToSpawn; i++)
        {
            Vector3 spawnPos = simpleWaves.isBossWave ? Vector3.zero : GetValidSpawnPos();

            GameObject randomEnemy = simpleWaves.enemiesThatCanSpawn[Random.Range(0, simpleWaves.enemiesThatCanSpawn.Length)];
            GameObject spawnedEnemy = Instantiate(randomEnemy, spawnPos, Quaternion.identity);

            activeEnemies.Add(spawnedEnemy);

            spawnTimer = 0;

            yield return new WaitForSeconds(simpleWaves.timeBetweenSpawns);
        }

        yield return new WaitUntil(() => activeEnemies.Count == 0);
        
        if (currentWaveIndex < waves.Length)
        {
            currentWaveIndex++;
            StartWave(currentWaveIndex);

            float delay = simpleWaves.timeInBetweenWaves;
            StartCoroutine(GameManager.Instance.ShowWaveText(delay));
        }
    }




    Vector3 GetValidSpawnPos()
    {
        Camera cam = Camera.main;
        float z = -cam.transform.position.z;
        float camOffset = 1f;
        float gap = 0.5f; // kleine tussenruimte tussen zones
        float minDistanceFromPlayer = 6f;

        Vector3 camMin = cam.ViewportToWorldPoint(new Vector3(0, 0, z));
        Vector3 camMax = cam.ViewportToWorldPoint(new Vector3(1, 1, z));

        Vector2 mapCenter = mapBounds.bounds.center;
        Vector2 mapSize = mapBounds.bounds.size;
        Rect mapRect = new Rect(mapCenter - mapSize / 2f, mapSize);

        float camWidth = camMax.x - camMin.x;
        float camHeight = camMax.y - camMin.y;

        List<Rect> zones = new List<Rect>();

        zones.Add(new Rect(camMin.x - camOffset - 2f, camMin.y - camOffset, 2f, (camHeight / 2f) - gap)); // Links-onder
        zones.Add(new Rect(camMin.x - camOffset - 2f, camMin.y + (camHeight / 2f) + gap, 2f, (camHeight / 2f) - gap)); // Links-boven
        zones.Add(new Rect(camMax.x + camOffset, camMin.y - camOffset, 2f, (camHeight / 2f) - gap)); // Rechts-onder
        zones.Add(new Rect(camMax.x + camOffset, camMin.y + (camHeight / 2f) + gap, 2f, (camHeight / 2f) - gap)); // Rechts-boven
        zones.Add(new Rect(camMin.x - camOffset, camMax.y + camOffset, (camWidth / 2f) - gap, 2f)); // Boven-links
        zones.Add(new Rect(camMin.x + (camWidth / 2f) + gap, camMax.y + camOffset, (camWidth / 2f) - gap, 2f)); // Boven-rechts
        zones.Add(new Rect(camMin.x - camOffset, camMin.y - camOffset - 2f, (camWidth / 2f) - gap, 2f)); // Onder-links
        zones.Add(new Rect(camMin.x + (camWidth / 2f) + gap, camMin.y - camOffset - 2f, (camWidth / 2f) - gap, 2f)); // Onder-rechts

        List<Rect> validSpawnZones = new List<Rect>();

        foreach (Rect zone in zones)
        {
            bool fullyInside = IsRectFullyInside(zone, mapRect);

            if (fullyInside)
            {
                validSpawnZones.Add(zone);
            }
        }

        if (validSpawnZones.Count == 0)
        {
            Debug.LogWarning("Geen geldige spawnzones gevonden — fallback naar midden map.");
            return mapBounds.bounds.center;
        }

        for (int i = 0; i < 20; i++)
        {
            Rect chosenZone = validSpawnZones[Random.Range(0, validSpawnZones.Count)];
            float x = Random.Range(chosenZone.xMin, chosenZone.xMax);
            float y = Random.Range(chosenZone.yMin, chosenZone.yMax);
            Vector3 pos = new Vector3(x, y, 0f);

            if (Vector3.Distance(player.transform.position, pos) >= minDistanceFromPlayer)
            {
                return pos;
            }
        }

        Debug.LogWarning("Kon geen geldige spawnpositie vinden met afstand tot speler — fallback.");
        return mapBounds.bounds.center;
    }

    bool IsRectFullyInside(Rect inner, Rect outer)
    {
        return inner.xMin >= outer.xMin &&
               inner.xMax <= outer.xMax &&
               inner.yMin >= outer.yMin &&
               inner.yMax <= outer.yMax;
    }

    private void OnDrawGizmos()
    {
        if (player == null || mapBounds == null) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        float z = -cam.transform.position.z;
        float camOffset = 1f;
        float gap = 0.5f;

        Vector3 camMin = cam.ViewportToWorldPoint(new Vector3(0, 0, z));
        Vector3 camMax = cam.ViewportToWorldPoint(new Vector3(1, 1, z));

        Vector2 mapCenter = mapBounds.bounds.center;
        Vector2 mapSize = mapBounds.bounds.size;
        Rect mapRect = new Rect(mapCenter - mapSize / 2f, mapSize);

        float camWidth = camMax.x - camMin.x;
        float camHeight = camMax.y - camMin.y;

        List<Rect> zones = new List<Rect>();

        zones.Add(new Rect(camMin.x - camOffset - 2f, camMin.y - camOffset, 2f, (camHeight / 2f) - gap)); // Links-onder
        zones.Add(new Rect(camMin.x - camOffset - 2f, camMin.y + (camHeight / 2f) + gap, 2f, (camHeight / 2f) - gap)); // Links-boven

        zones.Add(new Rect(camMax.x + camOffset, camMin.y - camOffset, 2f, (camHeight / 2f) - gap)); // Rechts-onder
        zones.Add(new Rect(camMax.x + camOffset, camMin.y + (camHeight / 2f) + gap, 2f, (camHeight / 2f) - gap)); // Rechts-boven

        zones.Add(new Rect(camMin.x - camOffset, camMax.y + camOffset, (camWidth / 2f) - gap, 2f)); // Boven-links
        zones.Add(new Rect(camMin.x + (camWidth / 2f) + gap, camMax.y + camOffset, (camWidth / 2f) - gap, 2f)); // Boven-rechts

        zones.Add(new Rect(camMin.x - camOffset, camMin.y - camOffset - 2f, (camWidth / 2f) - gap, 2f)); // Onder-links
        zones.Add(new Rect(camMin.x + (camWidth / 2f) + gap, camMin.y - camOffset - 2f, (camWidth / 2f) - gap, 2f)); // Onder-rechts

        Gizmos.color = Color.yellow;
        foreach (Rect r in zones)
        {
            if (IsRectFullyInside(r, mapRect))
            {
                Gizmos.DrawWireCube(new Vector3(r.center.x, r.center.y, 0), new Vector3(r.width, r.height, 0));
            }
        }
    }
}

