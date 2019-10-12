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
    }

    public int score;
    private int[] multiplier = { 1, 2, 4, 8 };
    private int currentMultiplierPos = 0;
    private int ghostScore = 200;
    private int smallBallScore = 10;
    private int bigBallScore = 50;
    public bool isOver = false;
    public Mode currentMode;
    public ArrayList allGhosts = new ArrayList();
    public Pacman pacman;
    public int ballCount;
    public GameResult gameResult;

    // Start is called before the first frame update
    void Start()
    {
        currentMode = GameData.Mode.ClassicMode;
        DontDestroyOnLoad(this);
    }

    public void ConsumeGhost()
    {

        score += ghostScore * multiplier[currentMultiplierPos];
        if (currentMultiplierPos < multiplier.Length - 1)
        {
            currentMultiplierPos++;
        }
    }

    public void ConsumeSmallBall()
    {
        score += smallBallScore;
        ballCount--;
        if (ballCount == 0)
        {
            isOver = true;
            gameResult = GameResult.Win;
        }
    }

    public void ConsumeBigBall()
    {
        score += bigBallScore;
        ballCount--;
        if (ballCount == 0)
        {
            isOver = true;
            gameResult = GameResult.Win;
        }
    }

    public void ConsumeFruit(Fruit fruit)
    {
        score += fruit.GetPoints();
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
        pacman.appliedEffect = effect;
        pacman.appliedEffect.StartDurationCountDown();
    }

    // Get Pacman and Ghosts references in each level
    public void GetRequiredReference()
    {
        GameObject[] ghostObjectList = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject ghostObject in ghostObjectList)
        {
            allGhosts.Add(ghostObject.GetComponent<Ghost>());
        }
        pacman = GameObject.FindGameObjectWithTag("Pacman").GetComponent<Pacman>();
        ballCount += GameObject.FindGameObjectsWithTag("SmallBall").Length;
        ballCount += GameObject.FindGameObjectsWithTag("BigBall").Length;
    }
}
