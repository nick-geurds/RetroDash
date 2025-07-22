using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovementSimple : MonoBehaviour
{
    [Header("Move settings")]
    public float followSpeed;


    [HideInInspector] public GameObject player;
    [HideInInspector] public SpriteRenderer sprite;
    [HideInInspector] public Color orgColor;
    [HideInInspector] public bool isDashing = false;


    protected virtual void Start()
    {
        player = GameObject.Find("Player");
        sprite = GetComponent<SpriteRenderer>();
        orgColor = sprite.color;
    }

    protected virtual void LateUpdate()
    {
        if (isDashing)
            return;

        float speed = followSpeed * Time.deltaTime;

        if (player != null)
        {
            transform.position = Vector2.MoveTowards(gameObject.transform.position , player.transform.position, speed);
        }
    }
}
