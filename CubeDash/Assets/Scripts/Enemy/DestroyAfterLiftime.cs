using UnityEngine;

public class DestroyAfterLiftime : MonoBehaviour
{
    public float lifetime;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
