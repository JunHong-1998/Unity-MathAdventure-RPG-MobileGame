using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Weapon_collider : MonoBehaviour
{
    [SerializeField] private Player_control ply;
    [SerializeField] private int damage, weapon_type, attackType;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag("Enemy") || !ply.isAttack) return;
        if (!(attackType == ply.attackType || attackType == 0 && ply.attackType == 1)) return;
        bool kill, kena;

        kill = kena = false;
        if (weapon_type == 1)
        {
            kill = ply.Kill1;
            kena = collider.gameObject.GetComponent<Skeleton_movement>().kena1;
        }
        else if (weapon_type == 2)
        {
            kill = ply.Kill2;
            kena = collider.gameObject.GetComponent<Skeleton_movement>().kena2;
        }
        if (ply.attackType == 0) damage = 75;
        /*else if (weapon_type == 3)
        {
            kill = ply.Kill1;
            kena = collider.gameObject.GetComponent<Skeleton_movement>().kena3;
        }*/
        if (attackType == 3 && ply.isAttack && kill )
        {
            collider.gameObject.GetComponent<Skeleton_movement>().takeDamage(damage);
            
            StartCoroutine(ResetTrigger(collider));
        }
        else if (ply.isAttack && !kena && kill)
        {
            collider.gameObject.GetComponent<Skeleton_movement>().takeDamage(damage);
            if (weapon_type == 1)
            {
                collider.gameObject.GetComponent<Skeleton_movement>().kena1 = true;
            }
            else if (weapon_type == 2)
            {
                collider.gameObject.GetComponent<Skeleton_movement>().kena2 = true;
            }
            /*else if (weapon_type == 3)
            {
                collider.gameObject.GetComponent<Skeleton_movement>().kena3 = true;
            }*/
        }
    }

    private IEnumerator ResetTrigger(Collider other)
    {
        other.enabled = false;
        yield return new WaitForSecondsRealtime(0.4f);
        other.enabled = true;
    }

}
