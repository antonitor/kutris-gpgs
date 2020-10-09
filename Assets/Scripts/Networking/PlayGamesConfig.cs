using GooglePlayGames.BasicApi;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayGamesConfig : MonoBehaviour
{
    public Text SingStatus;
    public Text SingInButtonText;
    private PlayGamesPlatform platform;

    // Start is called before the first frame update
    void Awake()
    {
        if (!Social.localUser.authenticated)
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                .AddOauthScope("openid")
                .AddOauthScope("profile")
                .RequestServerAuthCode(false)
                .Build();
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.InitializeInstance(config);
            platform = PlayGamesPlatform.Activate();
            SingInGMS();
        }
    }


    public void SingInGMS()
    {        
            Social.Active.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    Debug.Log("GPGS ---->> SIGNED-IN AS " + Social.localUser.userName);
                    Debug.Log("SCOPE ->>> EMAILS : " + PlayGamesPlatform.Instance.HasPermission("email"));
                    Debug.Log("SCOPE ->>> PROFILE : " + PlayGamesPlatform.Instance.HasPermission("profile"));
                    Debug.Log("SCOPE ->>> OPENID : " + PlayGamesPlatform.Instance.HasPermission("openid"));

                    SingStatus.text = "Signed in as " + Social.localUser.userName;                                 
                    SingInButtonText.text = "Sing Out";

                    //string token = PlayGamesPlatform.Instance.GetIdToken();
                    //string code = PlayGamesPlatform.Instance.GetServerAuthCode("");
                    //Debug.Log("TOKEN " + token.ToString());
                    //Debug.Log("AUTH CODE " + code.ToString());

                    LoginToEndpoint();
                }
                else
                {
                    Debug.Log("GPGS ---->> SING-IN FAILED");
                    SingStatus.text = "Sign-in failed";
                    SingInButtonText.text = "Sing In";
                }
                SingStatus.SetAllDirty();
                SingInButtonText.SetAllDirty();
            });
        
    }

    public void SignInAndOut()
    { 
        if (!Social.localUser.authenticated)
        {
            SingInGMS();
        }    
        else
        {
            ((PlayGamesPlatform)Social.Active).SignOut();
            SingStatus.text = "Signed Out";
            SingInButtonText.text = "Sing In";
        }
        SingStatus.SetAllDirty();
        SingInButtonText.SetAllDirty();
    }


    private void LoginToEndpoint()
    {
            StartCoroutine(NetUtils.getAuthCode((string code) => {
                StartCoroutine(NetUtils.LoginToEndPoint(code, (UnityWebRequest req) => {
                    if (req.isNetworkError || req.isHttpError)
                    {
                        Debug.Log("NETWORK ERROR: " + req.isNetworkError + " , HTTP ERROR: " + req.isHttpError);
                        Debug.Log($"{req.error}: {req.downloadHandler.text}");
                    }
                    else
                    {
                        NetUtils.SetPlayer(JsonUtility.FromJson<Player>(req.downloadHandler.text));
                        NetUtils.TOKEN = req.GetResponseHeader("Authorization");
                        

                        Debug.Log("------------ LOGIN END POINT RESPONSE ------------------- "+
                        " HEADER Authorization:" + req.GetResponseHeader("Authorization")+
                        " BODY " + req.downloadHandler.text +
                        " ------------ LOGIN END POINT RESPONSE -------------------");
                    }
                }));
            }));      
    }


}
