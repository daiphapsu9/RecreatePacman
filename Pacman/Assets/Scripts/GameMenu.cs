using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Class support Menu scene UI, recording selecting cursor position
public class GameMenu : MonoBehaviour
{
    
    //private GameMode gameMode { get; set; }
    [SerializeField]
    private Text classicModeText = null;
    [SerializeField]
    private Text innovativeModeText = null;
    [SerializeField]
    private Text battleModeText = null;
    [SerializeField]
    private GameData gameData = null;
    [SerializeField]
    private GameObject pacman = null;

    // Start is called before the first frame update
    void Start()
    {
        // Pacman is used as a selecting cursor
        if (pacman == null) pacman = GameObject.FindGameObjectWithTag("Pacman");
        pacman.transform.position = new Vector2(pacman.transform.position.x, classicModeText.transform.position.y + 5);
        if (gameData == null) gameData = GameObject.Find("GameData").GetComponent<GameData>();
        SetupView();
    }

    // Update is called once per frame
    void Update()
    {
        // Check user's inputs 
        CheckInput();
    }

    // setup position of selecting cursor for first time menu is loaded
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

    // User can use Up and Down arrow keys or W and S keys to move cursor up and down
    void CheckInput()
    {
        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            // Classic mode is set as default when first load, so if current position is classic, then it will change to
            // Innovative mode when move down. Similar to the rest options. Moving down from the lowest position option
            // will turn the cursor back to top. Moving up from top option moves cursor to bottom
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
        // User uses Enter (Return) key to select playing mode and load the relevant scene
        else if (Input.GetKeyUp(KeyCode.Return))
        {
            switch (gameData.currentMode)
            {
                case GameData.Mode.ClassicMode:
                    SceneManager.LoadSceneAsync("Classic");
                    break;
                case GameData.Mode.InnovativeMode:
                    SceneManager.LoadSceneAsync("Innovative");
                    break;
                case GameData.Mode.BattleMode:
                    SceneManager.LoadSceneAsync("Battle");
                    break;
            }
            gameData.isOver = false;
            gameData.ResetData();
        }
    }
}
