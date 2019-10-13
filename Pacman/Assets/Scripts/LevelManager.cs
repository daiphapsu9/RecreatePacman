using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Each playing scene has a level manager that keeps track of all relevant objects and sound 
public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private UIManager uiManager = null;

    [SerializeField]
    private GameObject[] spawningPoints = null;

    private float SpwanItemFrequency = 7f;
    [SerializeField]
    private GameObject cherry = null;
    [SerializeField]
    private GameObject strawberry = null;
    [SerializeField]
    private GameObject orange = null;
    [SerializeField]
    private GameObject apple = null;
    [SerializeField]
    private GameObject pear = null;
    [SerializeField]
    private GameObject banana = null;
    [SerializeField]
    private AudioSource levelStartAudioSource = null;
    [SerializeField]
    private AudioSource backgroundAudioSource = null;

    DateTime pauseStartingTime;
    private float pauseDuration;
    private bool firstStart = true;

    public GameData gameData;

    // Start is called before the first frame update
    void Start()
    {
        // show Ready canvas for playing scene that will pause the game in first 2 seconds
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
        gameData.GetRequiredReference();
        if (SceneManager.GetActiveScene().name != "GameMenu" && SceneManager.GetActiveScene().name != "GameOver")
        {
            Pause(2f);
            // spawn fruits each 7 seconds, position at a random waypoint
            StartCoroutine(SpawnTimer());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "GameMenu" && SceneManager.GetActiveScene().name != "GameOver")
        {
            // Update score label
            uiManager.UpdateScore(gameData.score, gameData.currentMode, gameData.player2Score);
            // if game is paused, unpaused game after given duration
            CheckPauseGame();
        }
        // Check if end game condition is met to load Game Over scene
        if (gameData.isOver)
        {
            foreach (Pacman pacman in gameData.allPacmans)
            {
                // Load game over scene only after pacman dead sound is playing (if pacman is dead)
                if (pacman.deathSound.isPlaying == false)
                {
                    SceneManager.LoadScene("GameOver");
                }
            }
        }
    }

    void CheckPauseGame()
    {
        if (Time.timeScale == 0) // is pausing, need to unpause
        {
            var timePassSincePause = DateTime.Now - pauseStartingTime;
            if (timePassSincePause.Seconds > pauseDuration)
            {
                Unpause();
                // if the game is pause at the first start, play level start audio
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
        // play background music after level start audio has finish playing
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

    public IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(SpwanItemFrequency);
        while (true)
        {
            SpawnFruits();
            yield return new WaitForSeconds(SpwanItemFrequency);
        }
    }

    // spawn random fruit every 7s at different rate 
    public void SpawnFruits()
    {
        if (spawningPoints == null || spawningPoints.Length <= 0) { return; }
        
        // get random waypoint to spawn the fruit
        int randomZoneIndex = UnityEngine.Random.Range(0, spawningPoints.Length - 1);
        GameObject point = spawningPoints[randomZoneIndex];

        GameObject toInstantiate = ObjectToSpawn().gameObject;
        Instantiate(toInstantiate, new Vector3(point.transform.position.x, point.transform.position.y, 0), Quaternion.identity);
    }

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
