using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_control : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [HideInInspector] public int currentHealth;
    [SerializeField] public HealthBAR healthBAR;
    [SerializeField] private GameObject damageShow_Prefab, damageShow_parent;
    [SerializeField] private float moveSpeed;
    [SerializeField] public Joystick joystick;
    [SerializeField] private Transform camera;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] public ParticleSystem[] particleSystem;
    [SerializeField] private Button[] AttackBtn;
    [SerializeField] private SphereCollider attack3Collider;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private Image[] buttonCover;
    //[SerializeField] private ParticleSystem ps_attack1_2;
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private float gravity = -9.81f;
    private float groundDistance = 0.4f;
    private Vector3 velocity;
    private bool isGrounded;
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Animator anim;
    [HideInInspector] public bool moveAllow = true;
    [HideInInspector] public bool isAttack, Kill1, Kill2, Heal, queeue, gameStart;
    [HideInInspector] public int attackType = -1;
    private List<string> healthSHOW = new List<string>();
    private GameObject showHealthText;
    private Collider healCollider;
    [HideInInspector] public AudioSource audio;

    private void Awake()
    {
        foreach (ParticleSystem ps in particleSystem)
        {
            ps.Stop();
        }
    }
    private void Start()
    {
        //currentHealth = maxHealth;
        ResetPlayer();
        currentHealth = 0;
        healthBAR.setHealth(0);
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        audio = GetComponent<AudioSource>();
    }

    public void ResetPlayer()
    {
        healthBAR.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
        if (Heal) StartCoroutine(HealPlayer());
        if (!queeue && healthSHOW.Count > 0 ) StartCoroutine(healthShowShow());
    }

    private void Move()
    {
        if (moveAllow && !isAttack)
        {
            float moveZ = joystick.Vertical;
            float moveX = joystick.Horizontal;
            Vector3 direction = new Vector3(moveX, 0, moveZ).normalized;

            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            if (isGrounded && velocity.y < 0)
            {
                velocity.y -= 2f;
            }
            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                Vector3 moveDir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

                controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
            }
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            //moveDirection = transform.TransformDirection(moveDirection);

            if (direction != Vector3.zero)
            {
                Walk();
            }
            else
            {
                Idle();
            }
        }
        
    }

    private void Idle()
    {
        anim.SetBool("walk", false);
    }

    private void Walk()
    {
        anim.SetBool("walk", true);
    }

    public void Attack(int attack)
    {
        attackType = attack;
        if (!isAttack)
        {
            isAttack = true;
            if (attackType!=0) StartCoroutine(btn_CoolDown(attack));
            GameObject[] skeleton = GameObject.FindGameObjectsWithTag("Enemy");
            foreach(GameObject skel in skeleton)
            {
                skel.GetComponent<Skeleton_movement>().kena1 = false;
                skel.GetComponent<Skeleton_movement>().kena2 = false;
            }
            checkClosetEnemy();
            StartCoroutine(AttackRoutine());
        }
        
    }

    private void checkClosetEnemy()
    {
        Transform targetEnemy = null;
        float closetDistance = 18;
        GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enem in enemy)
        {
            if (enem.GetComponent<Skeleton_movement>().dead) continue;
            Vector3 dir = enem.transform.position - this.transform.position;
            float dist = dir.sqrMagnitude;
            if (dist < closetDistance)
            {
                closetDistance = dist;
                targetEnemy = enem.transform;
            }
        }
        if (targetEnemy != null) transform.LookAt(targetEnemy); 
    }
    private IEnumerator AttackRoutine()
    {
        if (attackType ==0)
        {
            anim.SetTrigger("attack0");
            //particleSystem[1].Play();
            Kill1 = true;
            yield return new WaitForSecondsRealtime(0.3f);
            if (!AudioListener.pause) audio.PlayOneShot(clips[6], 0.5f);
            yield return new WaitForSecondsRealtime(0.7f);
            //particleSystem[1].Stop();
        }
        else if (attackType == 1)
        {
            anim.SetTrigger("attack1");
            yield return new WaitForSecondsRealtime(0.4f);
            //audio.clip = ;
            if (!AudioListener.pause) audio.PlayOneShot(clips[0]);
            particleSystem[0].Play();
            Kill1 = true;
            yield return new WaitForSecondsRealtime(0.4f);
            //audio.clip = clips[1];
            if (!AudioListener.pause) audio.PlayOneShot(clips[1]);
            particleSystem[1].Play();
            Kill2 = true;
            yield return new WaitForSecondsRealtime(0.3f);
            particleSystem[1].Stop();
            yield return new WaitForSecondsRealtime(0.7f);    //1.8f
        }
        else if (attackType==2)
        {
            anim.SetTrigger("attack2");
            //audio.clip = clips[2];
            if (!AudioListener.pause) audio.PlayOneShot(clips[2]);
            yield return new WaitForSecondsRealtime(0.3f);
            particleSystem[2].Play();
            
            yield return new WaitForSecondsRealtime(0.3f);
            particleSystem[4].Play();
            yield return new WaitForSecondsRealtime(0.1f);
            particleSystem[2].Stop();
            particleSystem[3].Play();
            yield return new WaitForSecondsRealtime(0.5f);
            Kill1 = true;
            yield return new WaitForSecondsRealtime(1.3f); //2.5f
            particleSystem[3].Stop();
            particleSystem[4].Stop();
        }
        else
        {
            
            Kill1 = true;
            particleSystem[6].Play();
            //audio.clip = clips[3];
            if (!AudioListener.pause) audio.PlayOneShot(clips[3]);
            yield return new WaitForSecondsRealtime(0.3f);
            attack3Collider.enabled = true;
            anim.SetTrigger("attack3");
            yield return new WaitForSecondsRealtime(2.5f);
            attack3Collider.enabled = false;
        }
        isAttack = Kill1 = Kill2 = false;
    }

    private IEnumerator btn_CoolDown(int attack)
    {
        float coolTime = 0;
        float runTime = 0;
        AttackBtn[attack].enabled = false;
        buttonCover[attack - 1].fillAmount = 1;
        if (attack == 1) coolTime = runTime = 5f;
        else if (attack == 2) coolTime = runTime = 8f;
        else if (attack == 3) coolTime = runTime = 15f;
        while (runTime>0)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            buttonCover[attack - 1].fillAmount = runTime/coolTime;
            runTime -= 0.1f;
        }
        buttonCover[attack - 1].fillAmount = 0;

        AttackBtn[attack].enabled = true;
    }


    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Golem_body"))  
        {            
            Vector3 dir = collider.gameObject.transform.position - transform.position;
            controller.Move(-dir.normalized * 15 * Time.deltaTime);
            takeDamage(300);
        }
        else if (collider.gameObject.name == "Platform" && gameStart)
        {
            Heal = true;
            healCollider = collider;
        }
        
    }

    public void takeDamage(int damage)
    {
        if (currentHealth > 0)
        {
            //audio.clip = clips[4];
            if (!AudioListener.pause) audio.PlayOneShot(clips[4]);
            healthSHOW.Add("-" + damage);
            currentHealth -= damage;
            healthBAR.setHealth(currentHealth);
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                moveAllow = false;
            }
        }
        
    }

    private IEnumerator HealPlayer()
    {
        yield return new WaitForSeconds(1f);
        if (Heal && currentHealth <maxHealth)
        {
            //audio.clip = clips[5];
            currentHealth += 10;
            healthBAR.setHealth(currentHealth);
            Heal = false;
            healCollider.enabled = false;
            healCollider.enabled = true;
        }
    }

    private IEnumerator healthShowShow()
    {
        queeue = true;
        showHealthText = Instantiate(damageShow_Prefab, damageShow_parent.transform);
        showHealthText.GetComponent<Text>().text = healthSHOW[0];
        yield return new WaitForSecondsRealtime(0.3f);
        GameObject.Destroy(showHealthText);
        healthSHOW.RemoveAt(0);
        queeue = false;
    }
}
