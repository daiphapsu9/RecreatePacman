using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    public enum Mode
    {
        ClassicMode,
        InnovativeMode,
        BattleMode
    }

    public Mode currentMode;
    // Start is called before the first frame update
    void Start()
    {
        currentMode = GameMode.Mode.ClassicMode;
        DontDestroyOnLoad(this);
    }
}
