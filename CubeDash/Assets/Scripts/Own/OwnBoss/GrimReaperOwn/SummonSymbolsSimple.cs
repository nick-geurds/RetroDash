using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class SummonSymbolsSimple : MonoBehaviour
{
    public GameObject objectToSpawn;

    private DamageHandlerSimple damageHandler;
    private SpriteRenderer spriteRenderer;
    private Color orgColor;


    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        orgColor = spriteRenderer.color;
        LeanTween.delayedCall(2f, SpinAndDissapear);
    }
    private void SpinAndDissapear()
    {
        float randomChange = Random.value;

        LeanTween.rotate(gameObject, new Vector3(0, 0, 360 / 5), .2f).setEaseInExpo().setOnComplete(() =>
        {
            StartCoroutine(colorSwitch());

            GameObject spawnObject = Instantiate(objectToSpawn);
            spawnObject.transform.position = gameObject.transform.position;
            damageHandler = spawnObject.GetComponent<DamageHandlerSimple>();

            if (randomChange < .2f)
            {
                damageHandler.isHittable = true;
            }
            else
            {
                damageHandler.isHittable = false;
            }

            //spriteRenderer.color = new Color(0.1176f, 0f, 0.5490f);
            
        });

        LeanTween.delayedCall(.5f, ScaleDown);
        
    } 

    private void ScaleDown()       
    {

        LeanTween.scale(gameObject, Vector3.zero, .4f).setEaseInBack().setOnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    IEnumerator colorSwitch()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(.1f);
        spriteRenderer.color = orgColor;
    }
}
