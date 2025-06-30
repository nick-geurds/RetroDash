using UnityEngine;

public class GameOverAnimations : MonoBehaviour
{
    private void OnEnable()
    {
        LeanTween.scale(gameObject, new Vector3(2, 2, 2), 2f).setEaseInElastic().setIgnoreTimeScale(true);
    }
}
