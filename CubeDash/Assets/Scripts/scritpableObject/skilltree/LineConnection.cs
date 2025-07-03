using UnityEngine;
using UnityEngine.UI;

public class LineConnection : MonoBehaviour
{
    public SkillUpgrade from;
    public SkillUpgrade to;
    public Image image;

    private SkillTreeManager manager;

    public void Initialize(SkillTreeManager manager)
    {
        this.manager = manager;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (manager == null || from == null || to == null) return;

        bool fromVisible = manager.IsVisible(from);
        bool toVisible = manager.IsVisible(to);

        gameObject.SetActive(fromVisible && toVisible);

        if (!gameObject.activeSelf)
            return;

        bool fromUnlocked = manager.IsUnlocked(from);
        bool toUnlocked = manager.IsUnlocked(to);

        image.color = (fromUnlocked && toUnlocked)
            ? new Color(0f, 0.937f, 1f, 1f)   // cyaan
            : new Color(1f, 1f, 1f, 1f);     // wit
    }
}
