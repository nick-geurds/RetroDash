using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waves
{
    public GameObject[] enemiesToSpawn;
    public float spawnInterval;
    public int numberOfEnemies;  // maak int ipv float, logischer
    public float timeBetweenWaves;
    public GameObject[] spawnPoints;
}

public class EnemySpawnManager : MonoBehaviour
{
    public static List<GameObject> activeEnemies = new List<GameObject>();

    public Waves[] waves;
    private Waves waveIndex;
    public int currentWaveIndex;

    public BoxCollider2D mapBounds; // <--- Voeg dit toe in de inspector

    private GameObject player;

    private bool canSpawn = true;
    private bool waveGotUpped;

    private float spawnTimer;
    private float timeBetweenWaveTimer;

    private int enemiesLeftToSpawn;

    private void OnEnable()
    {
        ResetSpawner();
    }

    private void ResetSpawner()
    {
        currentWaveIndex = 0;
        activeEnemies.Clear();
        InitializeWave(currentWaveIndex);

        player = GameObject.Find("Player");

        if (mapBounds == null)
        {
            mapBounds = Object.FindFirstObjectByType<BoxCollider2D>(); // of via MapBounds component
        }
    }

    private void InitializeWave(int index)
    {
        if (index < 0 || index >= waves.Length)
        {
            Debug.LogWarning("Wave index buiten bereik: " + index);
            canSpawn = false;
            return;
        }

        waveIndex = waves[index];
        enemiesLeftToSpawn = waveIndex.numberOfEnemies;
        canSpawn = true;
        waveGotUpped = false;
        spawnTimer = 0f;
        timeBetweenWaveTimer = 0f;
    }

    private void Update()
    {

        if (currentWaveIndex >= waves.Length)
        {
            // Alle waves zijn gedaan, niks meer spawnen
            return;
        }

        waveIndex = waves[currentWaveIndex];
        SpawnWave();
    }

    void SpawnWave()
    {
        spawnTimer += Time.deltaTime;

        if (canSpawn)
        {
            if (spawnTimer > waveIndex.spawnInterval && enemiesLeftToSpawn > 0)
            {
                GameObject randomEnemy = waveIndex.enemiesToSpawn[Random.Range(0, waveIndex.enemiesToSpawn.Length)];

                Vector3 spawnPos = GetValidSpawnPos();

                if (spawnPos != Vector3.zero)
                {
                    Instantiate(randomEnemy, spawnPos, Quaternion.identity);
                    enemiesLeftToSpawn--;
                }

                spawnTimer = 0;
            }

            if (enemiesLeftToSpawn <= 0)
            {
                canSpawn = false;
            }
        }

        if (activeEnemies.Count == 0)
        {
            timeBetweenWaveTimer += Time.deltaTime;

            if (timeBetweenWaveTimer > waveIndex.timeBetweenWaves && !waveGotUpped)
            {
                currentWaveIndex++;

                if (currentWaveIndex < waves.Length)
                {
                    InitializeWave(currentWaveIndex);
                    // Start coroutine hier of roep extern aan:
                    StartCoroutine(GameManager.Instance.ShowWaveText(waves[currentWaveIndex + 1].timeBetweenWaves));
                }
                else
                {
                    Debug.Log("Alle waves voltooid!");
                    // Hier evt logica voor einde level
                }

                timeBetweenWaveTimer = 0;
                waveGotUpped = true;
            }
        }
        else
        {
            // Reset guard zodra er enemies zijn
            waveGotUpped = false;
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
