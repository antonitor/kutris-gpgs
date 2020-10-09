using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject GUI;
    public GameObject BestScorePanel;
    public Text scoreText;
    public Text newHighScoreText;
    public InputField nameField;
    public Text validationText;
    public Button okButton;
    AudioManager audioManager;


    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }
    public void endGame()
    {
        //SHOW ENDGAME MENU; STOP MUSIC; HIDE GUI; ROTATE PLAYBOARD SMOTHLY
        //audioManager.Play("endgame");
        audioManager.StopSound("theme");        
        gameOverPanel.SetActive(true);
        GUI.SetActive(false);

        //IN CASE OF NEW HIGH SCORE SHOW HIGH SCORE MENU
        if (FindObjectOfType<ScoreController>().UpdateHighScore())
        {
            UploadScore();
            scoreText.text = "NEW HIGH SCORE " + ScoreController.score.ToString();
        }
        //ELSE SHOW NORMAL SCORE
        else
        {
            Debug.Log("NORMAL SCORE: " + ScoreController.score);
            scoreText.enabled = true;
            scoreText.text = "SCORE: " + ScoreController.score;
        }
    }

    public void playAgain()
    {
        SceneManager.LoadScene("Game");
    }

    public void GoMenu()
    {
        SceneManager.LoadScene("Menu");
        
    }

    //SHOW NEW HIGH SCORE
    void UploadScore()
    {
        if (NetUtils.GetPlayer().score < ScoreController.score)
        {
            NetUtils.GetPlayer().score = ScoreController.score;
            NetUtils.GetPlayer().lines_score = ScoreController.lines;
            NetUtils.GetPlayer().level_score = ScoreController.level;
            StartCoroutine(NetUtils.UploadScore(NetUtils.GetPlayer(), (UnityWebRequest request) =>
            {
                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("Score upload complete!");
                    Debug.Log(request.downloadHandler.text);
                }
            }));
        }
    }

}
