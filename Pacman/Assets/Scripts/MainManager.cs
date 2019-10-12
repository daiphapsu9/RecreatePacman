using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    private int score;
    private int[] multiplier = { 1, 2, 4, 8 };
    private int currentMultiplierPos = 0;
    private int ghostScore = 200;
    private int smallBallScore = 10;
    private int bigBallScore = 10;
    [SerializeField]
    private UIManager uiManager;

    [SerializeField]
    private GameObject[] spawningPoints;

    private float SpwanItemFrequency = 10f;
    [SerializeField]
    private GameObject cherry;
    [SerializeField]
    private GameObject strawberry;
    [SerializeField]
    private GameObject orange;
    [SerializeField]
    private GameObject apple;
    [SerializeField]
    private GameObject pear;
    [SerializeField]
    private GameObject banana;
    [SerializeField]
    private AudioSource levelStartAudioSource;
    [SerializeField]
    private AudioSource backgroundAudioSource;

    DateTime pauseStartingTime;
    private float pauseDuration;
    private bool firstStart = true;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        Pause(2f);
        //ShowReadyCanvas();
        StartCoroutine(SpawnTimer());
    }

    // Update is called once per frame
    void Update()
    {
        uiManager.UpdateScore(score);
        CheckPauseGame();
    }

    void CheckPauseGame()
    {
        if (Time.timeScale == 0) // is pausing, need to unpause
        {
            var timePassSincePause = DateTime.Now - pauseStartingTime;
            if (timePassSincePause.Seconds > pauseDuration)
            {
                Unpause();
                if (firstStart)
                {
                    StartReady();
                }
            }
        }
    }

    void StartReady()
    {
        if (!levelStartAudioSource.isPlaying)
        {
            levelStartAudioSource.Play();
        }
        //Debug.Log("levelStartAudioSource.clip.length == " + levelStartAudioSource.clip.length);
        Invoke("PlayBackgroundMusic", levelStartAudioSource.clip.length - pauseDuration - 1);
        uiManager.readyCanvas.SetActive(false);
        firstStart = false;
    }

    void PlayBackgroundMusic()
    {
        if (!backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Play();
        }
    }

    public void ConsumeGhost(){

        score += ghostScore * multiplier[currentMultiplierPos];
        if (currentMultiplierPos < multiplier.Length - 1)
        {
            currentMultiplierPos++;
        }
    }

    public void ConsumeSmallBall()
    {
        score += 10;
    }

    public void ConsumeBigBall()
    {
        score += 50;
    }

    public void ConsumeFruit(Fruit fruit)
    {
        score += fruit.GetPoints();
    }

    public void ResetMultiplier()
    {
        if (currentMultiplierPos != 0) currentMultiplierPos = 0;
    }

    public IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(10);
        while (true)
        {
            SpawnFruits();
            yield return new WaitForSeconds(SpwanItemFrequency);
        }
    }

    public void SpawnFruits()
    {
        
        if (spawningPoints == null || spawningPoints.Length <= 0) { return; }

        int randomZoneIndex = UnityEngine.Random.Range(0, spawningPoints.Length - 1);
        GameObject point = spawningPoints[randomZoneIndex];

        GameObject toInstantiate = ObjectToSpawn().gameObject;
        Instantiate(toInstantiate, new Vector3(point.transform.position.x, point.transform.position.y, 0), Quaternion.identity);
    }

    //Cherry = 100,
    //    Strawberry = 300,
    //    Orange = 500,
    //    Apple = 700,
    //    Pear = 1000,
    //    Banana = 2000,
    private GameObject ObjectToSpawn()
    {
        int randomNumber = UnityEngine.Random.Range(0, 100);
        
        if (randomNumber < 5) // 5% spawn banana
        {
            return banana;
        }
        else if (randomNumber < 15) // 10% spawn pear
        {
            return pear;
        }
        else if (randomNumber < 30) // 15% spawn apple
        {
            return apple;
        }
        else if (randomNumber < 50) // 20% orange
        {
            return orange;
        }
        else if (randomNumber < 75) // 25% spawn strawberry
        {
            return strawberry;
        }
        else //25% spawn cherry
        {
            return cherry;
        }
    }

    // Gameplay manage
    public void Pause(float duration)
    {
        pauseStartingTime = DateTime.Now;
        Time.timeScale = 0;
        pauseDuration = duration;
    }

    public void Unpause()
    {
        Time.timeScale = 1;

    }
}
