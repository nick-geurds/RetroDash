using System.Collections;
using UnityEngine;

public abstract class BaseAttack : MonoBehaviour
{
    public float delayBetweenAttacks = 1f;
    public int repeatCount = 1;

    protected Transform player;

    public virtual void Initialize(Transform playerTransform)
    {
        player = playerTransform;
    }

    public abstract IEnumerator ExecuteAttack();
}
