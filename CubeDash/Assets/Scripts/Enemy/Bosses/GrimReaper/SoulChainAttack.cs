using System.Collections;
using UnityEngine;

public class SoulChainAttack : BaseAttack
{
    public GameObject chainPrefab;  // De prefab voor de keten
    public Transform mapBounds;  // De MapBounds van de arena
    private CameraShakeLean cameraShakeLean;

    // Set je attackType
    public override AttackType attackType => AttackType.Special;

    public override IEnumerator ExecuteAttack()
    {
        // Verkrijg de hoeken van de MapBounds
        Vector3[] corners = GetMapCorners();

        // Maak een parent object voor de ketens
        GameObject chainParent = new GameObject("ChainParent");

        // Maak een lijst om de keten objecten bij te houden
        GameObject[] spawnedChains = new GameObject[4];

        // Spawn de keten prefabs en voeg ze toe als child van het parent object
        for (int i = 0; i < corners.Length; i++)
        {
            // Maak een nieuwe keten aan (zonder objectpool)
            GameObject chain = Instantiate(chainPrefab);

            // Bepaal het midden tussen de huidige hoek en Vector3.zero
            Vector3 midpoint = (corners[i] + Vector3.zero) / 2f;  // Midden tussen hoek en oorsprong

            // Zet de keten in de juiste positie en rotatie
            chain.transform.position = midpoint;
            float[] rotations = { 135f, 45f, -45f, -135f };
            chain.transform.rotation = Quaternion.Euler(0f, 0f, rotations[i]);

            // Zet de keten als child van het parent object
            chain.transform.SetParent(chainParent.transform);

            // Voeg de keten toe aan de lijst
            spawnedChains[i] = chain;

            // Start de kettingbewegingen tegelijk
            SoulChain soulChain = chain.GetComponentInChildren<SoulChain>();
            if (soulChain != null)
            {
                soulChain.MoveChain();  // Start de beweging van de ketting
                soulChain.SetWaitingForReturn(true);  // Zet de ketting op wachten voor de terugbeweging
            }
            else
            {
                Debug.LogError("SoulChain component niet gevonden op child object van keten!");
            }
        }

        // Wacht totdat de kettingen klaar zijn met bewegen (bijvoorbeeld 3 seconden na het spawnen)
        yield return new WaitForSeconds(3f);  // Laat de kettingen eerst 3 seconden bewegen

        // Camera shake en rotatie
        cameraShakeLean = Object.FindFirstObjectByType<CameraShakeLean>();

        if (cameraShakeLean != null)
        {
            StartCoroutine(cameraShakeLean.Rumble(0.2f, 0.2f, .2f));  // Start de trilling (0.2 intensiteit, 0.2 snelheid, 2 seconden)
        }
        else
        {
            Debug.LogError("CameraShakeLean component niet gevonden!");
        }

        // Gebruik LeanTween om het parent object 45 graden te draaien rond Vector3.zero
        LeanTween.rotate(chainParent, new Vector3(0f, 0f, 45f), 1f)
            .setEase(LeanTweenType.easeInOutElastic);  // Langzame start en snelle payoff

        // Nadat de rotatie is voltooid, wachten we een tijdje voordat de ketens verdwijnen
        yield return new WaitForSeconds(1f);  // Wacht even voordat we beginnen met de vernietiging

        // Wacht totdat alle kettingen klaar zijn met terugbewegen voordat we vernietigen
        foreach (var chain in spawnedChains)
        {
            // Check of het object nog bestaat voordat we proberen te controleren of het active is
            if (chain != null)
            {
                SoulChain soulChain = chain.GetComponentInChildren<SoulChain>();
                if (soulChain != null)
                {
                    // Wacht totdat de ketting haar terugbeweging heeft voltooid
                    yield return new WaitUntil(() => soulChain == null || !soulChain.gameObject.activeSelf);
                }
            }
        }

        // Verwijder de ketens uit de scène
        foreach (var chain in spawnedChains)
        {
            if (chain != null)
            {
                Destroy(chain);  // Vernietig de ketens na het uitvoeren van de aanval
            }
        }
    }

    // Verkrijg de hoeken van de MapBounds door het GameObject te vinden en te gebruiken
    private Vector3[] GetMapCorners()
    {
        if (mapBounds == null)
        {
            mapBounds = GameObject.Find("MapBounds").transform;
        }

        BoxCollider2D boxCollider = mapBounds.GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError("MapBounds heeft geen BoxCollider2D!");
            return new Vector3[0];
        }

        Vector2 size = boxCollider.size;
        Vector2 center = boxCollider.transform.position;

        Vector3[] corners = new Vector3[4];
        corners[0] = new Vector3(center.x - size.x / 2f, center.y - size.y / 2f, 0f); // Linksonder
        corners[1] = new Vector3(center.x - size.x / 2f, center.y + size.y / 2f, 0f); // Links boven
        corners[2] = new Vector3(center.x + size.x / 2f, center.y + size.y / 2f, 0f); // Rechts boven
        corners[3] = new Vector3(center.x + size.x / 2f, center.y - size.y / 2f, 0f); // Rechts onder

        return corners;
    }
}
