using UnityEngine;

public class PlayerFollowForCam : MonoBehaviour
{
    public GameObject player;

    public float followSpeed = .3f;

    private void LateUpdate()
    {
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, player.transform.position, followSpeed * Time.deltaTime);
    }
}
