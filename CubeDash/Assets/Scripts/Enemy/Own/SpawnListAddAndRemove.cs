using Unity.VisualScripting;
using UnityEngine;

public class SpawnListAddAndRemove : MonoBehaviour
{
    public void AddToList(GameObject obj)
    {
        EnemySpawnManager.activeEnemies.Add(obj);
    }

    public void RemoveFromList(GameObject obj)
    {
        EnemySpawnManager.activeEnemies.Remove(obj);
    }
}
