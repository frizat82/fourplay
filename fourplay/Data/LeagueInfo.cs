using System.ComponentModel.DataAnnotations.Schema;
namespace fourplay.Data;
public class LeagueInfo {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string LeagueName { get; set; }
    public DateTime DateCreated { get; set; }
}