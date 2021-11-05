using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rhino_movement : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask GroundMask, PlayerMask;
    [SerializeField] private int health;
    [SerializeField] private int moveSpeed;

    //Patroling
    [SerializeField] private Vector3 walkPoint;
    [SerializeField] private bool walkPointSet;
    [SerializeField] private float walkPointRange;

    //Attacking
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private bool alreadyAttack;

    //States
    private float sightRange, attackRange;
    private bool playerINsight, playerINattack;
    private CharacterController controller;
    private Animator anim;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
            anim.SetInteger("walk_run", 1);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        // calc random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, GroundMask)) walkPointSet = true;
    }
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        //agent.speed = moveSpeed;
    }
    private void AttackPlayer()
    {
        //enemy stay static
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttack)
        {
            // attack code here .......
            // *************************************************************************
            alreadyAttack = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttack = false;
    }

    // Update is called once per frame
    private void Update()
    {
        // check for sight n attack range
        playerINsight = Physics.CheckSphere(transform.position, sightRange, PlayerMask);
        playerINattack = Physics.CheckSphere(transform.position, attackRange, PlayerMask);

        if (!playerINsight && !playerINattack) Patroling();
        else if (playerINsight && !playerINattack) ChasePlayer();
        else AttackPlayer();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Invoke(nameof(Destroy), 2f);   // dead anim length
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
