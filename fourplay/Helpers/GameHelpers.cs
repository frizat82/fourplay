using System;

namespace fourplay.Helpers;

public static class GameHelpers {
    public static string GetWeekName(long week, bool isPostSeason = false) {
        if (!isPostSeason) {
            return $"Week {week}";
        }
        else {
            return week switch {
                1 => "Wild Card",
                2 => "Divisional",
                3 => "Conference",
                4 => "Super Bowl",
                _ => throw new ArgumentException("Invalid week number")
            };
        }
    }
    public static int GetRequiredPicks(long week, bool isPostSeason = false) {
        if (!isPostSeason) {
            return 4;
        }
        else {
            return week switch {
                1 => 3,
                2 => 3,
                3 => 2,
                4 => 1,
                _ => throw new ArgumentException("Invalid week number")
            };
        }
    }
    public static Competition GetCompetitionFromHomeAwayAbbr(string homeTeamAbbr, string awayTeamAbbr, ESPNScores scores) {
        foreach (var scoreEvent in scores.Events) {
            foreach (var competition in scoreEvent.Competitions) {
                if (GetHomeTeamAbbr(competition) == homeTeamAbbr && GetAwayTeamAbbr(competition) == awayTeamAbbr) {
                    return competition;
                }
            }
        }
        throw new ArgumentException("Competition not found");
    }
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
