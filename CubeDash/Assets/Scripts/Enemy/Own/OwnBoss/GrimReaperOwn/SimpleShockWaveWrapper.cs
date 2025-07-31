using System.Collections;
using UnityEngine;

public class SimpleShockWaveWrapper : MonoBehaviour
{
    private BossStateMachineSimple stateMachine;

    public GameObject objectToSpawn;
    public GameObject objectToRotate;
    public int numberOfShockwaves;
    public float interval = 1;

    private DamageHandlerSimple dmgHandle;

    private void Start()
    {
        stateMachine = FindFirstObjectByType<BossStateMachineSimple>();
        stateMachine.activeAttacks.Add(gameObject);
        StartCoroutine(ShockWaveAttack());
    }

    private IEnumerator ShockWaveAttack()
    {
        int randomNumber = Random.Range(numberOfShockwaves - 1, 0);

        GameObject pentagon = Instantiate(objectToRotate);

        yield return new WaitForSeconds(1.5f);

        float angle = 360/5 ;
        float nextAngle = 360 / 5;

        for (int i = 0; i < numberOfShockwaves; i++)
        {
            LeanTween.rotate(pentagon, new Vector3(0, 0, angle), interval / 2).setEaseInExpo();
            angle += nextAngle;

            yield return new WaitForSeconds(interval / 2);

            GameObject spawnedObject = Instantiate(objectToSpawn, Vector3.zero, Quaternion.identity);
            dmgHandle = spawnedObject.GetComponent<DamageHandlerSimple>();


            if (i == randomNumber)
            {
                dmgHandle.isHittable = true;
            }
            else
            {
                dmgHandle.isHittable = false;
            }



            yield return new WaitForSeconds(interval);
        }

        LeanTween.scale(pentagon, Vector3.zero, interval).setEaseInExpo().setOnComplete(() =>
        {
            Destroy(pentagon);
            stateMachine.activeAttacks.Remove(gameObject);
            Destroy(gameObject);
        });
    }
}
