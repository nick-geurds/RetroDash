using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackController : MonoBehaviour
{
    public List<BaseAttack> availableAttacks;
    public List<BaseAttack> phase2Attacks;

    public float waitTimeMin = 0.5f;
    public float waitTimeMax = 1.5f;

    private Transform player;
    private BossHealth bossHealth;



    private enum BossState
    {
        Idle,
        Attacking,
        Waiting,
        Enraged
    }

    private BossState currentState = BossState.Idle;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        bossHealth = GameObject.FindWithTag("Boss").GetComponent<BossHealth>();

        foreach (var attack in availableAttacks)
        {
            attack.Initialize(player);
        }

        foreach (var attack in phase2Attacks)
        {
            attack.Initialize(player);
        }

        StartCoroutine(StateMachine());
    }

    private IEnumerator StateMachine()
    {
        while (true)
        {
            if (bossHealth.isPhase2TransitionActive)
            {
                // Pauzeer alle acties zolang de transitie actief is
                yield return null; // wacht een frame
                continue;          // ga opnieuw checken in de volgende frame
            }

            switch (currentState)
            {
                case BossState.Idle:
                    yield return new WaitForSeconds(Random.Range(waitTimeMin, waitTimeMax));
                    currentState = BossState.Attacking;
                    break;

                case BossState.Attacking:
                    List<BaseAttack> attacks;

                    if (bossHealth.currentHealth <= bossHealth.maxHealth * 0.5f)
                    {
                        if (bossHealth.isPhase2Ready)
                            attacks = phase2Attacks;
                        else
                            attacks = availableAttacks;
                    }
                    else
                    {
                        attacks = availableAttacks;
                    }

                    if (attacks == null || attacks.Count == 0)
                    {
                        Debug.LogError("No attacks assigned!");
                        currentState = BossState.Waiting;
                        yield break;
                    }

                    BaseAttack selected = ChooseSmartAttack(attacks);

                    if (selected == null)
                    {
                        Debug.LogError("ChooseSmartAttack returned null!");
                        currentState = BossState.Waiting;
                        yield break;
                    }

                    Debug.Log("Boss executes attack: " + selected.name);

                    yield return StartCoroutine(selected.ExecuteAttack());

                    currentState = BossState.Waiting;
                    break;

                case BossState.Waiting:
                    yield return new WaitForSeconds(Random.Range(waitTimeMin, waitTimeMax));
                    currentState = BossState.Idle;
                    break;

                case BossState.Enraged:
                    // Optioneel: snellere of extra aanvallen
                    break;
            }

            yield return null;
        }
    }

    private BaseAttack ChooseSmartAttack(List<BaseAttack> attacks)
    {
        float distance = Vector2.Distance(player.position, transform.position);

        List<BaseAttack> closeRange = attacks.FindAll(a => a.attackType == AttackType.Melee);
        List<BaseAttack> longRange = attacks.FindAll(a => a.attackType == AttackType.Ranged);

        if (distance < 3f && closeRange.Count > 0)
            return closeRange[Random.Range(0, closeRange.Count)];

        if (distance >= 3f && longRange.Count > 0)
            return longRange[Random.Range(0, longRange.Count)];

        return attacks[Random.Range(0, attacks.Count)];
    }
}
