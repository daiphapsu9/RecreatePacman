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

    public int score;
    private int[] multiplier = { 1, 2, 4, 8 };
    private int currentMultiplierPos = 0;
    private int ghostScore = 200;
    private int smallBallScore = 10;
    private int bigBallScore = 10;
    public bool isOver = false;

    public Mode currentMode;
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

    public void ResetData()
    {
        currentMultiplierPos = 0;
        score = 0;
        isOver = false;
    }
}
