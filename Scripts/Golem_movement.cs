using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem_movement : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;
    [SerializeField] private LayerMask PlayerMask;

    //Patroling
    private Vector3 oriPoint;
    private Vector3 walkPoint;
    private bool walkPointSet;
    [SerializeField] private float walkPointRange;
    private int deg = 0;

    //Attacking
    [SerializeField] private float timeBetweenAttacks;
    private bool alreadyAttack;

    //States
    [SerializeField] private float sightRange, attackRange1, attackRange2;
    private bool playerINsight, playerINattack1, playerINattack2;
    private Animator anim;

    //equipment
    [SerializeField] private Transform RockPos;
    [SerializeField] private GameObject RockPrefab;
    private GameObject myRock;
    private AudioSource audio;
    private void Awake()
    {
        oriPoint = this.transform.position;
        //player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        audio = GetComponent<AudioSource>();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
            anim.SetBool("walk_idle", true);
        }

        //Vector3 distanceToWalkPoint = transform.position - walkPoint;
        //float dist = Mathf.Ceil(Mathf.Abs(this.transform.position.y) + 3);
        //if (distanceToWalkPoint.magnitude < dist) walkPointSet = false;
        if (agent.velocity.magnitude == 0) walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        float x = walkPointRange * Mathf.Cos(deg);
        float z = walkPointRange * Mathf.Sin(deg);
        deg += 45;
        if (deg >= 360) deg = 0;
        walkPoint = new Vector3(oriPoint.x + x, transform.position.y, oriPoint.z + z);
        walkPointSet = true;
    }
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        anim.SetBool("walk_idle", true);
    }
    private void AttackPlayer(int attack)
    {
        //enemy stay static
        agent.SetDestination(transform.position);
        anim.SetBool("walk_idle", false);
        transform.LookAt(player);

        if (!alreadyAttack)
        {
            // attack code here .......
            alreadyAttack = true;
            if (attack == 1)
            {
                anim.SetTrigger("attack1");
                StartCoroutine(GolemAttackRoutine1());
            }
            else
            {
                anim.SetTrigger("attack2");
                StartCoroutine(GolemAttackRoutine2());
            }
            
        }
    }

    private IEnumerator GolemAttackRoutine2()
    {
        
        yield return new WaitForSeconds(timeBetweenAttacks);
        alreadyAttack = false;
    }

    private IEnumerator GolemAttackRoutine1()
    {
        yield return new WaitForSeconds(0.3f);
        if (!AudioListener.pause) audio.Play();
        myRock = Instantiate(RockPrefab, RockPos.position, RockPos.rotation, RockPos.transform.parent);
        Destroy(myRock, 5);
        yield return new WaitForSeconds(1.2f);      //1.4
        myRock.GetComponent<Rock_behavior>().targetPos = player.position;
        myRock.GetComponent<Rock_behavior>().GolemPos = this.transform.position;
        myRock.GetComponent<Rock_behavior>().GolemAttackReady();
        yield return new WaitForSeconds(timeBetweenAttacks);
        alreadyAttack = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (alreadyAttack) return;
        // check for sight n attack range
        playerINsight = Physics.CheckSphere(transform.position, sightRange, PlayerMask);
        playerINattack1 = Physics.CheckSphere(transform.position, attackRange1, PlayerMask);
        playerINattack2 = Physics.CheckSphere(transform.position, attackRange2, PlayerMask);

        if (!playerINsight && !playerINattack1 && !playerINattack2) Patroling();
        else if (playerINsight && !playerINattack1 && !playerINattack2) ChasePlayer();
        else { 
            if (playerINattack2) AttackPlayer(2);
            else AttackPlayer(1);
        } 
    }
}
