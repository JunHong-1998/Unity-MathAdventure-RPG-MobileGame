using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShowCard : MonoBehaviour
{
    //[SerializeField] private GameObject canvas_parent;
    [SerializeField] private GameObject[] number;
    [SerializeField] private GameObject[] sign;
    [SerializeField] private AudioClip[] clip;
    [SerializeField] private AudioClip[] clip_answer;
    [SerializeField] private ParticleSystem[] particleSystem;
    [SerializeField] private GameObject EvalvBoard, ResultBoard;
    [SerializeField] private Button music;
    [SerializeField] private Sprite[] music_image;
    private GameObject card1, card2, card3, cardSO, cardSE, card4, card5, card6;
    private Vector3 ansCardx;
    private AudioSource audio;
    List<AudioClip> eqnREADER = new List<AudioClip>();
    private int answer = 0;       //random answer 
    [HideInInspector] public bool readyQUEST;
    [SerializeField] private Text showText1, showText2, showQuest, showTimer, showScore, showEvalv, ResultQuest, ResultAcc, ResultTime;
    [HideInInspector] private bool runTimer, runWatch;
    private int correctANSWER, quest_LEN, quest_answered, max;
    private float quest_TIMER;
    private void eqnGenerator()
    {
        int n1 = 0;
        int n2 = 0;
        
        int quest = Random.Range(6, 8);     // random quest type
        int blank = Random.Range(1, 4);     //random answer blank from n1 to n3
        if (PlayerPrefs.GetInt("game_Level") == -2 || PlayerPrefs.GetInt("game_Level") == 3) max = 100;
        else max = 11;
        int n3 = Random.Range(1, max);       //determine an answer 1st
        
        // add other maths
        if (quest == 6)
        {
            n1 = Random.Range(0, n3);
            n2 = n3 - n1;
        }
        else if(quest == 7){
            n1 = Random.Range(n3, max);
            n2 = n1-n3;
        }
              
     
        if (blank == 1)
        {
            cardArrange(card1, -6, true);
            answer = n1;
            eqnREADER.Add(clip[103]);    // what number sound
        }
        else
        {
            cardArrange(card1, -6, false, n1);
            eqnREADER.Add(clip[n1]);
        }
        // operator sound
        if (quest == 6)
        {
            cardSO = Instantiate(sign[1]) as GameObject;       // addition 
            eqnREADER.Add(clip[101]);
        }
        else
        {
            cardSO = Instantiate(sign[2]) as GameObject;       // subtraction
            eqnREADER.Add(clip[102]);
        }
        if (blank == 2)
        {
            cardArrange(card2, 0, true);
            answer = n2;
            eqnREADER.Add(clip[103]);    // what number sound
        }
        else
        {
            cardArrange(card2, 0, false, n2);
            eqnREADER.Add(clip[n2]);
        }
        // equals sound
        cardSE = Instantiate(sign[0]) as GameObject;    // equal sign
        eqnREADER.Add(clip[100]);
        if (blank==3)
        {
            cardArrange(card3, 6, true);
            answer = n3;
            eqnREADER.Add(clip[103]);    // what number sound
        }
        else
        {
            cardArrange(card3, 6, false, n3);
            eqnREADER.Add(clip[n3]);
        }


        cardSO.transform.position = new Vector3(-3, 2, -0.01f);
        cardSE.transform.position = new Vector3(3, 2.2f, -0.01f);
        print("type: " + blank + "\n" + n1 + " + " + n2 + " = " + n3);

        cardANS_selc();
        readyQUEST = true;
        //StartCoroutine(EqnReader());
    }

    private void cardANS_selc()
    {
        int[] ansLIST = new int[3];
        int tempGO;
        if (answer==0)
        {
            ansLIST = new int[] {answer, Random.Range(1,Mathf.RoundToInt(max/(float)2)), Random.Range(Mathf.RoundToInt(max / (float)2), max)};
        }
        else if(answer ==max-1)
        {
            ansLIST = new int[] { answer, Random.Range(0, Mathf.RoundToInt(max / (float)2)-1), Random.Range(Mathf.RoundToInt(max / (float)2)-1, max) };
        }
        else
        {
            ansLIST = new int[] { answer, Random.Range(0, answer), Random.Range(answer+1, max) };
        }
        for (int i = 0; i < ansLIST.Length - 1; i++)
        {
            int rnd = Random.Range(i, ansLIST.Length);
            tempGO = ansLIST[rnd];
            ansLIST[rnd] = ansLIST[i];
            ansLIST[i] = tempGO;
        }
        cardArrange(card4, -3.5f, false, ansLIST[0], -2.5f, true);
        cardArrange(card5, 0, false, ansLIST[1], -2.5f, true);
        cardArrange(card6, 3.5f, false, ansLIST[2], -2.5f, true);

    }
    private void cardArrange(GameObject card, float x, bool ansBlank, int num = -1, float y = 2, bool allow = false)
    {
        if (ansBlank)
        {
            ansCardx = new Vector3(x, y, -0.05f);
            card = Instantiate(sign[3]) as GameObject;
            card.transform.position = ansCardx;

        }
        else
        {
            if (PlayerPrefs.GetInt("game_Level") == -2 || PlayerPrefs.GetInt("game_Level") == 3)
            {
                card = Instantiate(number[11]) as GameObject;
                card.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = num.ToString();
                card.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = num.ToString();
                //card.GetComponentInChildren[0]<Text>().text = num.ToString();
                card.GetComponent<DragnDrop>().num = num;
            }
            else card = Instantiate(number[num]) as GameObject;
            if (!allow)
            {

                card.GetComponent<DragnDrop>().enabled = false;
            }
            card.transform.position = new Vector3(x, y, -0.01f);
            if (allow)
            {
                card.GetComponent<DragnDrop>().newPOS = ansCardx;
                card.tag = "Selection";
            }
            if (num == answer)
            {
                card.GetComponent<DragnDrop>().correct = true;
            }
        }
    }

    private void Awake()
    {
        if (PlayerPrefs.GetInt("Music") == 1) music.GetComponent<Image>().sprite = music_image[0];
        else music.GetComponent<Image>().sprite = music_image[1];
    }
        // Start is called before the first frame update
        void Start()
    {
        //Screen.SetResolution(1920, 1080, true);
        //Camera.main.orthographic = true;
        audio = GetComponent<AudioSource>();
        showTimer.color = Color.white;
        if (PlayerPrefs.GetString("navigate") == "MathPractice"){
            quest_LEN = 1;
            showQuest.text = "Quest " + quest_LEN;
            runWatch = true;
            quest_TIMER = 0;
            EvalvBoard.SetActive(true);
            showEvalv.enabled = true;
            showEvalv.text = "Accuracy\n-";
            showEvalv.color = Color.green;
        }
        else
        {
            quest_LEN = PlayerPrefs.GetInt("quest_LEN");
            quest_TIMER = PlayerPrefs.GetInt("quest_TIME");
            showQuest.text = "Quest " + quest_answered + "/" + quest_LEN;
            if (quest_TIMER == -1) showTimer.text = "-- : --";
            else runTimer = true;
        }
        


        quest_answered = 1;
        correctANSWER = 0;
        
        showScore.text = "Score " + correctANSWER + "/" + (quest_answered-1);
        
        
        eqnGenerator();
    }

    private void Update()
    {
        if (runTimer)
        {
            if (quest_TIMER > 0)
            {
                quest_TIMER -= Time.unscaledDeltaTime;
                float minutes = Mathf.FloorToInt(quest_TIMER / 60);
                float seconds = Mathf.FloorToInt(quest_TIMER % 60);
                showTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            else
            {
                showTimer.text = "00:00";
                runTimer = false;
                readyQUEST = false;
                quest_answered = quest_LEN;
                StartCoroutine(Respond(false));
                //force to stop
            }
            if (quest_TIMER <= 5) showTimer.color = Color.red;
            else if (quest_TIMER <= 10) showTimer.color = Color.magenta;
            
        }
        if (runWatch)
        {
            quest_TIMER += Time.unscaledDeltaTime;
            float minutes = Mathf.FloorToInt(quest_TIMER / 60);
            float seconds = Mathf.FloorToInt(quest_TIMER % 60);
            if (quest_TIMER < (99 * 60)) showTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        
    }

    public void ReadEquation()
    {
        if (!readyQUEST || AudioListener.pause) return;
        StartCoroutine(EqnReader());
    }

    private IEnumerator EqnReader()
    {
        readyQUEST = false;
        GameObject Reader = GameObject.Find("Read_btn");
        Reader.GetComponent<Button>().enabled = false;
        foreach (AudioClip read in eqnREADER)
        {
            audio.clip = read;
            audio.Play();
            yield return new WaitForSecondsRealtime(audio.clip.length);
        }
        readyQUEST = true;
        Reader.GetComponent<Button>().enabled = true;
    }

    public IEnumerator Respond(bool ans)
    {
        readyQUEST = false;
        yield return new WaitForSecondsRealtime(0.5f);
        string text_show="";
        int show;
        if (ans)
        {
            correctANSWER += 1;
            show = Random.Range(0, 6);
     
            if (show == 0) text_show = "Awesome";
            else if (show == 1) text_show = "Fabulous";
            else if (show == 2) text_show = "Fantastic";
            else if (show == 3) text_show = "Impressive";
            else if (show == 4) text_show = "Marvelous";
            else if (show == 5) text_show = "Unbelievable";
            
        }
        else
        {
            show = 6;
            text_show = "Answer : "+answer;
        }
        addEffect(1);
        showText1.text = text_show;
        showText2.text = text_show;
        showScore.text = "Score " + correctANSWER + "/" + quest_answered;
        audio.clip = clip_answer[show];
        if (!AudioListener.pause) audio.Play();
        if (ans) addEffect(2);
        yield return new WaitForSecondsRealtime(audio.clip.length + 1.25f);
        quest_answered += 1;
        clean();
        if (PlayerPrefs.GetString("navigate") == "MathGame")
        {
            if (quest_LEN + 1 != quest_answered)
            {
                eqnGenerator();
                showQuest.text = "Quest " + quest_answered + "/" + quest_LEN;
            }
            else
            {
                //int pass = 0;
                //if (correctANSWER == PlayerPrefs.GetInt("quest_LEN")) pass = 1;
                PlayerPrefs.SetInt("Score", correctANSWER);
                PlayerPrefs.SetString("navigate", "Backgame");
            }
        }
        else
        {
            float evalv = correctANSWER / (float)(quest_answered-1) * 100f;
            eqnGenerator();
            if (evalv < 10) showEvalv.color = Color.red;
            else if (evalv < 30) showEvalv.color = Color.yellow;
            else showEvalv.color = Color.green;
            showQuest.text = "Quest " + quest_answered;
            showEvalv.text = "Accuracy\n" + Mathf.RoundToInt(evalv) + "%";
        }
    }

    private void clean()
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Respawn");
        foreach (GameObject card in cards)
        {
            GameObject.Destroy(card); 
        }
        GameObject[] cards2 = GameObject.FindGameObjectsWithTag("Selection");
        foreach (GameObject card in cards2)
        {
            GameObject.Destroy(card);
        }
        eqnREADER.Clear();
        showText1.text = "try out";
        showText2.text = "try out";
        
    }

    public void addEffectOnSLC()
    {
        addEffect(0, ansCardx);
    }
    public void addEffect(int effect, Vector3 newPos=default)
    {
        if (newPos != default)
        {
            particleSystem[effect].transform.position = newPos;
        }
        particleSystem[effect].Play();
    }

    public void ReadNumber(int num)
    {
        if (AudioListener.pause) return;
        audio.clip = clip[num];
        audio.Stop();
        audio.Play();
    }

    public void buttonOption(int opt)
    {
        if (opt == -1)
        {
            clean();
            if (PlayerPrefs.GetString("navigate") == "MathPractice")
            {
                runWatch = false;
                float evalv = correctANSWER / (float)(quest_answered - 1) * 100f;
                ResultQuest.text = (quest_answered - 1).ToString();
                ResultAcc.text = Mathf.RoundToInt(evalv) + "%";
                float minutes = Mathf.FloorToInt(quest_TIMER / 60);
                float seconds = Mathf.FloorToInt(quest_TIMER % 60);
                ResultTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                ResultBoard.SetActive(true);
                PlayerPrefs.SetString("navigate", "home");
            }
            else
            {
                PlayerPrefs.SetString("navigate", "home2");
                PlayerPrefs.SetInt("Score", correctANSWER);
            }
        }
        else if (opt == 1)
        {
            if (PlayerPrefs.GetInt("Music") == 1)
            {
                audio.Stop();
                AudioListener.pause = true;
                PlayerPrefs.SetInt("Music", -1);
                music.GetComponent<Image>().sprite = music_image[1];
            }
            else
            {
                AudioListener.pause = false;
                PlayerPrefs.SetInt("Music", 1);
                music.GetComponent<Image>().sprite = music_image[0];
            }
        }
    }
}
