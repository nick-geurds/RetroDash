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

    //public List<GameObject> meleeAttacks = new List<GameObject>();
    //public List<GameObject> rangeAttacks = new List<GameObject>();

    public GameObject[] meleeAttacks;
    public GameObject[] rangeAttacks;

    private int lastMeleeNumber = -1;
    private int lastRangeAttackNumber = -1; 

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
                if (distance < meleeThreshold)
                {
                    MeleeAttack();
                }
                else
                {
                    RangeAttack();
                }
                
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
        int getNumber = Random.Range(0, rangeAttacks.Length);
        bool validNumber = false;

        if (lastMeleeNumber != getNumber)
        {
            validNumber = true;
        }
        else if (!validNumber && lastMeleeNumber > 0)
        {
            getNumber = getNumber - 1;
            validNumber = true;
        }
        else if (!validNumber && lastMeleeNumber == 0)
        {
            getNumber = getNumber + 1;
            validNumber = true;
        }

        if (validNumber)
        {
            Instantiate(meleeAttacks[getNumber]);
        }

        lastMeleeNumber = getNumber;
        interval = Random.Range(.7f, 1.8f);


    }

    void RangeAttack()
    {
        int getNumber = Random.Range(0, rangeAttacks.Length);
        bool validNumber = false;

        if (lastRangeAttackNumber != getNumber)
        {
            validNumber = true;
        }
        else if (!validNumber && lastRangeAttackNumber > 0)
        {
            getNumber = getNumber - 1;
            validNumber = true;
        }
        else if (!validNumber && lastRangeAttackNumber == 0)
        {
            getNumber = getNumber + 1;
            validNumber = true;
        }

        if (validNumber)
        {
            Instantiate(rangeAttacks[getNumber]);
        }

        lastRangeAttackNumber = getNumber;
        interval = Random.Range(.7f, 1.8f);
    }
}
