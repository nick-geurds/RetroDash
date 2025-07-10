using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackController : MonoBehaviour
{
    public List<BaseAttack> availableAttacks;

    private Transform player;

    private void Start()
    {
        player = GameObject.Find("Player").transform;

        foreach (var attack in availableAttacks)
        {
            attack.Initialize(player);
        }

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            // Kies random aanval
            BaseAttack selectedAttack = availableAttacks[Random.Range(0, availableAttacks.Count)];
            yield return StartCoroutine(selectedAttack.ExecuteAttack());

            yield return new WaitForSeconds(2f); // Pauze tussen aanvallen
        }
    }
}
