using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_WarpperSpinningSythe : BaseAttack
{
    public ObjectPool sythePool; // <-- Verwijzing naar object pool
    public float spawnInterval = 0.3f;
    public float xOffset = 3f;
    public float yOffsetRange = 1.5f;
    public Transform parentTransform;

    public override AttackType attackType => AttackType.Special;

    public override IEnumerator ExecuteAttack()
    {
        int totalSythes = 6;
        int hittableIndex = Random.Range(0, totalSythes);

        // Verzamel eerst beschikbare sythes
        List<GameObject> availableSythes = sythePool.GetAvailableObjects(totalSythes);

        if (availableSythes.Count < totalSythes)
        {
            Debug.LogWarning("Niet genoeg sythes beschikbaar in de pool!");
            yield break;
        }

        ShuffleList(availableSythes); // <-- Maak volgorde random

        for (int i = 0; i < totalSythes; i++)
        {
            GameObject sytheObj = availableSythes[i];

            float side = (i < 3) ? -1f : 1f;
            float xPos = side * xOffset;
            float yPos = Random.Range(-yOffsetRange, yOffsetRange);
            Vector2 spawnPosition = new Vector2(xPos, yPos);

            sytheObj.transform.position = spawnPosition;
            sytheObj.SetActive(true);

            if (parentTransform != null)
                sytheObj.transform.SetParent(parentTransform, true);

            // Init gedrag
            R_SpinningSythe spinningSythe = sytheObj.GetComponent<R_SpinningSythe>();
            if (spinningSythe != null)
            {
                spinningSythe.reverseCurveDirection = Random.value > 0.5f;
                spinningSythe.Initialize(); // Zorg dat deze methode alles reset (zoals Start() deed)
            }

            // Alleen 1 sikkel hittable
            TriggerDamageHandler damageHandler = sytheObj.GetComponentInChildren<TriggerDamageHandler>();
            if (damageHandler != null)
                damageHandler.SetHittableStatus(i == hittableIndex);

            StartCoroutine(DisableAfterSeconds(sytheObj, 4f));

            yield return new WaitForSeconds(spawnInterval);
        }

        yield return new WaitForSeconds(3f);
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    private IEnumerator DisableAfterSeconds(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        sythePool.Return(obj);
    }
}
