using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    
    //private GameMode gameMode { get; set; }
    [SerializeField]
    private Text classicModeText;
    [SerializeField]
    private Text innovativeModeText;
    [SerializeField]
    private Text battleModeText;
    [SerializeField]
    private GameData gameData;
    [SerializeField]
    private GameObject pacman;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GAME OVER START!!!!");
        if (pacman == null) pacman = GameObject.FindGameObjectWithTag("Pacman");
        pacman.transform.position = new Vector2(pacman.transform.position.x, classicModeText.transform.position.y + 5);
        if (gameData == null) gameData = GameObject.Find("GameData").GetComponent<GameData>();
        SetupView();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    // setup position of select cursor
    void SetupView()
    {
        if (gameData.currentMode == GameData.Mode.ClassicMode)
        {
            pacman.transform.position = new Vector2(pacman.transform.position.x, classicModeText.transform.position.y + 5);
        }
        else if (gameData.currentMode == GameData.Mode.InnovativeMode)
        {
            pacman.transform.position = new Vector2(pacman.transform.position.x, innovativeModeText.transform.position.y + 5);
        }
        else
        {
            pacman.transform.position = new Vector2(pacman.transform.position.x, battleModeText.transform.position.y + 5);
        }
    }

    void CheckInput()
    {
        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            if (gameData.currentMode == GameData.Mode.ClassicMode)
            {
                gameData.currentMode = GameData.Mode.InnovativeMode;
                pacman.transform.position = new Vector2(pacman.transform.position.x, innovativeModeText.transform.position.y + 5);
            }
            else if (gameData.currentMode == GameData.Mode.InnovativeMode)
            {
                gameData.currentMode = GameData.Mode.BattleMode;
                pacman.transform.position = new Vector2(pacman.transform.position.x, battleModeText.transform.position.y + 5);
            }
            else
            {
                gameData.currentMode = GameData.Mode.ClassicMode;
                pacman.transform.position = new Vector2(pacman.transform.position.x, classicModeText.transform.position.y + 5);
            }
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
        {
            if (gameData.currentMode == GameData.Mode.ClassicMode)
            {
                gameData.currentMode = GameData.Mode.BattleMode;
                pacman.transform.position = new Vector2(pacman.transform.position.x, battleModeText.transform.position.y + 5);
            }
            else if (gameData.currentMode == GameData.Mode.InnovativeMode)
            {
                gameData.currentMode = GameData.Mode.ClassicMode;
                pacman.transform.position = new Vector2(pacman.transform.position.x, classicModeText.transform.position.y + 5);
            }
            else
            {
                gameData.currentMode = GameData.Mode.InnovativeMode;
                pacman.transform.position = new Vector2(pacman.transform.position.x, innovativeModeText.transform.position.y + 5);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Return))
        {
            switch (gameData.currentMode)
            {
                case GameData.Mode.ClassicMode:
                    SceneManager.LoadSceneAsync("Classic");
                    break;
                case GameData.Mode.InnovativeMode:
                    SceneManager.LoadSceneAsync("Classic");
                    break;
                case GameData.Mode.BattleMode:
                    SceneManager.LoadSceneAsync("Classic");
                    break;
            }
            gameData.isOver = false;
            gameData.ResetData();
        }
    }
}
