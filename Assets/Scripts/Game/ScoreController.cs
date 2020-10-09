using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public static int score = 0;
    public static int lines = 0;
    public static int level = 1;
    public Text scoreBoard;
    public Text linesBoard;
    public Text levelBoard;

    private void Start()
    {       
        score = 0;
        lines = 0;
        level = 1;
        scoreBoard.text = "0";
        levelBoard.text = "1";
        linesBoard.text = "0";
    }

    public void addScore(int s)
    {
        score += s;
        scoreBoard.text = score.ToString();
        scoreBoard.SetAllDirty();
    }

    public void addLines(int s)
    {
        lines += s;
        linesBoard.text = lines.ToString();
        linesBoard.SetAllDirty();
    }


    public bool UpdateHighScore()    
    {        
        if (score > PlayerPrefs.GetInt(CONTRACT.HIGH_SCORE, 0))
        {
            PlayerPrefs.SetInt(CONTRACT.HIGH_SCORE, score);
            return true;
        }
        else
        {
            return false;
        }
    }



    public void updateLvl()
    {
        //Debug.Log("Level: " + level + ", lines: " + lines + ", lines / 10:" + lines / 10f);
        if (level <= lines / 10f)
        {
            //Debug.Log("Level ++ ");
            level++;
        }

        levelBoard.text = level.ToString();
        levelBoard.SetAllDirty();
    }
}
