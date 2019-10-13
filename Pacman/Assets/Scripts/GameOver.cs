using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Class that support Game Over scene, display game result and score
public class GameOver : MonoBehaviour
{
    [SerializeField]
    private Text ScoreText;
    [SerializeField]
    private GameObject WinText;
    [SerializeField]
    private GameObject LoseText;
    [SerializeField]
    private GameObject Player1WinText;
    [SerializeField]
    private GameObject Player2WinText;
    [SerializeField]
    private GameObject DrawText;
    public GameData gameData;
    void Start()
    {
        // get and store Game Data reference
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
        // display score
        if (gameData != null)
        {
            ScoreText.text = gameData.score.ToString();
        }
        switch (gameData.gameResult)
        {
            case GameData.GameResult.Win:
                WinText.SetActive(true);
                break;
            case GameData.GameResult.Lose:
                LoseText.SetActive(true);
                break;
            case GameData.GameResult.Player1Win:
                Player1WinText.SetActive(true);
                break;
            case GameData.GameResult.Player2Win:
                // in battle mode, display player 2 score if they win
                Player2WinText.SetActive(true);
                ScoreText.text = gameData.player2Score.ToString();
                break;
            case GameData.GameResult.Draw:
                DrawText.SetActive(true);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
