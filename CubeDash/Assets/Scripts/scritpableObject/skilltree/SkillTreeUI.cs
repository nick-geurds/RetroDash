using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class SkillTreeUI : MonoBehaviour
{
    public SkillTreeManager treeManager;
    public GameObject upgradeButtonPrefab;
    public GameObject linePrefab;

    public GameObject confirmationPanel;
    public TextMeshProUGUI confirmationTitle;
    public TextMeshProUGUI confirmationDescription;
    public Button confirmButton;
    public Button cancelButton;

    public RectTransform confirmationPanelRect;

    private SkillUpgrade selectedUpgrade;

    public RectTransform contentParent;
    public RectTransform lineContainer;    // apart object in je Canvas
    public RectTransform buttonContainer;  // apart object in je Canvas

    private Dictionary<SkillUpgrade, UpgradeButtonUI> buttonDict = new();
    private List<LineConnection> allLines = new List<LineConnection>();

    void Start()
    {
        treeManager.Initialize();
        BuildTree();
        UpdateUI();

        foreach (var line in Object.FindObjectsByType<LineConnection>(sortMode: FindObjectsSortMode.None))
        {
            line.Initialize(treeManager);
        }
    }

    void BuildTree()
    {
        foreach (var upgrade in treeManager.skillTree.allUpgrades)
        {
            // Instantieer knop in juiste container
            GameObject go = Instantiate(upgradeButtonPrefab);
            go.transform.SetParent(buttonContainer.transform, false);
            go.transform.SetAsLastSibling(); // ZET BUTTON BOVENAAN (zichtbaar)

            var btnUI = go.GetComponent<UpgradeButtonUI>();
            btnUI.Setup(upgrade, this);
            buttonDict[upgrade] = btnUI;
        }

        DrawLines();
        UpdateUI();
    }

    void DrawLines()
    {
        allLines.Clear();

        foreach (var upgrade in treeManager.skillTree.allUpgrades)
        {
            var from = buttonDict[upgrade].transform;
            foreach (var next in upgrade.unlocksAfterThis)
            {
                if (!buttonDict.ContainsKey(next)) continue;

                var to = buttonDict[next].transform;

                GameObject line = Instantiate(linePrefab);
                line.transform.SetParent(lineContainer.transform, false);
                line.transform.SetAsFirstSibling(); // lijn achteraan

                var img = line.GetComponent<Image>();
                if (img != null)
                    img.raycastTarget = false;

                Vector2 start = from.localPosition;
                Vector2 end = to.localPosition;
                Vector2 dir = end - start;
                float length = dir.magnitude;

                RectTransform rect = line.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(length, 5f);
                rect.pivot = new Vector2(0, 0.5f);
                rect.localPosition = start;
                rect.localRotation = Quaternion.FromToRotation(Vector3.right, dir);

                LineConnection connection = line.GetComponent<LineConnection>();
                connection.from = upgrade;
                connection.to = next;
                connection.image = img;

                connection.Initialize(treeManager);

                allLines.Add(connection);
            }
        }
    }

    public void OnUpgradeClicked(SkillUpgrade upgrade)
    {
        if (treeManager.CanUnlock(upgrade))
        {
            treeManager.Unlock(upgrade);
            UpdateUI();

            if (buttonDict.TryGetValue(upgrade, out var btnUI))
            {
                btnUI.transform.SetAsLastSibling();
            }
        }
    }


    public void UpdateUI()
    {
        var unlockable = treeManager.GetUnlockableUpgrades();
        var visibleUpgrades = treeManager.GetVisibleUpgrades(2);  // zelfde depth als in je upgrade logic

        // Update buttons
        foreach (var pair in buttonDict)
        {
            var upgrade = pair.Key;
            var ui = pair.Value;

            bool isUnlocked = treeManager.IsUnlocked(upgrade);
            bool canUnlock = unlockable.Contains(upgrade);
            bool isVisible = visibleUpgrades.Contains(upgrade);

            // Zet button actief of niet (active = zichtbaar)
            ui.gameObject.SetActive(isVisible);

            if (!isVisible)
                continue;

            ui.SetInteractable(isUnlocked, canUnlock);

            if (isUnlocked)
                ui.SetColor(new Color(0f, 0.937f, 1f, 1f)); // cyaan
            else if (canUnlock)
                ui.SetColor(new Color(1f, 1f, 1f, 1f));     // wit
            else
                ui.SetColor(new Color(0.2f, 0.2f, 0.2f, 1f)); // grijs
        }

        // Update lijnen
        foreach (var line in allLines)
        {
            line.UpdateVisual();
        }
    }

    void LateUpdate()
    {
        lineContainer.SetAsFirstSibling();
        buttonContainer.SetAsLastSibling();

        foreach (var pair in buttonDict)
        {
            pair.Value.transform.SetAsLastSibling();
        }
    }

    private HashSet<SkillUpgrade> CalculateVisibleUpgradesWithDepthLimit(int maxDepth = 3)
    {
        var visible = new HashSet<SkillUpgrade>();

        // Queue voor BFS-like traversal: (upgrade, huidige depth)
        Queue<(SkillUpgrade upgrade, int depth)> queue = new Queue<(SkillUpgrade, int)>();

        // Voeg alle unlocked upgrades toe als startpunt (depth 0)
        foreach (var upgrade in treeManager.skillTree.allUpgrades)
        {
            if (treeManager.IsUnlocked(upgrade))
            {
                visible.Add(upgrade);
                queue.Enqueue((upgrade, 0));
            }
        }

        while (queue.Count > 0)
        {
            var (current, depth) = queue.Dequeue();

            if (depth >= maxDepth)
                continue;

            if (current.unlocksAfterThis != null)
            {
                foreach (var next in current.unlocksAfterThis)
                {
                    if (!visible.Contains(next))
                    {
                        visible.Add(next);
                        queue.Enqueue((next, depth + 1));
                    }
                }
            }
        }

        return visible;
    }

    public void OnUpgradeSelected(SkillUpgrade upgrade)
    {
        selectedUpgrade = upgrade;
        confirmationTitle.text = upgrade.upgradeName;
        confirmationDescription.text = upgrade.description;

        if (buttonDict.TryGetValue(upgrade, out var btnUI))
        {
            Vector3 worldPos = btnUI.transform.position;

            RectTransform canvasRect = confirmationPanelRect.parent.GetComponent<RectTransform>();
            Vector2 localPoint;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Camera.main.WorldToScreenPoint(worldPos), Camera.main, out localPoint);

            // Bereken de offset: 
            float buttonHeight = btnUI.GetComponent<RectTransform>().rect.height;
            float panelHeight = confirmationPanelRect.rect.height;

            // Zet panel boven de knop met voldoende ruimte (panel hoogte + kleine marge)
            float offsetY = buttonHeight * 0.5f + panelHeight * 0.5f + 20f; // 10 is marge, pas aan naar wens

            confirmationPanelRect.localPosition = localPoint + new Vector2(0, offsetY);
        }

        confirmationPanel.SetActive(true);

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => ConfirmPurchase());

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() => CancelPurchase());
    }

    // Wordt aangeroepen als speler op confirm knop drukt: unlock de upgrade
    void ConfirmPurchase()
    {
        Debug.Log("Confirm clicked!");
        if (selectedUpgrade != null && treeManager.CanUnlock(selectedUpgrade))
        {
            treeManager.Unlock(selectedUpgrade);
            UpdateUI();
        }
        confirmationPanel.SetActive(false);
    }

    void CancelPurchase()
    {
        Debug.Log("Cancel clicked!");
        confirmationPanel.SetActive(false);
    }
}
