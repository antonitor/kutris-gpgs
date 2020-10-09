using System.Collections.Generic;

[System.Serializable]
public class AppPlayerList
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

[System.Serializable]
public class Embedded
{
    public List<AppPlayerList> appPlayerList;
}

[System.Serializable]
public class Self2
{
    public string href;
}

[System.Serializable]
public class Links2
{
    public Self2 self;
}

[System.Serializable]
public class Root
{
    public Embedded _embedded;
    public Links2 _links;
}
