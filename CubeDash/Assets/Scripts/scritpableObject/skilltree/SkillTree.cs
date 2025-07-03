using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Skill Tree", menuName = "Skill Tree/Tree")]
public class SkillTree : ScriptableObject
{
    public List<SkillUpgrade> allUpgrades;
}
