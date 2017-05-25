using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Scores {

    public static List<PlayerScore> scoresList = new List<PlayerScore>();
    public static List<PlayerScore> UberStageScoresList = new List<PlayerScore>();

    public static void AddPlayerScore(string name, float score)
    {
        scoresList.Add(new PlayerScore(name, score));
        scoresList.Sort();
        scoresList.Reverse();
    }

    public static string GetNames()
    {
        string names = "";
        for (int i = 0; i < scoresList.Count; i++)
        {
            names += scoresList[i].Name + "\n";
        }
        return names;
    }

    public static string GetScores()
    {
        string scores = "";
        for (int i = 0; i < scoresList.Count; i++)
        {
            scores += scoresList[i].Score + "\n";
        }
        return scores;
    }

    #region Uber Stage

    public static void AddPlayerScoreUberStage(string name, float score)
    {
        UberStageScoresList.Add(new PlayerScore(name, score));
        UberStageScoresList.Sort();
    }

    public static string GetNamesUberStage()
    {
        string names = "";
        for (int i = 0; i < UberStageScoresList.Count; i++)
        {
            names += UberStageScoresList[i].Name + "\n";
        }
        return names;
    }

    public static string GetUberStageTimes()
    {
        string times = "";
        for (int i = 0; i < UberStageScoresList.Count; i++)
        {
            float time = UberStageScoresList[i].Score;
            float seconds = time % 60f;
            float minutes = time / 60;
            
            times += string.Format("{0:00}:{1:00}", (int)minutes, (int)seconds) + "\n";
        }
        return times;
    }
    #endregion
}

public class PlayerScore : IComparable<PlayerScore>
{
    public float Score { get; set; }
    public string Name { get; set; }

    public PlayerScore(string name, float score)
    {
        Name = name;
        Score = score;
    }

    public int CompareTo(PlayerScore comparePart)
    {
        if (comparePart == null)
            return 1;

        else
            return Score.CompareTo(comparePart.Score);
    }
}
