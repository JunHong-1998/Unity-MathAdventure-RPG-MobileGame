using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock_behavior : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    private Vector3 shootPos;
    [HideInInspector] public bool attack_ready;
    [HideInInspector] public Vector3 targetPos;
    [HideInInspector] public Vector3 GolemPos;
    private GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
    }
    private void Update()
    {
        if (attack_ready)
        {
            //Rigidbody rb = GetComponent<Rigidbody>();
            
            Vector3 shootDir = (targetPos - shootPos).normalized;
            transform.position += shootDir * speed * Time.deltaTime;
            //shootPos = this.transform.position;
            //rb.AddForce(shootDir * speed, ForceMode.Impulse);
            //rb.velocity = transform.right * speed * Time.deltaTime;
            //rb.AddForce(transform.right * speed, ForceMode.Impulse);
        }
    }
    public void GolemAttackReady()
    {
        //yield return new WaitForSeconds(1.4f);      //1.4
        shootPos = new Vector3(GolemPos.x, this.transform.position.y, GolemPos.z);
        if (GolemPos.y<targetPos.y+1.5f) targetPos.y += 5f;
        else targetPos.y = Mathf.Max(this.transform.position.y-0.5f, targetPos.y-0.5f);
        attack_ready = true;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (attack_ready && collider.CompareTag("Player"))
        {
            Vector3 hitpoint = collider.gameObject.GetComponent<Collider>().ClosestPointOnBounds(player.transform.position);
            Vector3 dir = hitpoint - player.transform.position;
            player.GetComponent<CharacterController>().Move(-dir.normalized * 5 * Time.deltaTime);
            player.GetComponent<Player_control>().takeDamage(damage);
            GameObject.Destroy(this.gameObject);
        }
    }

    /*private void OnTriggerEnter(Collider collider)
     {
         GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
         foreach (GameObject ply in player)
         {
             if (collider.gameObject == ply)
             {
                 print("hit ar~");
                 Destroy(gameObject);
                 break;
             }
         }


     }*/
}
