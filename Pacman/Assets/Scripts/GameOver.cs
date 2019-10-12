using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private Text ScoreText;
    public GameData gameData;
    // Start is called before the first frame update
    void Start()
    {
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
        if (gameData != null)
        {
            ScoreText.text = gameData.score.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
