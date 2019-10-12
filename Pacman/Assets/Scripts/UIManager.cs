using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText;
    public Text player2ScoreText;
    public GameObject readyCanvas;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void UpdateScore(int score, GameData.Mode gameMode, int player2Score)
    {
        scoreText.text = "Player 1 score: " + score.ToString();
        if (gameMode == GameData.Mode.BattleMode)
        {
            player2ScoreText.text = "Player 2 score: " + player2Score.ToString();
        }
    }
}
