using System.ComponentModel.DataAnnotations.Schema;

public class NFLScores {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int Season { get; set; }
    public int NFLWeek { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public double HomeTeamScore { get; set; }
    public double AwayTeamScore { get; set; }
    public DateTime GameTime { get; set; }
    public DateTime DateCreated { get; set; }
}