using System.ComponentModel.DataAnnotations.Schema;
namespace fourplay.Data;
public class LeagueJuiceMapping {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public LeagueInfo League { get; set; }
    public int LeagueId { get; set; }
    public int Season { get; set; }
    public int Juice { get; set; } = 13;
    public int JuiceDivisonal { get; set; } = 10;
    public int JuiceConference { get; set; } = 6;
    public int WeeklyCost { get; set; }
    public DateTime DateCreated { get; set; }
}