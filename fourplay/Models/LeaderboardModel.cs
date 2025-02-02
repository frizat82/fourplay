using fourplay.Data;
using fourplay.Models.Enum;
using System;

namespace fourplay.Models;

public class LeaderboardModel {
    public ApplicationUser User { get; set; }
    public long Total { get; set; }
    public LeaderboardWeekResults[] WeekResults { get; set; }
}

public class LeaderboardWeekResults {
    public int Week { get; set; }
    public WeekResult WeekResult { get; set; }
    public long Score { get; set; }
}
