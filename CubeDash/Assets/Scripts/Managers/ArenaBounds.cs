using UnityEngine;

public class ArenaBounds : MonoBehaviour
{
    public Vector2 minBounds; // bijvoorbeeld linker onderhoek van de arena
    public Vector2 maxBounds; // rechter bovenhoek van de arena

    public static ArenaBounds Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Clamp positie binnen arena
    public Vector3 ClampPosition(Vector3 position)
    {
        float clampedX = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
        return new Vector3(clampedX, clampedY, position.z);
    }

    private void OnDrawGizmos()
    {
        // Kleur van de gizmo
        Gizmos.color = Color.cyan;

        // Bereken center en size van de box op basis van min en max bounds
        Vector3 center = new Vector3(
            (minBounds.x + maxBounds.x) / 2f,
            (minBounds.y + maxBounds.y) / 2f,
            0f);

        Vector3 size = new Vector3(
            Mathf.Abs(maxBounds.x - minBounds.x),
            Mathf.Abs(maxBounds.y - minBounds.y),
            0.1f); // kleine dikte voor de box

        Gizmos.DrawWireCube(center, size);
    }
}
