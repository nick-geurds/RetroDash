using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public float delayTime;

    public void LoadSceneWithDelay(string scenename)
    {
        StartCoroutine(Delay(scenename));
    }

    IEnumerator Delay(string scenename)
    {
        yield return new WaitForSeconds(delayTime);

        LoadScene(scenename);
    }

    public void LoadScene(string scenename)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(scenename);
    }
}
