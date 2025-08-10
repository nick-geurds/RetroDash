using System.Collections;
using UnityEngine;

public class SimpleShockWave : MonoBehaviour
{
    public GameObject schockWave;

    public AnimationCurve curve;
    public float schockDuration;

    public float schockScale;
    private Vector3 endScale;

    public bool debugMode;
    private Transform player;

    private PlayerDataSimple playerData;
    [HideInInspector] public bool canShockWave = false;

    private void Start()
    {
        player = FindFirstObjectByType<SimpleShockWave>().transform;
        StartCoroutine(InitializeStats());
    }

    private void Update()
    {
        if (!debugMode)
            return;

        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(SchokWave());
        }
    }
    private IEnumerator InitializeStats()
    {
        yield return null;
        canShockWave = PlayerDataSimple.Instance.canShockWave;
    }

    public IEnumerator SchokWave()
    {
        if (!canShockWave)
            yield break;

        KnockBack knockback = GetComponent<KnockBack>();
        yield return new WaitForSeconds(knockback.knockBackDuration);

        GameObject shockWave = Instantiate(schockWave);

        float t = 0;

        shockWave.transform.position = player.transform.position;

        endScale = shockWave.transform.localScale * schockScale;

        while (t < schockDuration)
        {
            float animationCurve = curve.Evaluate(t / schockDuration);

            shockWave.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, endScale, animationCurve);
            t += Time.deltaTime;
            yield return null;
        }

        shockWave.transform.localScale = endScale;
        yield return null;
        Destroy(shockWave);
    }
}
