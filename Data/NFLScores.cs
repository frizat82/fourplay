using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

public class NFLScores
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public int Season { get; set; }
    public int NFLWeek { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public double HomeTeamScore { get; set; }
    public double AwayTeamScore { get; set; }
    public DateTime GameTime { get; set; }
    public DateTime DateCreated { get; set; }
}