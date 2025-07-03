using UnityEngine;

public class CanvasGroupAlpha : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float duration = 1.4f;
   public void AlphaCanvas()
    {
        LeanTween.cancel(gameObject);
        LeanTween.alphaCanvas(canvasGroup, 0 , duration).setIgnoreTimeScale(true);
    }
}
