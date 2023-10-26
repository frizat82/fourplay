using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

public class NFLPicks
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    // Foreign key to the AspNetUsers table
    public string UserId { get; set; }
    public IdentityUser User { get; set; }
    public string Team { get; set; }
    public int NFLWeek { get; set; }
    public int Season { get; set; }
    public DateTime DateCreated { get; set; }
}