using System.ComponentModel.DataAnnotations.Schema;
namespace fourplay.Data;
public class NFLSpreads {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int Season { get; set; }
    public int NFLWeek { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public double HomeTeamSpread { get; set; }
    public double AwayTeamSpread { get; set; }
    public double OverUnder { get; set; }
    public DateTime GameTime { get; set; }
    public DateTime DateCreated { get; set; }
}