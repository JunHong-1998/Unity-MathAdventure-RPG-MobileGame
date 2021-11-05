using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.UI;

public class Game_Manager : MonoBehaviour
{
    [SerializeField] private Camera camera_main;
    [SerializeField] private CinemachineFreeLook camera_free;
    [SerializeField] private Player_control player_ctrl;
    [SerializeField] private GameObject ActualPlayer, GameInterface, Skeleton_prefab, GolemTeam, thePlayer, theLoadingBar, Play_button,
        SubMenu_btn, Difficulty_P_btn, Difficulty_Adv_btn, back_btn, LoseBoard, WinBoard, GiveUpBoard, superBackBtn, inst_SHOW;
    [SerializeField] private Light PlatformLight, DirectLight;
    //[SerializeField] private AudioListener listener;
    [SerializeField] private Transform platform;
    [SerializeField] private Text timeTaken, finalResult;
    private Vector3[] Skeleton_POS;
    [SerializeField] private GameObject[] CrystalPoint, Diamond_basePOS, Diamond_POS, Eye_Fire;
    [HideInInspector] public bool isPaused;
    [HideInInspector] public ParticleSystem[] ps;
    [HideInInspector] public GameObject currentCheckPoint, currentDiamond;
    [HideInInspector] public int checkPOINT, Skeleton_num;
    [SerializeField] private float rot_deg = 0;
    [SerializeField] private AudioClip[] clips;
    [HideInInspector] public float FinalScore;
    private AudioSource audio;
    private float runTime;
    private bool gameEnd, loadINcheckPOINT;
    private Vector3[] preset_checkPoint = new Vector3[5];
    private Vector3[] preset_diamond_basePoint = new Vector3[5];
    private Vector3[] preset_diamondPoint = new Vector3[5];
    //private Transform initTrans;
    // Start is called before the first frame update

    void Start()
    {
       
        audio = GetComponent<AudioSource>();
        cleanTheScene();
        PlayerPrefs.SetInt("Music", 1);
        PlayerPrefs.SetString("navigate", "AnimeEffect");
        StartCoroutine(AnimationEffect());

        /*camera_free.m_YAxis.Value = 0.45f;
        thePlayer.SetActive(true);
        Play_button.SetActive(true);
        PlayerPrefs.SetString("navigate", "MainMenu");*/

    }

    // Update is called once per frame
    void Update()
    {
        if(!gameEnd) runTime += Time.unscaledDeltaTime;
        if (PlayerPrefs.GetString("navigate") == "Backgame") BackToGame();
        else if (PlayerPrefs.GetString("navigate") == "home" || PlayerPrefs.GetString("navigate") == "home2") BackToHomeRequest();


        if (player_ctrl.gameStart && !gameEnd && player_ctrl.currentHealth == 0) StartCoroutine(PlayerLose());
        else if (!gameEnd && checkPOINT == 6) StartCoroutine(PlayerWin());
        //if (press) ContinueAfterPopUp();
        /*if (PlayerPrefs.GetInt("Music") == -1)
        {
            AudioListener.pause = true;
            PlayerPrefs.SetInt("Music", 0);
        }
        else if (PlayerPrefs.GetInt("Music") == 1)
        {
            AudioListener.pause = false;
            PlayerPrefs.SetInt("Music", 0);
        }*/
    }

