using UnityEngine;

public class VerticalScrollMoveTwoObjects : MonoBehaviour
{
    [Tooltip("Eerste object dat verticaal bewogen wordt")]
    public GameObject targetObject1;

    [Tooltip("Tweede object dat verdwijnt zodra scroll/swipe begint")]
    public GameObject targetObject2;

    [Tooltip("Snelheid van de beweging")]
    public float speed = 5f;

    [Tooltip("Duur van de tween animatie in seconden")]
    public float tweenDuration = 0.2f;

    private Vector2 touchStartPos;
    private bool isTouching = false;
    private bool startedMoving = false;

    private float offsetY = 0f;
    private float startY1;

    void Start()
    {
        if (targetObject1 != null)
            startY1 = targetObject1.transform.position.y;
    }

    void Update()
    {
        if (targetObject1 == null) return;

        float moveDelta = 0f;

        // PC scroll input
        moveDelta = Input.mouseScrollDelta.y * speed;

        // Mobiele touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isTouching = true;
                    startedMoving = false; // reset start flag bij nieuwe touch
                    break;

                case TouchPhase.Moved:
                    if (isTouching)
                    {
                        float swipeDeltaY = touch.deltaPosition.y;
                        moveDelta = swipeDeltaY * speed * 0.1f;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isTouching = false;
                    startedMoving = false;
                    break;
            }
        }

        if (Mathf.Abs(moveDelta) > 0.01f)
        {
            if (!startedMoving)
            {
                startedMoving = true;

                // Zet targetObject2 uit zodra scroll/swipe begint
                if (targetObject2 != null)
                    targetObject2.SetActive(false);
            }

            offsetY += moveDelta * Time.deltaTime;

            LeanTween.cancel(targetObject1);
            LeanTween.moveY(targetObject1, startY1 + offsetY, tweenDuration).setEase(LeanTweenType.easeOutQuad);
        }
    }
}
