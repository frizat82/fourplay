using System.Text.Json;
using System.Text.Json.Serialization;

namespace SportslineOdds;

public partial class SportslineOddsData {
    [JsonPropertyName("data")]
    public Data Data { get; set; }
}

public partial class Data {
    [JsonPropertyName("odds")]
    public Odds Odds { get; set; }
}

public partial class Odds {
    [JsonPropertyName("sportsBooks")]
    public SportsBook[] SportsBooks { get; set; }

    [JsonPropertyName("oddsCompetitions")]
    public OddsCompetition[] OddsCompetitions { get; set; }
}

public partial class OddsCompetition {
    [JsonPropertyName("id")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }

    [JsonPropertyName("abbr")]
    public string Abbr { get; set; }

    [JsonPropertyName("homeTeamId")]
    public long HomeTeamId { get; set; }

    [JsonPropertyName("awayTeamId")]
    public long AwayTeamId { get; set; }

    [JsonPropertyName("homeTeam")]
    public Team HomeTeam { get; set; }

    [JsonPropertyName("awayTeam")]
    public Team AwayTeam { get; set; }

    [JsonPropertyName("homeTeamForecast")]
    public object HomeTeamForecast { get; set; }

    [JsonPropertyName("awayTeamForecast")]
    public object AwayTeamForecast { get; set; }

    [JsonPropertyName("venue")]
    public Venue Venue { get; set; }

    [JsonPropertyName("neutral")]
    public bool Neutral { get; set; }

    [JsonPropertyName("tvInfo")]
    public TvInfo[] TvInfo { get; set; }

    [JsonPropertyName("scheduledTime")]
    public DateTimeOffset ScheduledTime { get; set; }

    [JsonPropertyName("competitionStatus")]
    public CompetitionStatus CompetitionStatus { get; set; }

    [JsonPropertyName("league")]
    public League League { get; set; }

    [JsonPropertyName("sport")]
    public Sport Sport { get; set; }

    [JsonPropertyName("sportsBookOdds")]
    public SportsBookOdds SportsBookOdds { get; set; }

    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; }

    [JsonPropertyName("matchupBreakdowns")]
    public MatchupBreakdown[] MatchupBreakdowns { get; set; }

    [JsonPropertyName("expertPicksCount")]
    public long ExpertPicksCount { get; set; }
}

public partial class Team {
    [JsonPropertyName("id")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }

    [JsonPropertyName("league")]
    public League League { get; set; }

    [JsonPropertyName("sport")]
    public Sport Sport { get; set; }

    [JsonPropertyName("location")]
    public string Location { get; set; }

    [JsonPropertyName("nickName")]
    public string NickName { get; set; }

    [JsonPropertyName("mediumName")]
    public string MediumName { get; set; }

    [JsonPropertyName("shortName")]
    public string ShortName { get; set; }

    [JsonPropertyName("abbr")]
    public string Abbr { get; set; }

    [JsonPropertyName("status")]
    public Status Status { get; set; }

    [JsonPropertyName("color")]
    public string Color { get; set; }

    [JsonPropertyName("colorPrimary")]
    public string ColorPrimary { get; set; }

    [JsonPropertyName("colorSecondary")]
    public string ColorSecondary { get; set; }

    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; }
}

public partial class MatchupBreakdown {
    [JsonPropertyName("awayStats")]
    public string AwayStats { get; set; }

    [JsonPropertyName("homeStats")]
    public string HomeStats { get; set; }

    [JsonPropertyName("label")]
    public Label Label { get; set; }

    [JsonPropertyName("sortOrder")]
    public long SortOrder { get; set; }
}

public partial class SportsBookOdds {
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("draftkings")]
    public Bet365Newjersey Draftkings { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("fanduel")]
    public Bet365Newjersey Fanduel { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("bet365newjersey")]
    public Bet365Newjersey Bet365Newjersey { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("caesars")]
    public Bet365Newjersey Caesars { get; set; }

    [JsonPropertyName("consensus")]
    public Bet365Newjersey Consensus { get; set; }
}