    public void MusicOnOff()
    {
        if (PlayerPrefs.GetInt("Music")== 1)
        {
            AudioListener.pause = true;
            PlayerPrefs.SetInt("Music", -1);
        }
        else
        {
            AudioListener.pause = false;
            PlayerPrefs.SetInt("Music", 1);
        }
    }
    private IEnumerator AnimationEffect()
    {
        camera_free.m_YAxis.Value = 0.8f;
        yield return new WaitForSeconds(0.3f);
        audio.clip = clips[4];
        audio.Play();
        PlatformLight.enabled = true;

        yield return new WaitForSeconds(2.5f);
        audio.clip = clips[0];
        audio.Play();
        Eye_Fire[0].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        audio.Play();
        Eye_Fire[1].SetActive(true);
        yield return new WaitForSeconds(2f);
        audio.clip = clips[1];
        audio.Play();
        
        while (PlatformLight.range < 7)
        {
            PlatformLight.range += 0.5f;
            camera_free.m_YAxis.Value -= 0.01f;
            yield return new WaitForSeconds(0.1f);  //1f
        }
        theLoadingBar.SetActive(true);
        float speedy = 0.15f;
        while (camera_free.m_YAxis.Value > 0.45f)
        {
            camera_free.m_YAxis.Value -= 0.01f;
            player_ctrl.currentHealth += 100;
            player_ctrl.healthBAR.setHealth(player_ctrl.currentHealth);
            //if (player_ctrl.currentHealth > 2000 && DirectLight.intensity < 1.25f) DirectLight.intensity += 0.25f;
            yield return new WaitForSeconds(speedy);  //1f
            if (speedy > 0.001f) speedy -= 0.01f;
            if (player_ctrl.currentHealth == 500)
            {
                player_ctrl.particleSystem[5].Play();
            }
            if (player_ctrl.currentHealth == 1500) thePlayer.SetActive(true);
            if (player_ctrl.currentHealth == 2000) player_ctrl.anim.SetTrigger("attack3");
        }
        camera_free.m_YAxis.Value = 0.45f;
        player_ctrl.audio.clip = clips[2];
        player_ctrl.audio.Play();
        while (DirectLight.intensity < 1f)
        {
            player_ctrl.currentHealth += 150;
            player_ctrl.healthBAR.setHealth(player_ctrl.currentHealth);
            DirectLight.intensity += 0.25f;
            yield return new WaitForSeconds(speedy);  //1f
            if (speedy > 0.001f) speedy -= 0.01f;
        }
        yield return new WaitForSeconds(1.5f);
        Play_button.SetActive(true);
        player_ctrl.gameStart = true; // end for healthbar ovveride
        PlayerPrefs.SetString("navigate", "MainMenu");
        

    }
    private void ReadyForGameStart()
    {
        gameEnd = isPaused = false;
        GameInterface.SetActive(true);
        GolemTeam.SetActive(true);
        player_ctrl.ResetPlayer();
        thePlayer.SetActive(true);
        theLoadingBar.SetActive(true);
        ActualPlayer.transform.rotation = Quaternion.Euler(0, 0, 0);
        camera_free.m_XAxis.Value = rot_deg = 0;
        gameLevel(); // supposed set by menu !!!!!!!!!!!!!!!!!!!!!!!!!!!!! game level

        Skeleton_POS = new Vector3[] { new Vector3(190f, 0.5f, 247f), new Vector3(140f, 0.5f, 216f), new Vector3(195.5f, 0.5f, 185.9f), new Vector3(186.5f, 0.5f, 119f), new Vector3(242.5f, 0.5f, 84f),
           new Vector3(335f, 0.5f, 115f), new Vector3(317f, 0.5f, 157f), new Vector3(345.5f, 0.5f, 187f), new Vector3(345.5f, 0.5f, 242f), new Vector3(284f, 0.5f, 275f), new Vector3(246f, 0.5f, 223f),
           new Vector3(218.5f, 0.5f, 151f), new Vector3(282.5f, 0.5f, 154f)};
        SpawnSkeleton();
        PlayerPrefs.SetString("navigate", "game");
        checkPOINT = 1;
        RearrangeCheckPoint();
        runTime = FinalScore = 0;
        player_ctrl.gameStart = true;

    }
    public IEnumerator ReadyToChangeScene()
    {
        if (PlayerPrefs.GetString("navigate") == "MathGame")
        {
            player_ctrl.moveAllow = false;
            player_ctrl.anim.SetBool("walk", false);

            //gameLevel(PlayerPrefs.GetInt("game_Level"));
            Time.timeScale = 0;
            if (!AudioListener.pause) audio.PlayOneShot(clips[3]);
            while (currentCheckPoint.GetComponentInChildren<Light>().intensity < 10)
            {
                currentCheckPoint.GetComponentInChildren<Light>().intensity += 1f;
                yield return new WaitForSecondsRealtime(0.25f);
            }
            GameInterface.SetActive(false);
        }
        
        SceneManager.LoadScene("MathGame", LoadSceneMode.Additive);
        yield return new WaitForSecondsRealtime(0.1f);
        //listener.enabled = false;
        camera_free.enabled = false;
        camera_main.enabled = false;
    }

