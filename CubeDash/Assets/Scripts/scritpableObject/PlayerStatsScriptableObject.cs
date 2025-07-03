using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "New PlayerSo", menuName = "Player")]
public class PlayerStatsScriptableObject : ScriptableObject

{
    public float attackAmount = 1f;
    public float maxHealth;
    public float dashDis = .2f;
    public float dashInterval = 1.5f;
    public float dashSpeed = 10f;
}
