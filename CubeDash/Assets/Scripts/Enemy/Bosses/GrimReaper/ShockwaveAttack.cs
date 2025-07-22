using System.Collections;
using UnityEngine;

public class ShockwaveAttack : BaseAttack
{
    public GameObject chargeCirclePrefab;
    public GameObject shockwavePrefab;

    [Header("Settings")]
    public int numberOfShockwaves = 3;
    public float radius = 5f;
    public float timeBetweenShockwaves = 0.5f;
    public float chargeDuration = 1.5f;

    public override AttackType attackType => AttackType.Special;

    public override IEnumerator ExecuteAttack()
    {
        Vector2 center = Vector2.zero;

        // Maak warning zichtbaar
        GameObject charge = Instantiate(chargeCirclePrefab, center, Quaternion.identity, null);
        charge.transform.localScale = Vector3.one * radius * 2f;

        yield return new WaitForSeconds(chargeDuration);

        for (int i = 0; i < numberOfShockwaves; i++)
        {
            bool isLast = i == numberOfShockwaves - 1;

            GameObject wave = Instantiate(shockwavePrefab, center, Quaternion.identity, null);
            ShockwaveRing ring = wave.GetComponent<ShockwaveRing>();

            if (ring != null)
            {
                if (isLast)
                    ring.Initialize(radius, () => Destroy(charge));
                else
                    ring.Initialize(radius);
            }

            yield return new WaitForSeconds(timeBetweenShockwaves);
        }
    }
}
