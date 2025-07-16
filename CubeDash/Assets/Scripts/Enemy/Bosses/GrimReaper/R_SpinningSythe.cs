using UnityEngine;

public class R_SpinningSythe : MonoBehaviour
{
    public float windUpTime = 2f;
    public float maxFollowSpeed = 1.5f;
    public float minFollowSpeed = 0.5f;
    public float curveOffsetStrength = 2f;

    public bool reverseCurveDirection = false;

    private GameObject player;
    private Vector2 startPoint;
    private Vector2 controlPoint;
    private Vector2 endPoint;

    private float t;
    private bool isWindUpComplete;
    private float windUpTimer;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Geen speler gevonden met de tag 'Player'");
            enabled = false;
        }
    }

    public void Initialize()
    {
        // Reset variabelen
        t = 0f;
        windUpTimer = 0f;
        isWindUpComplete = false;
        enabled = true;

        if (player == null) return;

        startPoint = transform.position;
        endPoint = player.transform.position;

        Vector2 midPoint = (startPoint + endPoint) / 2f;
        Vector2 direction = (endPoint - startPoint).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);
        float offset = reverseCurveDirection ? -curveOffsetStrength : curveOffsetStrength;
        controlPoint = midPoint + perpendicular * offset;
    }

    void Update()
    {
        if (!isWindUpComplete)
        {
            windUpTimer += Time.deltaTime;
            if (windUpTimer >= windUpTime)
                isWindUpComplete = true;
        }

        float speed = isWindUpComplete ? maxFollowSpeed : minFollowSpeed;
        t += Time.deltaTime * speed;

        if (t >= 1f)
        {
            transform.position = endPoint;
            enabled = false;
            return;
        }

        Vector2 newPos = CalculateQuadraticBezierPoint(t, startPoint, controlPoint, endPoint);
        transform.position = newPos;
    }

    Vector2 CalculateQuadraticBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }
}