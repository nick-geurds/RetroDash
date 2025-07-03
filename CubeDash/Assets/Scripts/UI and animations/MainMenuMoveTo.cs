using UnityEngine;

public class MainMenuMoveTo : MonoBehaviour
{
    public float duration;
    public LeanTweenType type;
    public void MoveTo(float amount)
    {
        LeanTween.cancel(gameObject);
        LeanTween.moveLocalX(gameObject, amount, duration).setEase(type).setIgnoreTimeScale(true);
    }
}