public partial class Bet365Newjersey {
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("moneyline")]
    public Moneyline Moneyline { get; set; }

    [JsonPropertyName("spread")]
    public Moneyline Spread { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("total")]
    public Total Total { get; set; }
}

public partial class Moneyline {
    [JsonPropertyName("home")]
    public Away Home { get; set; }

    [JsonPropertyName("away")]
    public Away Away { get; set; }
}

public partial class Away {
    [JsonPropertyName("source")]
    public Source Source { get; set; }

    [JsonPropertyName("bookId")]
    public BookId BookId { get; set; }

    [JsonPropertyName("bookName")]
    public BookName BookName { get; set; }

    [JsonPropertyName("bookDisplayName")]
    public BookName BookDisplayName { get; set; }

    [JsonPropertyName("type")]
    public TypeEnum Type { get; set; }

    [JsonPropertyName("side")]
    public Side Side { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("openingValue")]
    public string OpeningValue { get; set; }

    [JsonPropertyName("outcomeOdds")]
    public string OutcomeOdds { get; set; }

    [JsonPropertyName("openingOutcomeOdds")]
    public string OpeningOutcomeOdds { get; set; }

    [JsonPropertyName("selectionId")]
    public object SelectionId { get; set; }

    [JsonPropertyName("timestamp")]
    public object Timestamp { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("competition")]
    public object Competition { get; set; }
}

public partial class Total {
    [JsonPropertyName("over")]
    public Away Over { get; set; }

    [JsonPropertyName("under")]
    public Away Under { get; set; }
}

public partial class TvInfo {
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("callLetters")]
    public string CallLetters { get; set; }

    [JsonPropertyName("countryId")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long CountryId { get; set; }

    [JsonPropertyName("teamId")]
    public object TeamId { get; set; }

    [JsonPropertyName("countryName")]
    public CountryName CountryName { get; set; }

    [JsonPropertyName("typeId")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long TypeId { get; set; }

    [JsonPropertyName("TvInfoTypeName")]
    public TvInfoTypeName TvInfoTypeName { get; set; }
}

public partial class Venue {
    [JsonPropertyName("id")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }

    [JsonPropertyName("abbr")]
    public string Abbr { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("country")]
    public Country? Country { get; set; }

    [JsonPropertyName("zipCode")]
    public string ZipCode { get; set; }

    [JsonPropertyName("timeZone")]
    public string TimeZone { get; set; }

    [JsonPropertyName("weatherCode")]
    public string WeatherCode { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public partial class SportsBook {
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("sortOrder")]
    public long SortOrder { get; set; }
}

public enum League { Nfl };

public enum Sport { Football };

public enum Status { C };

public enum CompetitionStatus { Scheduled, InProgress, Final };

public enum Label { AtsWL, OU, PtDiff, WL };

public enum BookName { Bet365NewJersey, Consensus, DraftKings, FanDuel, WilliamHillNewJersey, Unknown };

public enum BookId { Consensus, SrBook18149, SrBook18186, SrBook28901, SrBook32219 };

public enum Side { Away, Home, Over, Under };

public enum Source { Consensus, Market };

public enum TypeEnum { MoneyLine, OverUnder, PointSpread };

public enum CountryName { UnitedStates };

public enum TvInfoTypeName { National };

public enum Country { Usa };

internal static class SportslineOddsConverters {
    public static readonly JsonSerializerOptions Settings = new(JsonSerializerDefaults.General) {
        Converters =
            {
                LeagueConverter.Singleton,
                SportConverter.Singleton,
                StatusConverter.Singleton,
                CompetitionStatusConverter.Singleton,
                LabelConverter.Singleton,
                BookNameConverter.Singleton,
                BookIdConverter.Singleton,
                SideConverter.Singleton,
                SourceConverter.Singleton,
                TypeEnumConverter.Singleton,
                CountryNameConverter.Singleton,
                TvInfoTypeNameConverter.Singleton,
                CountryConverter.Singleton,
                new DateOnlyConverter(),
                new TimeOnlyConverter(),
                IsoDateTimeOffsetConverter.Singleton
            },
    };
}


internal class LeagueConverter : JsonConverter<League> {
    public override bool CanConvert(Type t) => t == typeof(League);

    public override League Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        if (value == "NFL") {
            return League.Nfl;
        }
        throw new Exception("Cannot unmarshal type League");
    }

    public override void Write(Utf8JsonWriter writer, League value, JsonSerializerOptions options) {
        if (value == League.Nfl) {
            JsonSerializer.Serialize(writer, "NFL", options);
            return;
        }
        throw new Exception("Cannot marshal type League");
    }

    public static readonly LeagueConverter Singleton = new();
}

internal class SportConverter : JsonConverter<Sport> {
    public override bool CanConvert(Type t) => t == typeof(Sport);

    public override Sport Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        if (value == "FOOTBALL") {
            return Sport.Football;
        }
        throw new Exception("Cannot unmarshal type Sport");
    }

    public override void Write(Utf8JsonWriter writer, Sport value, JsonSerializerOptions options) {
        if (value == Sport.Football) {
            JsonSerializer.Serialize(writer, "FOOTBALL", options);
            return;
        }
        throw new Exception("Cannot marshal type Sport");
    }

    public static readonly SportConverter Singleton = new();
}

