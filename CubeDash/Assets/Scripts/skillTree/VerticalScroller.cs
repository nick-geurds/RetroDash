using UnityEngine;

public class VerticalScroller : MonoBehaviour
{
    [Header("Scroll settings")]
    public GameObject scrollObject;
    public float scrollSpeed = 1f;

    [Header("Scroll Limits (Y Axis)")]
    public float minY = -5f;
    public float maxY = 5f;

    [Header("Optional")]
    public GameObject confirmPanel;
    private Vector3 confirmPanelTargetPos;

    private bool isDragging = false;
    private Vector2 startDragPos;
    private bool hasDeactivated = false;

    private void Start()
    {
        if (confirmPanel != null)
        {
            confirmPanelTargetPos = confirmPanel.transform.position;
        }
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#elif UNITY_IOS || UNITY_ANDROID
        HandleTouchInput();
#endif
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDrag(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Drag(Input.mousePosition);

        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                StartDrag(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Drag(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                EndDrag();
            }
        }
    }

    void StartDrag(Vector2 position)
    {
        isDragging = true;
        startDragPos = position;

    }

    void Drag(Vector2 currentPos)
    {
        if (scrollObject == null) return;

        // Bepaal deltaY op basis van input
        float inputDeltaY = (currentPos.y - startDragPos.y) * scrollSpeed * Time.deltaTime;

        // Bepaal nieuwe scrollpositie en clamp deze
        Vector3 oldPos = scrollObject.transform.position;
        Vector3 newPos = oldPos + Vector3.up * inputDeltaY;
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        // Verplaats scrollObject
        scrollObject.transform.position = newPos;

        // Bepaal echte delta in wereldruimte
        Vector3 actualDelta = newPos - oldPos;

        // Verplaats confirmPanel met exact dezelfde delta
        if (confirmPanel != null)
        {
            confirmPanel.transform.position += actualDelta;
        }

        // Update startpositie voor volgende frame
        startDragPos = currentPos;
    }



    void EndDrag()
    {
        isDragging = false;
    }
}
