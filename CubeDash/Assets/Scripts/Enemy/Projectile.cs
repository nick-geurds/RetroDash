using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float force;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        transform.localPosition += transform.up * force * Time.deltaTime;
    }

}
