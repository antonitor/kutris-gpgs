using GooglePlayGames;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class NetUtils
{
    public static string HTTPS_URL = "https://jacdemanec.com:8443/";
    public static string HTTP_URL = "http://jacdemanec.com:8080/";
    public static string TOKEN;
    private static Player PLAYER;

    public static IEnumerator FetchLeaderboard(Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(HTTP_URL + "leaderboard"))
        {
            request.certificateHandler = new MyCertificateHandler();
            // Send the request and wait for a response
            yield return request.SendWebRequest();
            callback(request);
        }
    }

    public static IEnumerator FetchPlayer(int id, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(HTTP_URL + "players/" + id))
        {
            request.SetRequestHeader("Authorization", TOKEN);
            //request.certificateHandler = new MyCertificateHandler();
            // Send the request and wait for a response
            yield return request.SendWebRequest();
            callback(request);
        }
    }

    public static IEnumerator UploadScore(Player player, Action<UnityWebRequest> callback)
    {
        string url = HTTP_URL + "players/score/" + PLAYER.id;

        var jsonToSend = JsonUtility.ToJson(player);
        byte[] codeToSend = new System.Text.UTF8Encoding().GetBytes(jsonToSend);
        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonToSend))
        {
            request.uploadHandler = new UploadHandlerRaw(codeToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", TOKEN);
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            callback(request);
        }
    }

    public static IEnumerator LoginToEndPoint(string authCode, Action<UnityWebRequest> callback)
    {
        byte[] codeToSend = new System.Text.UTF8Encoding().GetBytes(authCode);
        UnityWebRequest request = UnityWebRequest.Post(HTTPS_URL + "login", authCode);
        request.uploadHandler = new UploadHandlerRaw(codeToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new MyCertificateHandler();
        yield return request.SendWebRequest();
        callback(request);
    }


    public static IEnumerator getAuthCode(Action<string> callback)
    {
        bool gotAuthCode = false;
        while (!gotAuthCode)
        {
            yield return new WaitForSeconds(5);
            Debug.Log("Trying to get server auth code.");
            string code = PlayGamesPlatform.Instance.GetServerAuthCode("");
            if (!code.Equals("") && code != null)
            {
                Debug.Log("Got server authCode: " + code);
                callback(code);
                gotAuthCode = true;
            }
        }
    }

    public static Player GetPlayer()
    {
        return PLAYER;
    }

    public static void SetPlayer(Player player)
    {
        PlayerPrefs.SetString(CONTRACT.PLAYER_ID, player.id.ToString());
        PlayerPrefs.SetString(CONTRACT.PLAYER_ALIAS, player.aliasString);
        PlayerPrefs.SetString(CONTRACT.PLAYER_SCORE, player.score.ToString());
        PlayerPrefs.SetString(CONTRACT.PLAYER_CLASSIFICATION, player.classification.ToString());
        PlayerPrefs.SetString(CONTRACT.PLAYER_LEVEL, player.level_score.ToString());
        PlayerPrefs.SetString(CONTRACT.PLAYER_LINES, player.lines_score.ToString());
        PLAYER = player;
    }

    public static bool IsPlayerSet()
    {
        return PLAYER != null;
    }

}
