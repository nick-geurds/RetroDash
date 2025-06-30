using UnityEngine;

public class CameraShakeLean : MonoBehaviour
{
    public static CameraShakeLean instance; 

    [Header("Shake Settings")]
    public float shakeAmount;
    public float shakeDur;
    public LeanTweenType shake = LeanTweenType.easeShake;

    [Header("punch settings")]
    public float punchAmount;
    public float punchDur;
    public LeanTweenType punch = LeanTweenType.punch;

    private Vector3 orgPos;
    private Quaternion orgRot;
    private Vector3 orgScale;


    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        orgPos = transform.position;
        orgRot = transform.rotation;
        orgScale = transform.localScale;
    }    

    public void ImpactShake()
    {
        float rotatePositive = transform.rotation.z + 15;
        float rotateNegative = transform.rotation.z - 15;

        float rotateRandom = Random.value < 0.5f ? rotatePositive : rotateNegative;
        LeanTween.cancel(gameObject);
        LeanTween.rotate(gameObject, new Vector3(0, 0, rotateRandom), shakeDur).setEase(shake);
        
        
    }
}
