using System.Collections;
using UnityEngine;

public class SimpleSytheAttack : MonoBehaviour
{
    public float startangle = -40f;
    public float endAngle = 120f;
    public float anticipation = .5f;
    public float duration = .3f;

    private Vector3 orgScale;
    private bool isFlipped = false;

    private void Start()
    {
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
        yield return new WaitForSeconds(anticipation);

        LeanTween.cancel(gameObject);

        if (isFlipped)
        {
            LeanTween.rotate(gameObject, new Vector3(0, 0, -endAngle), duration).setEaseInExpo();
        }
        else
        {
            LeanTween.rotate(gameObject, new Vector3(0, 0, endAngle), duration).setEaseInExpo();
        }
       

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
