using System.Collections;
using UnityEngine;

public class SimpleSummonSpecial : MonoBehaviour
{
    public float spawnRadius = 6f;

    public GameObject objectToSpawn;

    public int nummberToSpawn;

    public float interval;
    private float timer = 0f;

    private DamageHandlerSimple dmgHandle;
    private BossStateMachineSimple bossStateMachine;

    private void Start()
    {
        StartCoroutine(Summon());
        bossStateMachine = FindFirstObjectByType<BossStateMachineSimple>();
        bossStateMachine.activeAttacks.Add(gameObject);
    }

    private IEnumerator Summon()
    {
        int randomHittable = Random.Range(0, nummberToSpawn - 1);

        float nextAngle = ((360f / 360f) * 2f * Mathf.PI) / nummberToSpawn;
        float angle = Mathf.PI / 2;

        for (int i = 0; i < nummberToSpawn; i++)
        {
            


            float x = Mathf.Cos(angle) * spawnRadius;
            float y = Mathf.Sin(angle) * spawnRadius;

            GameObject spawnedObject = Instantiate(objectToSpawn);
            spawnedObject.transform.position = new Vector2(transform.position.x + x, transform.position.y + y);
            spawnedObject.transform.rotation = Quaternion.identity;

            //dmgHandle = spawnedObj.GetComponent<DamageHandlerSimple>();

            //if (i == randomHittable)
            //{
            //    dmgHandle.isHittable = true;
            //}
            //else
            //{
            //    dmgHandle.isHittable = false;
            //}
            angle += nextAngle;
        }

        yield return new WaitForSeconds(5);
        bossStateMachine.activeAttacks.Remove(gameObject);
        Destroy(gameObject);
    }
}
