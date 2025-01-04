using System;

namespace fourplay.Helpers;

public static class GameHelpers {
    public static string GetAwayTeamAbbr(Competition competition) => GetAwayTeam(competition).Team.Abbreviation;
    public static string GetHomeTeamAbbr(Competition competition) => GetHomeTeam(competition).Team.Abbreviation;
    public static Competitor GetAwayTeam(Competition competition) => competition.Competitors[1];
    public static Competitor GetHomeTeam(Competition competition) => competition.Competitors[0];
    public static string GetAwayTeamLogo(Competition competition) => GetAwayTeam(competition).Team.Logo.ToString();
    public static string GetHomeTeamLogo(Competition competition) => GetHomeTeam(competition).Team.Logo.ToString();
    public static long GetHomeTeamScore(Competition competition) => GetHomeTeam(competition).Score;
    public static long GetAwayTeamScore(Competition competition) => GetAwayTeam(competition).Score;

    public static bool IsGameStarted(Competition competition) => competition.Status.Type.Name != TypeName.StatusScheduled;

}
