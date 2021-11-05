using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragnDrop : MonoBehaviour
{
    private Vector3 initPOS;
    [HideInInspector] public Vector3 newPOS;
    [HideInInspector] public bool correct;
    //private AudioSource audio;
    bool moveAllowed;
    bool locked;
    [SerializeField] public int num;
    //Collider2D col;
    // Start is called before the first frame update
    void Start()
    {
        //col = GetComponent<Collider2D>();
        //audio = GetComponent<AudioSource>();
        initPOS = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject QL = GameObject.Find("QuestionLoader");
        if (tag == "Selection")
        {
            if (Input.touchCount > 0 && QL.GetComponent<ShowCard>().readyQUEST)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

                if (touch.phase == TouchPhase.Began)
                {
                    //Collider2D touchCollider = Physics2D.OverlapPoint(touchPos);
                    float offX = Mathf.Abs(transform.position.x - touchPos.x);
                    float offY = Mathf.Abs(transform.position.y - touchPos.y);
                    if (offX <= 1f && offY <= 1f)
                    {
                        QL.GetComponent<ShowCard>().addEffect(3, new Vector3(touchPos.x, touchPos.y, 0));
                        moveAllowed = true;
                        QL.GetComponent<ShowCard>().ReadNumber(num);
                    //audio.Play();
                    }
                }
                if (touch.phase == TouchPhase.Moved)
                {
                    if (moveAllowed)
                    {
                        transform.position = new Vector3(touchPos.x, touchPos.y, 0);
                    }
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    moveAllowed = false;
                    float offX = Mathf.Abs(transform.position.x - newPOS.x);
                    float offY = Mathf.Abs(transform.position.y - newPOS.y);
                    if (offX <= 0.8f && offY <= 0.8f)
                    {
                        locked = true;
                    }
                    if (locked)
                    {
                        transform.position = newPOS;
                    }
                    else
                    {
                        transform.position = initPOS;
                    }
                    if (locked)
                    {
                        QL.GetComponent<ShowCard>().addEffectOnSLC();
                        if (correct)
                        {
                            StartCoroutine(QL.GetComponent<ShowCard>().Respond(true));
                        }
                        else
                        {
                            StartCoroutine(QL.GetComponent<ShowCard>().Respond(false));
                        }
                    } 
                }
                

            }
        }
    }
}
