using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSytheAttack : MonoBehaviour
{
    public float startangle = -40f;
    public float endAngle = 120f;
    public float duration = .3f;

    private Vector3 orgScale;
    private bool isFlipped = false;

    private SytheSlashSimple sytheSlashAttackSimple;
    private Color orgcolor;
    private SpriteRenderer spriteRenderer;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        orgcolor = spriteRenderer.color;
        sytheSlashAttackSimple = FindFirstObjectByType<SytheSlashSimple>();


        orgScale = transform.localScale;

        float random = Random.value;

        if (random < .5f)
        {
            isFlipped = true;
        }
        
        if (isFlipped)
        {
            gameObject.transform.localScale = new Vector3(-orgScale.x, orgScale.y, orgScale.z);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, -startangle);
        }
        else
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, startangle);
        }

        StartCoroutine(DoSlash());
    }

    private IEnumerator DoSlash()
    {
        yield return new WaitForSeconds(sytheSlashAttackSimple.anticipation - .1f);
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(.1f);
        spriteRenderer.color = orgcolor;

        LeanTween.cancel(gameObject);

        if (isFlipped)
        {
            LeanTween.rotate(gameObject, new Vector3(0, 0, -endAngle), duration).setEaseInExpo();
        }
        else
        {
            LeanTween.rotate(gameObject, new Vector3(0, 0, endAngle), duration).setEaseInExpo();
        }
       

        yield return new WaitForSeconds(.5f);
        sytheSlashAttackSimple.activeObjects.Remove(gameObject);
        Destroy(gameObject);
    }
}
