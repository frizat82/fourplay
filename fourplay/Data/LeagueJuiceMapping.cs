using System.ComponentModel.DataAnnotations.Schema;

public class LeagueJuiceMapping {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public LeagueInfo League { get; set; }
    public int LeagueId { get; set; }
    public int Season { get; set; }
    public int Juice { get; set; }
    public DateTime DateCreated { get; set; }
}