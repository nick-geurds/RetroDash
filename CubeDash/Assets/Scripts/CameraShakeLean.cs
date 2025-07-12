using System.Collections;
using UnityEngine;

public class CameraShakeLean : MonoBehaviour
{
    public static CameraShakeLean instance;

    [Header("Shake Settings")]
    public float shakeAmount;
    public float shakeDur;
    public LeanTweenType shake = LeanTweenType.easeShake;

    [Header("Punch Settings")]
    public float punchAmount;
    public float punchDur;
    public LeanTweenType punch = LeanTweenType.punch;

    [Header("Rumble Settings")]
    public LeanTweenType rumbleEase = LeanTweenType.easeShake;  // Gemakkelijkere bewegingseffecten

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

    public IEnumerator Rumble(float intensity, float speed, float duration)
    {
        // Stop bestaande bewegingen voordat we een nieuwe starten
        LeanTween.cancel(gameObject);  // Stop alle bestaande tweens voor dit object

        // Verkrijg de huidige positie van de camera
        Vector3 originalPos = transform.position;
        Vector3 targetPos = originalPos; // De basispositie van de camera (vastgesteld door PlayerFollowForCam)
        float endTime = Time.time + duration;

        while (Time.time < endTime)
        {
            // Willekeurige verschuiving in de X en Y richting
            float xOffset = Random.Range(-intensity, intensity);
            float yOffset = Random.Range(-intensity, intensity);

            // Voeg de trilling als een offset toe bovenop de bestaande positie
            Vector3 rumblePosition = targetPos + new Vector3(xOffset, yOffset, 0f);

            // Pas de trilling toe als een offset zonder de PlayerFollowForCam volledig te vervangen
            LeanTween.moveLocal(gameObject, rumblePosition, speed)
                .setEase(rumbleEase);

            // Wacht een frame voordat de volgende verschuiving gebeurt
            yield return null;  // Dit zorgt ervoor dat de trilling over meerdere frames gebeurt
        }

        // Na afloop van de trilling, herstel de originele positie (optioneel)
        transform.position = targetPos;
    }
}
