using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Self
{
    public string href;
}

public class Players
{
    public string href;
}

public class Links
{
    public Self self;
    public Players players;
}

public class Player
{
    public int id;
    public string aliasString;
    public string gpgsId;
    public int score;
    public int classification;
    public int lines_score;
    public int level_score;
    public Links _links;
}



