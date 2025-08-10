using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UpgradeUIButtonSimple : MonoBehaviour
{
    
    public UpgradesSimple Upgrades;

    public Image upgradeIcon;
    public Sprite iconSprite;
    public Color upgradeColor;

    private Button button;

    [Header("Confirm Settings")]
    private GameObject ConfirmPanel;
    private Button confirmButton;
    private Button cancelButton;

    private void Start()
    {
        transform.position = Upgrades.position;
        button = GetComponent<Button>();
        ConfirmPanel = GameObject.Find("ConfirmPanel");
        confirmButton = GameObject.Find("ConfirmButton").GetComponent<Button>();
        cancelButton = GameObject.Find("CancelButton").GetComponent<Button>();

        ConfirmPanel.SetActive(false);
    }

    public void PopUpConfirmPanel()
    {
        ConfirmPanel.SetActive(true);
        ConfirmPanel.transform.position = Upgrades.position + new Vector2(0, 20);
    }



}
