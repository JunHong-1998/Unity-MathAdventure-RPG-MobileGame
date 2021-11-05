using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CheckPoint : MonoBehaviour
{
    [SerializeField] private Game_Manager GM;
    [SerializeField] private float playerRange;
    [SerializeField] private LayerMask PlayerMask;
    [SerializeField] private ParticleSystem[] ps;
    [SerializeField] private Color color;
    [SerializeField] private GameObject diamond;
    [HideInInspector] public int StationNumber;

    // Update is called once per frame
    void Update()
    {
        if (StationNumber==GM.checkPOINT && !GM.isPaused)
        {
            if (PlayerInRange()) 
            {
                GM.currentCheckPoint = this.gameObject;
                GM.currentDiamond = diamond;
                
                GM.isPaused = true;
                GM.ps = ps;
                GM.ps[1].startColor = new Color(color.r, color.g, color.b, color.a);
                GM.ps[0].Play();
                PlayerPrefs.SetString("navigate", "MathGame");
                StartCoroutine(GM.ReadyToChangeScene());
                
                
            }
            
        }
        
    }

    

    private bool PlayerInRange()
    {
        return Physics.CheckSphere(transform.position, playerRange, PlayerMask);
    }

}
