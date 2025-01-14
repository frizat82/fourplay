public static class ESPNScoresExtensions
{
    public static bool IsPostSeason(this ESPNScores scores)
    {
        if (scores!.Season.Type == (int)TypeOfSeason.PostSeason)
            return true;
        return false;
    }
}