internal class StatusConverter : JsonConverter<Status> {
    public override bool CanConvert(Type t) => t == typeof(Status);

    public override Status Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        if (value == "C") {
            return Status.C;
        }
        throw new Exception("Cannot unmarshal type Status");
    }

    public override void Write(Utf8JsonWriter writer, Status value, JsonSerializerOptions options) {
        if (value == Status.C) {
            JsonSerializer.Serialize(writer, "C", options);
            return;
        }
        throw new Exception("Cannot marshal type Status");
    }

    public static readonly StatusConverter Singleton = new();
}

internal class CompetitionStatusConverter : JsonConverter<CompetitionStatus> {
    public override bool CanConvert(Type t) => t == typeof(CompetitionStatus);

    public override CompetitionStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        if (value == "SCHEDULED") {
            return CompetitionStatus.Scheduled;
        }
        if (value == "IN_PROGRESS") {
            return CompetitionStatus.InProgress;
        }
        if (value == "FINAL") {
            return CompetitionStatus.Final;
        }
        throw new Exception("Cannot unmarshal type CompetitionStatus");
    }

    public override void Write(Utf8JsonWriter writer, CompetitionStatus value, JsonSerializerOptions options) {
        if (value == CompetitionStatus.Scheduled) {
            JsonSerializer.Serialize(writer, "SCHEDULED", options);
            return;
        }
        throw new Exception("Cannot marshal type CompetitionStatus");
    }

    public static readonly CompetitionStatusConverter Singleton = new();
}

internal class LabelConverter : JsonConverter<Label> {
    public override bool CanConvert(Type t) => t == typeof(Label);

    public override Label Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "ATS W-L":
                return Label.AtsWL;
            case "O/U":
                return Label.OU;
            case "PT DIFF":
                return Label.PtDiff;
            case "W-L":
                return Label.WL;
        }
        throw new Exception("Cannot unmarshal type Label");
    }

    public override void Write(Utf8JsonWriter writer, Label value, JsonSerializerOptions options) {
        switch (value) {
            case Label.AtsWL:
                JsonSerializer.Serialize(writer, "ATS W-L", options);
                return;
            case Label.OU:
                JsonSerializer.Serialize(writer, "O/U", options);
                return;
            case Label.PtDiff:
                JsonSerializer.Serialize(writer, "PT DIFF", options);
                return;
            case Label.WL:
                JsonSerializer.Serialize(writer, "W-L", options);
                return;
        }
        throw new Exception("Cannot marshal type Label");
    }

    public static readonly LabelConverter Singleton = new();
}

