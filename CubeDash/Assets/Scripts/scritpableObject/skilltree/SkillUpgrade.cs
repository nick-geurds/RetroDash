using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Skill Upgrade", menuName = "Skill Tree/Upgrade")]
public class SkillUpgrade : ScriptableObject
{
    public int upgradeIndex;

    public string upgradeName;
    public string description;

    public Sprite symbol;

    public bool isUnlockedAtStart;

    // Welke upgrades komen vrij als deze wordt gekocht
    public List<SkillUpgrade> unlocksAfterThis;

    // Voor visualisatie (optioneel)
    public Vector2 uiPosition;
}
