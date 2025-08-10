using UnityEngine;
using UnityEngine.Rendering;

public class AnimOnDamage : MonoBehaviour
{
    [Header("Anim Settigns")]
    public float scaleAmount;
    public float scaleDuration;

    private Vector3 orgScale;

    private void Start()
    {
        orgScale = transform.localScale;
    }
    public void ScaleOnDamage()
    {
        LeanTween.cancel(gameObject);

        LeanTween.scale(gameObject, orgScale * scaleAmount, scaleDuration).setEasePunch().setOnComplete(() =>
        {
            transform.localScale = orgScale;
        });
    }
}
