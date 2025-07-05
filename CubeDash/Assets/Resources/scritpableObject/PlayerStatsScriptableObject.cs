using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "New PlayerSo", menuName = "Player")]
public class PlayerStatsScriptableObject : ScriptableObject

{
    public float attackAmount = 1f;
    public float maxHealth = 5;
    public float dashDis = 8f;
    public float dashInterval = 1f;
    public float dashSpeed = 50f;
}
