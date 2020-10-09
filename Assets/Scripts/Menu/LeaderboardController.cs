using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /*
    IEnumerator GetScores()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://34.254.232.251/kutris/score/read.php?top=1");
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
            personalScorePanel.SetActive(true);
            rankingPanel.SetActive(false);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            rankingPanel.SetActive(true);
            setHighScores(www.downloadHandler.text);

            // Or retrieve results as binary data
        }
    }

    IEnumerator GetOwnPossition()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://34.254.232.251/kutris/score/possition.php?id=" + PlayerPrefs.GetString("player_id"));
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            PlayerPossition possition = JsonUtility.FromJson<PlayerPossition>(www.downloadHandler.text);
            ownPossition.text = possition.playerRank;
        }
    }
    
    void setHighScores(string json)
    {
        PlayerData playerInfo = JsonUtility.FromJson<PlayerData>(json);

        if (!PlayerPrefs.HasKey("player_id"))
        {
            personalScorePanel.SetActive(false);
            for (int i = 0; i < playerNicks.Length; i++)
            {
                playerNicks[i].text = playerInfo.playerInfo[i].nick;
                scores[i].text = playerInfo.playerInfo[i].score.ToString();
            }
        }
        else
        {
            bool ranked = false;
            for (int i = 0; i < playerNicks.Length; i++)
            {
                if(playerInfo.playerInfo[i].id == (PlayerPrefs.GetString("player_id")))
                {
                    ranked = true;
                    playerNicks[i].color = Color.black;
                    playerNicks[i].fontStyle = FontStyle.Bold;
                    scores[i].color = Color.black;
                    scores[i].fontStyle = FontStyle.Bold;
                }
                playerNicks[i].text = playerInfo.playerInfo[i].nick;
                scores[i].text = playerInfo.playerInfo[i].score.ToString();
            }        
            
            if (ranked)
            {
                personalScorePanel.SetActive(false);
            }
            else
            {
                ownScore.text = PlayerPrefs.GetInt("highScore", 0).ToString();
                ownNick.text = PlayerPrefs.GetString("nick_name");
                //TODO: GET OWN POSSITION
                StartCoroutine(GetOwnPossition());
            }
        }


    }
    */
}
