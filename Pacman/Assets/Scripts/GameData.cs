using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Central class to store all data that will stay while the game lives such as game mode, game score and result
 */
public class GameData : MonoBehaviour
{
    /*
     * Game mode
     * Classic Mode: Pacman collects all balls to win
     * Innovative Mode: Similar to classic mode but ghosts can also consume fruits. Some fruits have special effects when consumed  
     * Battle Mode: 2 players play against each other, who is alive when the other dies wins
     * - if all balls are consumed before one of them dies, player with higher score wins
     */
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

    // members 
    public int score;
    public int player2Score;
    private int[] multiplier = { 1, 2, 4, 8 }; // ghosts consume multiplier
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
        // Get Pacmans and all ghosts reference
        GetRequiredReference();
        // keep this object alive
        DontDestroyOnLoad(this);
    }

    /*
     * function called when a ghost is consumes
     * Param: isPlayer1 - to check which player should receive score from consuming ghost 
     * Points from consuming ghosts will increase based on their multiplier
     * Multiplier increase ghosts are consumed accumulatedly in limited time (7s)
     */
    public void ConsumeGhost(bool isPlayer1 = true)
    {
        if (isPlayer1)
        {
            // score is calculated based on current position of multiplier
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

    /*
     * Consuming balls and fruits on map to get score
     */
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

    // Reset multiplier index position after limited time has passed 
    public void ResetMultiplier()
    {
        if (currentMultiplierPos != 0) currentMultiplierPos = 0;
    }

    // Reset all data when go to a new level
    public void ResetData()
    {
        currentMultiplierPos = 0;
        score = 0;
        player2Score = 0;
        isOver = false;
        ballCount = 0;
    }

    /*
     * Method to apply item effect to all ghosts
     * Used innovative and battle mode
     */
    public void AddEffectToGhosts(Effect effect)
    {
        foreach (Ghost ghost in allGhosts)
        {
            ghost.appliedEffect = effect;
            ghost.appliedEffect.StartDurationCountDown();
        }
    }

    /*
     * Method to apply item effect to all pacman
     * Used innovative and battle mode
     */
    public void AddEffectToPacman(Effect effect)
    {
        foreach (Pacman pacman in allPacmans)
        {
            pacman.appliedEffect = effect;
            pacman.appliedEffect.StartDurationCountDown();
        }
    }

    // Get Pacmans and Ghosts references in each level, also count total number of balls in the level
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