internal class BookNameConverter : JsonConverter<BookName> {
    public override bool CanConvert(Type t) => t == typeof(BookName);

    public override BookName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "Bet365NewJersey":
                return BookName.Bet365NewJersey;
            case "Consensus":
                return BookName.Consensus;
            case "DraftKings":
                return BookName.DraftKings;
            case "FanDuel":
                return BookName.FanDuel;
            case "WilliamHillNewJersey":
                return BookName.WilliamHillNewJersey;
            case "WilliamHill":
                return BookName.WilliamHillNewJersey;
            default:
                return BookName.Unknown;
        }
        throw new Exception("Cannot unmarshal type BookName");
    }

    public override void Write(Utf8JsonWriter writer, BookName value, JsonSerializerOptions options) {
        switch (value) {
            case BookName.Bet365NewJersey:
                JsonSerializer.Serialize(writer, "Bet365NewJersey", options);
                return;
            case BookName.Consensus:
                JsonSerializer.Serialize(writer, "Consensus", options);
                return;
            case BookName.DraftKings:
                JsonSerializer.Serialize(writer, "DraftKings", options);
                return;
            case BookName.FanDuel:
                JsonSerializer.Serialize(writer, "FanDuel", options);
                return;
            case BookName.WilliamHillNewJersey:
                JsonSerializer.Serialize(writer, "WilliamHillNewJersey", options);
                return;
        }
        throw new Exception("Cannot marshal type BookName");
    }

    public static readonly BookNameConverter Singleton = new();
}

internal class BookIdConverter : JsonConverter<BookId> {
    public override bool CanConvert(Type t) => t == typeof(BookId);

    public override BookId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "consensus":
                return BookId.Consensus;
            case "sr:book:18149":
                return BookId.SrBook18149;
            case "sr:book:18186":
                return BookId.SrBook18186;
            case "sr:book:28901":
                return BookId.SrBook28901;
            case "sr:book:32219":
                return BookId.SrBook32219;
        }
        throw new Exception("Cannot unmarshal type BookId");
    }

    public override void Write(Utf8JsonWriter writer, BookId value, JsonSerializerOptions options) {
        switch (value) {
            case BookId.Consensus:
                JsonSerializer.Serialize(writer, "consensus", options);
                return;
            case BookId.SrBook18149:
                JsonSerializer.Serialize(writer, "sr:book:18149", options);
                return;
            case BookId.SrBook18186:
                JsonSerializer.Serialize(writer, "sr:book:18186", options);
                return;
            case BookId.SrBook28901:
                JsonSerializer.Serialize(writer, "sr:book:28901", options);
                return;
            case BookId.SrBook32219:
                JsonSerializer.Serialize(writer, "sr:book:32219", options);
                return;
        }
        throw new Exception("Cannot marshal type BookId");
    }

    public static readonly BookIdConverter Singleton = new();
}

internal class SideConverter : JsonConverter<Side> {
    public override bool CanConvert(Type t) => t == typeof(Side);

    public override Side Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "AWAY":
                return Side.Away;
            case "HOME":
                return Side.Home;
            case "OVER":
                return Side.Over;
            case "UNDER":
                return Side.Under;
        }
        throw new Exception("Cannot unmarshal type Side");
    }

    public override void Write(Utf8JsonWriter writer, Side value, JsonSerializerOptions options) {
        switch (value) {
            case Side.Away:
                JsonSerializer.Serialize(writer, "AWAY", options);
                return;
            case Side.Home:
                JsonSerializer.Serialize(writer, "HOME", options);
                return;
            case Side.Over:
                JsonSerializer.Serialize(writer, "OVER", options);
                return;
            case Side.Under:
                JsonSerializer.Serialize(writer, "UNDER", options);
                return;
        }
        throw new Exception("Cannot marshal type Side");
    }

    public static readonly SideConverter Singleton = new();
}

internal class SourceConverter : JsonConverter<Source> {
    public override bool CanConvert(Type t) => t == typeof(Source);

