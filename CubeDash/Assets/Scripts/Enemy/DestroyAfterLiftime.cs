using UnityEngine;

public class DestroyAfterLiftime : MonoBehaviour
{
    public float lifetime;
    public bool playerProjectile = false;   

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerProjectile && collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
