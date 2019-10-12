using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public enum Mode
    {
        ClassicMode,
        InnovativeMode,
        BattleMode
    }

    public enum GameResult
    {
        Win,
        Lose,
        Player1Win,
        Player2Win,
        Draw
    }

    public int score;
    public int player2Score;
    private int[] multiplier = { 1, 2, 4, 8 };
    private int currentMultiplierPos = 0;
    private int currentPlayer2MultiplierPos = 0;
    private int ghostScore = 200;
    private int smallBallScore = 10;
    private int bigBallScore = 50;
    public bool isOver = false;
    public Mode currentMode;
    public ArrayList allGhosts = new ArrayList();
    public ArrayList allPacmans = new ArrayList();
    public int ballCount;
    public GameResult gameResult;

    // Start is called before the first frame update
    void Start()
    {
        currentMode = GameData.Mode.ClassicMode;
        GetRequiredReference();
        DontDestroyOnLoad(this);
    }

    public void ConsumeGhost(bool isPlayer1 = true)
    {
        if (isPlayer1)
        {
            score += ghostScore * multiplier[currentMultiplierPos];
            if (currentMultiplierPos < multiplier.Length - 1)
            {
                currentMultiplierPos++;
            }
        }
        else
        {
            player2Score += ghostScore * multiplier[currentPlayer2MultiplierPos];
            if (currentPlayer2MultiplierPos < multiplier.Length - 1)
            {
                currentPlayer2MultiplierPos++;
            }
        }
    }

    public void ConsumeSmallBall(bool isPlayer1 = true)
    {
        if (isPlayer1)
        {
            score += smallBallScore;
        } else player2Score += smallBallScore;

        ballCount--;
        if (ballCount == 0)
        {
            isOver = true;
            if (score > player2Score) { gameResult = GameResult.Player1Win; }
            else if (score < player2Score) { gameResult = GameResult.Player2Win; }
            else gameResult = GameResult.Draw;
        }
    }

    public void ConsumeBigBall(bool isPlayer1 = true)
    {
        if (isPlayer1)
        {
            score += bigBallScore;
        }
        else player2Score += bigBallScore;
        ballCount--;
        if (ballCount == 0)
        {
            isOver = true;
            if (score > player2Score) { gameResult = GameResult.Player1Win; }
            else if (score < player2Score) { gameResult = GameResult.Player2Win; }
            else gameResult = GameResult.Draw;
        }
    }

    public void ConsumeFruit(Fruit fruit, bool isPlayer1 = true)
    {
        if (isPlayer1)
        {
            score += fruit.GetPoints();
        }
        else player2Score += fruit.GetPoints();
    }

    public void ResetMultiplier()
    {
        if (currentMultiplierPos != 0) currentMultiplierPos = 0;
    }

    public void ResetData()
    {
        currentMultiplierPos = 0;
        score = 0;
        isOver = false;
    }

    public void AddEffectToGhosts(Effect effect)
    {
        foreach (Ghost ghost in allGhosts)
        {
            ghost.appliedEffect = effect;
            ghost.appliedEffect.StartDurationCountDown();
        }
    }

    public void AddEffectToPacman(Effect effect)
    {
        foreach (Pacman pacman in allPacmans)
        {
            pacman.appliedEffect = effect;
            pacman.appliedEffect.StartDurationCountDown();
        }
    }

    // Get Pacman and Ghosts references in each level
    public void GetRequiredReference()
    {
        allGhosts = new ArrayList();
        allPacmans = new ArrayList();
        GameObject[] pacmanObjectList = GameObject.FindGameObjectsWithTag("Pacman");
        foreach (GameObject pacman in pacmanObjectList)
        {
            allPacmans.Add(pacman.GetComponent<Pacman>());
        }
        GameObject[] ghostObjectList = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject ghostObject in ghostObjectList)
        {
            allGhosts.Add(ghostObject.GetComponent<Ghost>());
        }
        ballCount += GameObject.FindGameObjectsWithTag("SmallBall").Length;
        ballCount += GameObject.FindGameObjectsWithTag("BigBall").Length;
    }
}
