using UnityEngine;

public class ShockWaveBoss : MonoBehaviour
{
    private Vector3 orgScale;
    public float duration;

    private void Start()
    {
        orgScale = gameObject.transform.localScale;
        ExpandShockWave();
    }
    public void ExpandShockWave()
    {
        gameObject.transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, orgScale, duration).setEaseOutExpo().setOnComplete(() =>
        {
             Destroy(gameObject);
        });
    }
}
