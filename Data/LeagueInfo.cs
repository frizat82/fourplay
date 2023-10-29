using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

public class LeagueInfo {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string LeagueName { get; set; }
    public int Season { get; set; }
    public int Juice { get; set; }

    public DateTime DateCreated { get; set; }
}