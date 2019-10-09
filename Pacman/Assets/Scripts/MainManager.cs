using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    private int score;
    private int[] multiplier = { 1, 2, 4, 8 };
    private int currentMultiplierPos = 0;
    private int ghostScore = 200;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private GameObject[] ghosts;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        uiManager.UpdateScore(score);
    }

    public void ConsumeGhost(){

        score += ghostScore * multiplier[currentMultiplierPos];
        if (currentMultiplierPos < multiplier.Length - 1)
        {
            currentMultiplierPos++;
        }
    }

    public void ResetMultiplier()
    {
        if (currentMultiplierPos != 0) currentMultiplierPos = 0;
    }
}