    public override Source Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "CONSENSUS":
                return Source.Consensus;
            case "MARKET":
                return Source.Market;
            case "TEAM_MARKET":
                return Source.Market;
        }
        throw new Exception("Cannot unmarshal type Source");
    }

    public override void Write(Utf8JsonWriter writer, Source value, JsonSerializerOptions options) {
        switch (value) {
            case Source.Consensus:
                JsonSerializer.Serialize(writer, "CONSENSUS", options);
                return;
            case Source.Market:
                JsonSerializer.Serialize(writer, "MARKET", options);
                return;
        }
        throw new Exception("Cannot marshal type Source");
    }

    public static readonly SourceConverter Singleton = new();
}

internal class TypeEnumConverter : JsonConverter<TypeEnum> {
    public override bool CanConvert(Type t) => t == typeof(TypeEnum);

    public override TypeEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "MONEY_LINE":
                return TypeEnum.MoneyLine;
            case "OVER_UNDER":
                return TypeEnum.OverUnder;
            case "POINT_SPREAD":
                return TypeEnum.PointSpread;
        }
        throw new Exception("Cannot unmarshal type TypeEnum");
    }

    public override void Write(Utf8JsonWriter writer, TypeEnum value, JsonSerializerOptions options) {
        switch (value) {
            case TypeEnum.MoneyLine:
                JsonSerializer.Serialize(writer, "MONEY_LINE", options);
                return;
            case TypeEnum.OverUnder:
                JsonSerializer.Serialize(writer, "OVER_UNDER", options);
                return;
            case TypeEnum.PointSpread:
                JsonSerializer.Serialize(writer, "POINT_SPREAD", options);
                return;
        }
        throw new Exception("Cannot marshal type TypeEnum");
    }

    public static readonly TypeEnumConverter Singleton = new();
}

internal class CountryNameConverter : JsonConverter<CountryName> {
    public override bool CanConvert(Type t) => t == typeof(CountryName);

    public override CountryName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        if (value == "United States") {
            return CountryName.UnitedStates;
        }
        throw new Exception("Cannot unmarshal type CountryName");
    }

    public override void Write(Utf8JsonWriter writer, CountryName value, JsonSerializerOptions options) {
        if (value == CountryName.UnitedStates) {
            JsonSerializer.Serialize(writer, "United States", options);
            return;
        }
        throw new Exception("Cannot marshal type CountryName");
    }

    public static readonly CountryNameConverter Singleton = new();
}

internal class TvInfoTypeNameConverter : JsonConverter<TvInfoTypeName> {
    public override bool CanConvert(Type t) => t == typeof(TvInfoTypeName);

    public override TvInfoTypeName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        if (value == "National") {
            return TvInfoTypeName.National;
        }
        throw new Exception("Cannot unmarshal type TvInfoTypeName");
    }

    public override void Write(Utf8JsonWriter writer, TvInfoTypeName value, JsonSerializerOptions options) {
        if (value == TvInfoTypeName.National) {
            JsonSerializer.Serialize(writer, "National", options);
            return;
        }
        throw new Exception("Cannot marshal type TvInfoTypeName");
    }

    public static readonly TvInfoTypeNameConverter Singleton = new();
}

internal class CountryConverter : JsonConverter<Country?> {
    public override bool HandleNull => true;
    public override bool CanConvert(Type t) => t == typeof(Country?);

    public override Country? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        if (value == "USA") {
            return Country.Usa;
        }
        else
            return null;
        throw new Exception("Cannot unmarshal type Country");
    }

    public override void Write(Utf8JsonWriter writer, Country? value, JsonSerializerOptions options) {
        if (value == null) {
            writer.WriteStringValue(string.Empty);
        }
        if (value == Country.Usa) {
            JsonSerializer.Serialize(writer, "USA", options);
            return;
        }
        throw new Exception("Cannot marshal type Country");
    }

    public static readonly CountryConverter Singleton = new();
}



