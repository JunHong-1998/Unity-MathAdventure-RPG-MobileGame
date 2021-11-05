using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Skeleton_movement : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private int maxHealth;
    private int currentHealth;
    [SerializeField] private HealthBAR healthBAR;
    [SerializeField] private GameObject damageShow_Prefab, damageShow_parent;
    [SerializeField] private ParticleSystem[] ps;

    private GameObject player;
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
    [HideInInspector] public bool alreadyAttack, queeue, dead, kena1, kena2, attackFinish;

    //States
    [SerializeField] private float sightRange, attackRange;
    private bool playerINsight, playerINattack,  playerGUILTY;
    private Animator anim;
    private List<int> damageSHOW = new List<int>();
    private GameObject showDamage;
    private AudioSource audio;

    private void Awake()
    {
        
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        if (!agent.Warp(this.transform.position))
        {
            GameObject.Destroy(this.gameObject);
            Debug.Log("Spawn wrong place, killed");
        }
        oriPoint = this.transform.position;
    }

    private void Start()
    {
        player = GameObject.Find("Player");
        currentHealth = maxHealth;
        healthBAR.SetMaxHealth(maxHealth);
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
        GameObject platform = GameObject.Find("Platform");
        Vector3 dir = platform.transform.position - this.transform.position;
        float dist = dir.sqrMagnitude;
        if (dist < 90 && agent.velocity.magnitude == 0)
        {
            playerGUILTY = true;
            
        }
        else agent.SetDestination(player.transform.position);

        anim.SetBool("walk_idle", true);
    }
    private void AttackPlayer()
    {
        //enemy stay static
        agent.SetDestination(transform.position);
        anim.SetBool("walk_idle", false);
        transform.LookAt(player.transform);

        if (!alreadyAttack)
        {
            // attack code here .......
            alreadyAttack = true;
            StartCoroutine(SkeletonAttackRoutine());
            

        }
    }


    private IEnumerator SkeletonAttackRoutine()
    {
        anim.SetTrigger("attack");
        yield return new WaitForSeconds(0.2f);
       
        attackFinish = true;
        yield return new WaitForSeconds(0.9f);
        audio.clip = clips[0];
        if (!AudioListener.pause) audio.Play();
        attackFinish = false;
        yield return new WaitForSeconds(timeBetweenAttacks);
        alreadyAttack = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (alreadyAttack || dead) return;
        if (!queeue && damageSHOW.Count > 0) StartCoroutine(DestroyDamageSHow());
        // check for sight n attack range
        playerINsight = Physics.CheckSphere(transform.position, sightRange, PlayerMask);
        playerINattack = Physics.CheckSphere(transform.position, attackRange, PlayerMask);
        

        if (!playerINsight && !playerINattack || playerGUILTY) Patroling();
        else if (playerINsight && !playerINattack) ChasePlayer();
        else AttackPlayer();
        if (playerGUILTY)
        {
            if (!playerINsight && !playerINattack)
            {
                playerGUILTY = false;
            }
        }
    }


    public void takeDamage(int damage)
    {
        if (dead) return;
        damageSHOW.Add(damage);
        if (!alreadyAttack) anim.SetTrigger("hit");
        currentHealth -= damage;
        healthBAR.setHealth(currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            dead = true;
            agent.SetDestination(transform.position);
            StartCoroutine(FallEffect());
        }
    }

    private IEnumerator DestroyDamageSHow()
    {
        queeue = true;
        showDamage = Instantiate(damageShow_Prefab, damageShow_parent.transform);
        showDamage.GetComponent<Text>().text = "-" + damageSHOW[0];
        yield return new WaitForSecondsRealtime(0.3f);
        GameObject.Destroy(showDamage);
        damageSHOW.RemoveAt(0);
        queeue = false;
    }
    private IEnumerator FallEffect()
    {
        anim.SetTrigger("hit");
        yield return new WaitForSeconds(0.3f);
        anim.SetTrigger("fall");
        yield return new WaitForSeconds(2);
        ps[0].Play();
        //audio.clip = clips[1];
        if (!AudioListener.pause) audio.PlayOneShot(clips[1]);
        GameObject.Destroy(GetComponent<Transform>().GetChild(0).gameObject);
        yield return new WaitForSeconds(2.1f);
        GameObject.Destroy(this.gameObject);
        GameObject GM = GameObject.Find("Game Manager");
        GM.GetComponent<Game_Manager>().FinalScore += 100;
    }

}
