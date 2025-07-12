using UnityEngine;

public class EXPManager : MonoBehaviour
{
    public static EXPManager instance;

    public bool debugMode;
    public int startXP = 10000;

    private const string ExpKey = "PlayerTotalEXP";
    public int totalExp { get; private set; }


    public delegate void XPChanged(int newTotal);
    public event XPChanged OnXPChanged;

    private void Awake()
    {
        // Singleton setup
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        

        instance = this;
        DontDestroyOnLoad(gameObject);

        totalExp = PlayerPrefs.GetInt(ExpKey, 0);
        OnXPChanged?.Invoke(totalExp);
    }

    private void Start()
    {
        if (debugMode)
        {
            totalExp += startXP;

            // Vergeet niet ook nu de UI te updaten:
            OnXPChanged?.Invoke(totalExp);
        }
    }
    public void AddEXP(int amount)
    {
        totalExp += amount;
        PlayerPrefs.SetInt(ExpKey, totalExp);
        PlayerPrefs.Save();

        OnXPChanged?.Invoke(totalExp); //  Notify listeners
        Debug.Log($"Total XP is now: {totalExp}");
    }

    public void ResetEXP()
    {
        totalExp = 0;
        PlayerPrefs.SetInt(ExpKey, totalExp);
        OnXPChanged?.Invoke(totalExp);
    }

    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
