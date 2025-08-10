using System.Collections;
using UnityEngine;

public class SimpleChain : MonoBehaviour
{
    //public float moveLocalY;
    public float moveDuration;
    public bool moveX = false;
    public float moveToAmount;

    private Vector3 orgPos;
    void Start()
    {
        orgPos = transform.localPosition;
        if (!moveX)
        {

            ChainMoveY();
        }
        else
        {
            ChainMoveX();
        }
    }

    private void ChainMoveY()
    {
        LeanTween.cancel(gameObject);
        LeanTween.moveLocal(gameObject, Vector3.zero, moveDuration).setEaseInExpo();
    }

    private void ChainMoveX()
    {
        float offset = moveToAmount;
        LeanTween.cancel(gameObject);
        LeanTween.moveLocalX(gameObject, transform.localPosition.x + offset , moveDuration).setEaseInExpo();
    }
}
