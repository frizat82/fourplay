using System.ComponentModel.DataAnnotations.Schema;
using fourplay.Models.Enum;
namespace fourplay.Data;
public class NFLPostSeasonPicks : IEquatable<NFLPostSeasonPicks> {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public int Id { get; set; }
    public LeagueInfo League { get; set; }
    public int LeagueId { get; set; }
    // Foreign key to the AspNetUsers table
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string Team { get; set; }
    public PickType Pick { get; set; }
    public int NFLWeek { get; set; }
    public int Season { get; set; }
    public DateTime DateCreated { get; set; }
    public override int GetHashCode() {
        return HashCode.Combine(Team, Pick);
    }

    public override bool Equals(object obj) {
        return Equals(obj as NFLPostSeasonPicks);
    }

    public bool Equals(NFLPostSeasonPicks other) {
        if (other == null)
            return false;

        return Team == other.Team && Pick == other.Pick && Season == other.Season
        && NFLWeek == other.NFLWeek && UserId == other.UserId && LeagueId == other.LeagueId;
    }

    public static bool operator ==(NFLPostSeasonPicks left, NFLPostSeasonPicks right) {
        if (left is null)
            return right is null;

        return left.Equals(right);
    }

    public static bool operator !=(NFLPostSeasonPicks left, NFLPostSeasonPicks right) {
        return !(left == right);
    }
}