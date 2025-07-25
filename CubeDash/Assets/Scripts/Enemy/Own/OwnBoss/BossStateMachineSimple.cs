using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BossStateMachineSimple : MonoBehaviour
{
    private GameObject player;
    public float meleeThreshold = 3f;
    public enum State {melee , range, special, idle}
    private State CurrentState = State.idle;

    
    private float interval;
    public float intervalTimer = 0f;

    public List<GameObject> meleeAttacks = new List<GameObject>();
    public List<GameObject> rangeAttacks = new List<GameObject>();
    public List<GameObject> specialAttacks = new List<GameObject>();

    [HideInInspector] public List<GameObject> activeAttacks = new List<GameObject>();

    

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    private void Update()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        


        if (activeAttacks.Count == 0)
        {
            interval -= Time.deltaTime;

            if (interval <= intervalTimer)
            {
                MeleeAttack();
            }
        }
            
        //else if (distance <= meleeThreshold)
        //{
        //    MeleeAttack();
        //}
        //else if (distance > meleeThreshold)
        //{
        //    RangeAttack();
        //}
        
    }
    void MeleeAttack()
    {
       int getNumber = Random.Range(0, meleeAttacks.Count);
       Instantiate(meleeAttacks[getNumber]);
       interval = Random.Range(.7f, 1.8f);
    }

    void RangeAttack()
    {
        int getNumber = Random.Range(0, rangeAttacks.Count - 1);
        Instantiate(rangeAttacks[getNumber]);
        interval = Random.Range(.7f, 1.8f);
    }
}
