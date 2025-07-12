using UnityEngine;

public class BossCanvas : MonoBehaviour
{
    private Canvas canvas;
    private Camera mainCam;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        mainCam = Camera.main;

        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = mainCam;
    }
}
