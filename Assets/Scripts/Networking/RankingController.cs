using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RankingController : MonoBehaviour
{
    public Text[] aliases;
    public Text[] scores;
    public Transform ownScoreBoard;
    public Text ownClassification;
    public Text ownAlias;
    public Text ownScore;


    public void UpdateRanking()
    {
        UpdateLeaderboard();
        FetchOwnPlayer();
    }


    public void UpdateLeaderboard()
    {
        StartCoroutine(NetUtils.FetchLeaderboard((System.Action<UnityWebRequest>)((UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log("NETWORK ERROR: " + req.isNetworkError + " , HTTP ERROR: " + req.isHttpError);
                Debug.Log($"{req.error}: {req.downloadHandler.text}");
            }
            else
            {
                Root respuesta = JsonUtility.FromJson<Root>(req.downloadHandler.text);

                Debug.Log("RESPUESTA : " + respuesta._embedded.appPlayerList.ToString());
                for (int i = 0; i < respuesta._embedded.appPlayerList.Count && i < 10; i++)
                {
                    aliases[i].text = respuesta._embedded.appPlayerList[i].aliasString;
                    scores[i].text = respuesta._embedded.appPlayerList[i].score.ToString();
                }
            }
        })));
    }

    public void SetUpOwnRanking()
    {
        if (PlayerPrefs.GetString(CONTRACT.PLAYER_SCORE, "0").Equals("0"))
        {
            ownScoreBoard.gameObject.active = false;
        }
        else
        {
            ownClassification.text = NetUtils.GetPlayer().classification.ToString();
            ownAlias.text = NetUtils.GetPlayer().aliasString;
            ownScore.text = NetUtils.GetPlayer().score.ToString();
        }        
    }

    public void FetchOwnPlayer()
    {
        if (NetUtils.IsPlayerSet())
        {
            StartCoroutine(NetUtils.FetchPlayer(NetUtils.GetPlayer().id, (UnityWebRequest req) => {
                if (req.isNetworkError || req.isHttpError)
                {
                    Debug.Log("NETWORK ERROR: " + req.isNetworkError + " , HTTP ERROR: " + req.isHttpError);
                    Debug.Log($"{req.error}: {req.downloadHandler.text}");
                }
                else
                {
                    Player respuesta = JsonUtility.FromJson<Player>(req.downloadHandler.text);
                    NetUtils.SetPlayer(respuesta);
                    SetUpOwnRanking();
                }
            }));
        }
        else
        {
            Debug.Log("PLAYER NOT SET");
        }
    }
  
}