    private void cleanTheScene()
    {
        GameInterface.SetActive(false);
        GolemTeam.SetActive(false);
        thePlayer.SetActive(false);
        GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enem in enemy) GameObject.Destroy(enem);
    }

    public void ResumeGame()
    {
        if (gameEnd) ContinueAfterPopUp();
        else if (PlayerPrefs.GetString("navigate") == "home")
        {
            superBackBtn.SetActive(false);
            BackToHome();
        }
        else if (PlayerPrefs.GetString("navigate") == "home2")
        {
            superBackBtn.SetActive(false);
            GiveUpBoard.SetActive(false);
            BackToGame();
        }
        else
        {
            superBackBtn.SetActive(false);
            GiveUpBoard.SetActive(false);
            Time.timeScale = 1;
            GameInterface.SetActive(true);
        }
        
    }
    public void ContinueAfterPopUp()
    {
        LoseBoard.SetActive(false);
        WinBoard.SetActive(false);
        GiveUpBoard.SetActive(false);
        superBackBtn.SetActive(false);
        player_ctrl.anim.SetTrigger("reborn");
        print("out");
        BackToHome();
        player_ctrl.moveAllow = true;
        if (Time.timeScale == 0) Time.timeScale = 1;
    }
    private IEnumerator PlayerLose()
    {
        gameEnd = true;
        player_ctrl.anim.SetTrigger("dead");
        yield return new WaitForSecondsRealtime(2.15f);
        LoseBoard.SetActive(true);
        if (!AudioListener.pause) audio.PlayOneShot(clips[6]);
        yield return new WaitForSecondsRealtime(1f);
        superBackBtn.SetActive(true);
        GameInterface.SetActive(false);
    }
    private IEnumerator PlayerWin()
    {
        gameEnd = true;
        player_ctrl.moveAllow = false;
        GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enem in enemy) GameObject.Destroy(enem);
        float minutes = Mathf.FloorToInt(runTime / 60);
        float seconds = Mathf.FloorToInt(runTime % 60);
        timeTaken.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        FinalScore += (100000 / runTime);
        finalResult.text = Mathf.RoundToInt(FinalScore).ToString();
        player_ctrl.anim.SetTrigger("attack3");

        player_ctrl.audio.PlayOneShot(clips[2]);

        yield return new WaitForSecondsRealtime(1.25f);
        WinBoard.SetActive(true);
        if (!AudioListener.pause) audio.PlayOneShot(clips[5]);
        GameInterface.SetActive(false);
        while (isPaused) yield return new WaitForSecondsRealtime(1f);
        superBackBtn.SetActive(true);
    }

    public void BackToHomeRequest()
    {
        if (player_ctrl.joystick.Horizontal != 0 || player_ctrl.joystick.Vertical != 0) return;
        superBackBtn.SetActive(true);
        if (PlayerPrefs.GetString("navigate") == "home") return;
        GiveUpBoard.SetActive(true);
        Time.timeScale = 0;
        GameInterface.SetActive(false);
        
    }
    private void BackToHome()
    {
        
        //isPaused = true;
        PlayerPrefs.SetString("navigate", "MainMenu");
        cleanTheScene();
        Play_button.SetActive(true);
        thePlayer.SetActive(true);
        theLoadingBar.SetActive(false);
        player_ctrl.controller.enabled = player_ctrl.gameStart = false;
        ActualPlayer.transform.position = platform.position;
        print("change jor position" + ActualPlayer.transform.position);
        ActualPlayer.transform.rotation = Quaternion.Euler(0, 42.34f, 0);
        
        if (SceneManager.sceneCount == 2) SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("MathGame").buildIndex);
        camera_free.m_XAxis.Value = 216.24f;
        camera_free.enabled = true;
        camera_main.enabled = true;
        player_ctrl.controller.enabled = true;

    }

    private void BackToGame()
    {
        PlayerPrefs.SetString("navigate", "game");
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("MathGame").buildIndex);
        GameInterface.SetActive(true);
        camera_free.enabled = true;
        camera_main.enabled = true;
        //listener.enabled = true;
        //StartCoroutine(TransformPlayerBack());
        if (checkTest(PlayerPrefs.GetInt("game_Level"), PlayerPrefs.GetInt("Score"), PlayerPrefs.GetInt("quest_LEN")))
        {
            StartCoroutine(PassTest());
        }
        else
        {
            StartCoroutine(FailTest());
        }
        

        print("Test mark: "+PlayerPrefs.GetInt("Score"));
    }

    private IEnumerator FailTest()
    {
        while (currentCheckPoint.GetComponentInChildren<Light>().intensity > 2)
        {
            ps[0].startSize -= 1f;
            currentCheckPoint.GetComponentInChildren<Light>().intensity -= 1f;
            yield return new WaitForSecondsRealtime(0.25f);
        }
        ps[0].Stop();
        ps[0].startSize = 12;
        player_ctrl.transform.position = platform.position;
        Time.timeScale = 1;
        player_ctrl.moveAllow = true;
        isPaused = false;
    }
    private IEnumerator PassTest()
    {
        while (currentCheckPoint.GetComponentInChildren<Light>().intensity > 0)
        {
            ps[0].startSize -= 1f;
            currentCheckPoint.GetComponentInChildren<Light>().intensity -= 1f;
            yield return new WaitForSecondsRealtime(0.25f);
            if (currentCheckPoint.GetComponentInChildren<Light>().intensity == 3f) ps[1].Play();
        }
        ps[0].Stop();
        ps[0].startSize = 12;
        currentCheckPoint.SetActive(false);
        currentCheckPoint.GetComponentInChildren<Light>().intensity = 2;
        //GameObject.Destroy(currentCheckPoint;        //destroy if accomplish
        Time.timeScale = 1;
        player_ctrl.moveAllow = true;
        isPaused = false;
        currentDiamond.SetActive(true);
        checkPOINT += 1;
    }

    private void gameLevel()
    {
        if (PlayerPrefs.GetInt("game_Level")==1)
        {
            PlayerPrefs.SetInt("quest_LEN", 3);     // easy mode 3 quest
            PlayerPrefs.SetInt("quest_TIME", -1);     // easy mode no time(secs) limit -1
            Skeleton_num = 1;
        }
        else if (PlayerPrefs.GetInt("game_Level")==2)
        {
            PlayerPrefs.SetInt("quest_LEN", 5);     // moderate mode 5 quest
            PlayerPrefs.SetInt("quest_TIME", 300);    // 1 min per quest
            Skeleton_num = 2;
        }
        else if (PlayerPrefs.GetInt("game_Level") == 3)
        {
            PlayerPrefs.SetInt("quest_LEN", 10);     // hard mode 10 quest
            PlayerPrefs.SetInt("quest_TIME", 450);    // .45 min per quest
            Skeleton_num = 4;
        }
    }

    private bool checkTest(int lvl, int score, int quest_LEN)
    {
        bool pass = false;
        if (lvl == 1)
        {
            if (score >= 1) pass = true;
        }
        else if (lvl == 2)
        {
            if (score >= 3) pass = true;
        }
        else
        {
            if (score >= 7) pass = true;
        }
        if (pass) FinalScore += (score / quest_LEN * 100);
        else FinalScore -= 100;
        return pass;
    }

    private void RearrangeCheckPoint()
    {
        if (!loadINcheckPOINT)
        {
            loadINcheckPOINT = true;
            for (int c = 0; c < 5; c++)
            {
                preset_checkPoint[c] = CrystalPoint[c].transform.position;
                preset_diamondPoint[c] = Diamond_POS[c].GetComponent<RectTransform>().position;
                preset_diamond_basePoint[c] = Diamond_basePOS[c].GetComponent<RectTransform>().position;
            }
        }
        List<int> seq = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            int a = Random.Range(1, 6);

            if (i > 0)
            {
                while (seq.Contains(a))
                {
                    a = Random.Range(1, 6);
                }
            }
            seq.Add(a);
        }
        int[] pointSEQ = seq.ToArray();

        int n = 0;
        foreach (int p in pointSEQ)
        {
            CrystalPoint[n].transform.position = preset_checkPoint[p-1];
            CrystalPoint[n].SetActive(true);
            n += 1;
           
        }
        seq.Clear();
        for (int i = 0; i < 5; i++)
        {
            int a = Random.Range(1, 6);

            if (i > 0)
            {
                while (seq.Contains(a))
                {
                    a = Random.Range(1, 6);
                }
            }
            seq.Add(a);
        }
        pointSEQ = seq.ToArray();
        n = 0;
        foreach (int p in pointSEQ)
        {
            CrystalPoint[n].GetComponent<CheckPoint>().StationNumber = p;
            Diamond_basePOS[n].GetComponent<RectTransform>().position = preset_diamond_basePoint[p - 1];
            Diamond_POS[n].GetComponent<RectTransform>().position = preset_diamondPoint[p - 1];
            Diamond_POS[n].SetActive(false);
            n += 1;

        }
    }

    public void PressRegion(bool LR)
    {
        if (isPaused) return;
        if (LR) //right
        {
            rot_deg += 1;
        }
        else
        {
            rot_deg -= 1;
        }
        camera_free.m_XAxis.Value = rot_deg;
    }

    private void SpawnSkeleton()
    {
        foreach(Vector3 dest in Skeleton_POS)
        {
            float rand = Random.value;
            for (int i=0; i<Skeleton_num; i++)
            {
                float angle = i * 360/Skeleton_num*rand;
                Vector3 newPos;
                newPos.x = dest.x + 18 * Mathf.Cos(angle * Mathf.Deg2Rad);
                newPos.z = dest.z + 18 * Mathf.Sin(angle * Mathf.Deg2Rad);
                newPos.y = dest.y;
                Quaternion newRot = Quaternion.FromToRotation(Vector3.forward, dest - newPos);
                Instantiate(Skeleton_prefab, newPos, newRot);
            }
        }
    }

    public void NavigateTo(int where)
    {
        if (where == -1) // Back Button
        {
            if (PlayerPrefs.GetString("navigate") == "SubMenu")
            {
                back_btn.SetActive(false);
                SubMenu_btn.SetActive(false);
                Play_button.SetActive(true);
                thePlayer.SetActive(true);
                theLoadingBar.SetActive(true);
                PlayerPrefs.SetString("navigate", "MainMenu");
            }
            else if (PlayerPrefs.GetString("navigate") == "Practice")
            {
                Difficulty_P_btn.SetActive(false);
                SubMenu_btn.SetActive(true);
                PlayerPrefs.SetString("navigate", "SubMenu");
            }
            else if (PlayerPrefs.GetString("navigate") == "Adventure")
            {
                Difficulty_Adv_btn.SetActive(false);
                SubMenu_btn.SetActive(true);
                PlayerPrefs.SetString("navigate", "SubMenu");
            }


        }
        else if (where == -2) inst_SHOW.SetActive(false);
        else if (where == 0) inst_SHOW.SetActive(true);

        else if (where == 1) //pass by play page >> Practice / ADV (SubMenu)
        {
            Play_button.SetActive(false);
            thePlayer.SetActive(false);
            theLoadingBar.SetActive(false);
            PlayerPrefs.SetString("navigate", "SubMenu");
            SubMenu_btn.SetActive(true);
            back_btn.SetActive(true);
        }
        else if (where == 2)    // choose practice
        {
            SubMenu_btn.SetActive(false);
            PlayerPrefs.SetString("navigate", "Practice");
            Difficulty_P_btn.SetActive(true);
        }
        else if (where == 3)    // choose adventure
        {
            SubMenu_btn.SetActive(false);
            PlayerPrefs.SetString("navigate", "Adventure");
            Difficulty_Adv_btn.SetActive(true);
        }
        else if (where == 4)        // choose practice easy
        {
            Difficulty_P_btn.SetActive(false);
            back_btn.SetActive(false);
            PlayerPrefs.SetString("navigate", "MathPractice");
            PlayerPrefs.SetInt("game_Level", -1);
            StartCoroutine(ReadyToChangeScene());
        }
        else if (where == 5)        // choose practice hard
        {
            //hard
            Difficulty_P_btn.SetActive(false);
            back_btn.SetActive(false);
            PlayerPrefs.SetString("navigate", "MathPractice");
            PlayerPrefs.SetInt("game_Level", -2);
            StartCoroutine(ReadyToChangeScene());
        }
        else if (where == 6)        // choose adventure easy
        {
            Difficulty_Adv_btn.SetActive(false);
            back_btn.SetActive(false);
            PlayerPrefs.SetString("navigate", "MathGame");
            PlayerPrefs.SetInt("game_Level", 1);
            ReadyForGameStart();
        }
        else if (where == 7)        // choose adventure moderate
        {
            Difficulty_Adv_btn.SetActive(false);
            back_btn.SetActive(false);
            PlayerPrefs.SetString("navigate", "MathGame");
            PlayerPrefs.SetInt("game_Level", 2);
            ReadyForGameStart();
        }
        else if (where == 8)        // choose adventure hard
        {
            Difficulty_Adv_btn.SetActive(false);
            back_btn.SetActive(false);
            PlayerPrefs.SetString("navigate", "MathGame");
            PlayerPrefs.SetInt("game_Level", 3);
            ReadyForGameStart();
        }
    }
}
