using TMPro;
using UnityEngine;

public class ExpUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text xpText;
    private EXPManager expManager;


    private void OnEnable()
    {
        expManager = GameObject.FindWithTag("ExpManager").GetComponent<EXPManager>();

        if (expManager != null)
        {
           expManager.OnXPChanged += UpdateXPText;
            UpdateXPText(expManager.totalExp);
        }

        if (expManager != null)
        {
            UpdateXPText(expManager.totalExp);
        }
    }

    private void OnDisable()
    {
        if (expManager != null)
        {
            expManager .OnXPChanged -= UpdateXPText;
        }
    }

    private void UpdateXPText(int xp)
    {
        if (xpText != null)
            xpText.text = $"XP: {xp}";
        else
            Debug.LogWarning("[ExpUIController] xpText is not assigned in the inspector.");
    }
}
