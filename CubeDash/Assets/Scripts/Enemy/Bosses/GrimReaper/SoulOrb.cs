//using UnityEngine;

//public class SoulOrb : MonoBehaviour
//{
//    public float damageToBoss = 10f;
//    public float moveFactor = 1f;

//    public Vector3 startPos;
//    private bool wasHit = false;

//    private void Start()
//    {
//        startPos = transform.position;
//    }

//    public void Init(Vector3 targetPos, float factor)
//    {
//        moveFactor = factor;
//        startPos = transform.position;
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (wasHit) return;

//        if (other.CompareTag("Player"))
//        {
//            PlayerMovement pm = other.GetComponent<PlayerMovement>();
//            if (pm != null && pm.isDashing)
//            {
//                BossHealth boss = GameObject.FindWithTag("Boss").GetComponent<BossHealth>();
//                if (boss != null)
//                {
//                    boss.TakeDamage(damageToBoss);
//                    wasHit = true;

//                    // FX en destroy
//                    Destroy(gameObject);
//                }
//            }
//        }
//    }
//}
