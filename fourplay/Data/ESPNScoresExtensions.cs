using fourplay.Data;
public static class ESPNScoresExtensions {
    public static bool IsPostSeason(this ESPNScores scores) {
        if (scores!.Season.Type == (int)TypeOfSeason.PostSeason)
            return true;
        return false;
    }
    public static bool IsPostSeason(this Event scoreEvent) {
        if (scoreEvent!.Season.Type == (int)TypeOfSeason.PostSeason)
            return true;
        return false;
    }
}
