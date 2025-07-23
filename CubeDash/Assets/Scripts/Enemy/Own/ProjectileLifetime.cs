using System.Collections;
using UnityEngine;

public class ProjectileLifetime : MonoBehaviour
{
    private ObjectPoolSimple pool;
    private float lifeTime = 1f;

    public void Init(ObjectPoolSimple poolRef, float duration)
    {
        pool = poolRef;
        lifeTime = duration;
        StartCoroutine(ReturnToPoolAfterDelay());
    }

    private IEnumerator ReturnToPoolAfterDelay()
    {
        yield return new WaitForSeconds(lifeTime);

        if (gameObject.activeInHierarchy)
        {
            pool.ReturnObjectToPool(gameObject);
        }
    }
}
