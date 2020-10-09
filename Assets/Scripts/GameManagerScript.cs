using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    PlayGamesConfig m_playGames = new PlayGamesConfig();
    Settings mSettings = new Settings();



    private void Awake()
    {
        //Application.runInBackground = true;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
