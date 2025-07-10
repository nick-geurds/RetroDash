using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "Boss/AttackData")]
public class AttackData : ScriptableObject
{
    public GameObject objectToSpawn;
    public float yOffset;
    public float delayBetweenAttacks;
    public int repeatCount;
}
