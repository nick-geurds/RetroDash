using UnityEngine;

public class MapBounds : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        var col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
