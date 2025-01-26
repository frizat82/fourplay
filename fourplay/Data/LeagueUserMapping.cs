using System.ComponentModel.DataAnnotations.Schema;
using fourplay.Data;
using Microsoft.AspNetCore.Identity;
namespace fourplay.Data;
public class LeagueUserMapping {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public int Id { get; set; }
    public LeagueInfo League { get; set; }
    public int LeagueId { get; set; }
    // Foreign key to the AspNetUsers table
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public DateTime DateCreated { get; set; }
}