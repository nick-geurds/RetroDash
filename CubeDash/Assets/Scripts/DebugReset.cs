using UnityEngine;

public class DebugReset : MonoBehaviour
{
    void Update()
    {
        // Druk op R + Shift om alles te resetten
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.LogWarning("PlayerPrefs volledig gereset!");
        }
    }
}
