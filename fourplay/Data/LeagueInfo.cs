using System.ComponentModel.DataAnnotations.Schema;

public class LeagueInfo {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string LeagueName { get; set; }
    public DateTime DateCreated { get; set; }
}