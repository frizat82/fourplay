using System.ComponentModel.DataAnnotations.Schema;
using fourplay.Data;
using Microsoft.AspNetCore.Identity;

public class NFLPicks {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    
    public int Id { get; set; }
    public LeagueInfo League { get; set; }
    public int LeagueId { get; set; }
    // Foreign key to the AspNetUsers table
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string Team { get; set; }
    public int NFLWeek { get; set; }
    public int Season { get; set; }
    public DateTime DateCreated { get; set; }
}