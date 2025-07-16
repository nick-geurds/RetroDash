using UnityEngine;

public class SpinningSprite : MonoBehaviour
{
    public float slowSpeedDuration = 2f;     // Hoe lang de trage rotatie duurt
    public float slowRotationTime = 1f;      // Tijd (seconden) voor 1 trage rotatie
    public float fastRotationTime = 0.2f;    // Tijd (seconden) voor 1 snelle rotatie

    private int tweenId = -1; // Voor het bijhouden van de huidige LeanTween (zodat we hem kunnen cancelen)

    private void Start()
    {
        StartSlowRotation();
        Invoke(nameof(SwitchToFastRotation), slowSpeedDuration);
    }

    void StartSlowRotation()
    {
        tweenId = LeanTween.rotateAroundLocal(gameObject, Vector3.forward, 360f, slowRotationTime)
            .setRepeat(-1)
            .setLoopClamp()
            .id;
    }

    void SwitchToFastRotation()
    {
        LeanTween.cancel(tweenId); // Stop de langzame rotatie

        tweenId = LeanTween.rotateAroundLocal(gameObject, Vector3.forward, 360f, fastRotationTime)
            .setRepeat(-1)
            .setLoopClamp()
            .id;
    }
}
