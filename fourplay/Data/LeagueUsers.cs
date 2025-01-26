using System.ComponentModel.DataAnnotations.Schema;
using fourplay.Data;
using Microsoft.AspNetCore.Identity;
namespace fourplay.Data;
public class LeagueUsers {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string GoogleEmail { get; set; }
}