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
    private Text cursorText;
    [SerializeField]
    private GameMode gameMode;
    [SerializeField]
    private GameObject pacman;

    // Start is called before the first frame update
    void Start()
    {
        pacman.transform.position = new Vector2(pacman.transform.position.x, classicModeText.transform.position.y + 5);
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    void CheckInput()
    {
        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            if (gameMode.currentMode == GameMode.Mode.ClassicMode)
            {
                gameMode.currentMode = GameMode.Mode.InnovativeMode;
                pacman.transform.position = new Vector2(pacman.transform.position.x, innovativeModeText.transform.position.y + 5);
            }
            else if (gameMode.currentMode == GameMode.Mode.InnovativeMode)
            {
                gameMode.currentMode = GameMode.Mode.BattleMode;
                pacman.transform.position = new Vector2(pacman.transform.position.x, battleModeText.transform.position.y + 5);
            }
            else
            {
                gameMode.currentMode = GameMode.Mode.ClassicMode;
                pacman.transform.position = new Vector2(pacman.transform.position.x, classicModeText.transform.position.y + 5);
            }
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
        {
            if (gameMode.currentMode == GameMode.Mode.ClassicMode)
            {
                gameMode.currentMode = GameMode.Mode.BattleMode;
                pacman.transform.position = new Vector2(pacman.transform.position.x, battleModeText.transform.position.y + 5);
            }
            else if (gameMode.currentMode == GameMode.Mode.InnovativeMode)
            {
                gameMode.currentMode = GameMode.Mode.ClassicMode;
                pacman.transform.position = new Vector2(pacman.transform.position.x, classicModeText.transform.position.y + 5);
            }
            else
            {
                gameMode.currentMode = GameMode.Mode.InnovativeMode;
                pacman.transform.position = new Vector2(pacman.transform.position.x, innovativeModeText.transform.position.y + 5);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Return))
        {
            Debug.Log("ENTER!!!");
            switch (gameMode.currentMode)
            {
                case GameMode.Mode.ClassicMode:
                    SceneManager.LoadSceneAsync("Classic");
                    break;
                case GameMode.Mode.InnovativeMode:
                    SceneManager.LoadSceneAsync("Classic");
                    break;
                case GameMode.Mode.BattleMode:
                    SceneManager.LoadSceneAsync("Classic");
                    break;
            }
        }
    }
}
