using System.Drawing;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RotateVisualSimple : MonoBehaviour
{
    public float rotateSpeed;

    public bool hasWindUp;
    public float windUpTime = .4f;

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        float speedMult = 1f;

        if (hasWindUp && timer < windUpTime)
        {
            speedMult = .2f;
        }

        float rotateThisFrame = rotateSpeed * speedMult * 100 * Time.deltaTime;
        float currentRotation = transform.localEulerAngles.z;

        transform.rotation = Quaternion.Euler(0, 0, currentRotation + rotateThisFrame);
        

    }
}
