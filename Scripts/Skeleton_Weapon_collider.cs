using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_Weapon_collider : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private Skeleton_movement skeleton;
    private bool attacked;
    //private Collider col;
    // Start is called before the first frame update

    private void Start()
    {
        skeleton = GetComponentInParent<Skeleton_movement>();
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && skeleton.attackFinish && !attacked)
        {
            //col = collider;
            collider.gameObject.GetComponent<Player_control>().takeDamage(damage);
            attacked = true;   
        }
    }

    private void Update()
    {
        if (attacked)
        {
            if (!skeleton.attackFinish) attacked = false;
        }
    }


}
