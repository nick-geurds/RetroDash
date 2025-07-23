using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyProjectileShooter : MonoBehaviour
{
    [Header("OnDeathSettings")]
    public bool onlyExplodeOnDeath;

    [Header("Projectile settings")]
    private ObjectPoolSimple objectPool;
    public float degree = 360f;
    public int numberOfProjectiles;
    public float spawnRadius;
    public float speed;
    public float projInterval;


    private float timer;

    private List<GameObject> activeProjectiles = new List<GameObject>();

    private void Start()
    {
        objectPool = GameObject.Find("ProjectilePool1").GetComponent<ObjectPoolSimple>();
    }


    private void LateUpdate()
    {
        if (onlyExplodeOnDeath)
            return;

        timer += Time.deltaTime;

        if (timer >= projInterval)
        {
            Spawn();
            timer = 0;
        }
    }

    public void Spawn()
    {
        float arcAngle = (degree / 360f) * 2f * Mathf.PI;
        float nextAngle = arcAngle / numberOfProjectiles;
        float angle = Mathf.PI / 2;

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            float x = Mathf.Cos(angle) * spawnRadius;
            float y = Mathf.Sin(angle) * spawnRadius;

            GameObject obj = objectPool.GetObject();
            obj.transform.position = transform.position;
            obj.transform.rotation = Quaternion.identity;

            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            rb.linearVelocity = new Vector2(x, y).normalized * speed * 3;

            // Voeg ProjectileLifetime toe en initialiseer
            ProjectileLifetime lifetime = obj.GetComponent<ProjectileLifetime>();
            if (lifetime != null)
            {
                lifetime.Init(objectPool, projInterval - 0.1f);
            }

            angle += nextAngle;
        }
    }


    public IEnumerator ReturnToPool(GameObject projectile)
    {
        yield return new WaitForSeconds(projInterval - .1f);
        objectPool.ReturnObjectToPool(projectile);
    }

    public void ReturnAllProjectiles()
    {
        foreach (var projectile in activeProjectiles)
        {
            if (projectile.activeInHierarchy)
            {
                objectPool.ReturnObjectToPool(projectile);
            }
        }

        activeProjectiles.Clear();
    }

}
