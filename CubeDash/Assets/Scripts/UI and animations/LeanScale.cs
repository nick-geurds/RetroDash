using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LeanScale : MonoBehaviour
{
    [Header("Anim Settings")]
    public float scale;
    public float duration;
    public LeanTweenType tweenType;

    [Header("SpriteSettings")]
    public bool changeSprite;
    public CanvasGroup spriteToFadeIn;

    private Image image;


    private Vector3 orgScale;

    private SceneChanger changer;

    private void Start()
    {
        orgScale = transform.localScale;

        if (GetComponent<Image>() != null )
            image = GetComponent<Image>();

        if (GetComponent<SceneChanger>() != null)
            changer = GetComponent<SceneChanger>();

        if (changer != null)
            duration = changer.delayTime;
    }

    public void ScaleDown()
    {
        LeanTween.scale(gameObject, orgScale * scale, duration).setEase(tweenType).setIgnoreTimeScale(true);

        if (changeSprite)
        {
            LeanTween.delayedCall(.5f, () =>
            {
                LeanTween.alphaCanvas(spriteToFadeIn, 1, duration - .6f);
            });
            
        }
    }



}
