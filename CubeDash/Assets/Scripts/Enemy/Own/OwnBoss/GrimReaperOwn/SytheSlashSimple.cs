using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class SytheSlashSimple : MonoBehaviour
{
    private BossStateMachineSimple stateMachine;

    public GameObject objectToSpawn;
    public int numberToSpawn;
    public float interval;
    public float yOffset = -2f;

    private DamageHandlerSimple dmgHandle;

    private GameObject player;

    private void Start()
    {
        stateMachine = FindFirstObjectByType<BossStateMachineSimple>();
        stateMachine.activeAttacks.Add(gameObject);
        player = GameObject.Find("Player");
        
        StartCoroutine(SliceAttack());
    }

    //private void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.B))
    //    {
    //        StartCoroutine(SliceAttack());
    //    }
    //}

    private IEnumerator SliceAttack()
    {
        int randomNumber = Random.Range(numberToSpawn - 1, 0);

        for (int i = 0; i < numberToSpawn; i++)
        {

            GameObject spawnedObject = Instantiate(objectToSpawn, new Vector3(player.transform.position.x, player.transform.position.y + yOffset, 0) , Quaternion.identity);
            dmgHandle = spawnedObject.GetComponent<DamageHandlerSimple>();

            if (i == numberToSpawn - randomNumber)
            {
                dmgHandle.isHittable = true;
            }
            else
            {
                dmgHandle.isHittable = false;
            }

            yield return new WaitForSeconds(interval);

        }
                stateMachine.activeAttacks.Remove(gameObject);
                Destroy(gameObject);

        
    }
}
