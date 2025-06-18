using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace fourplay.Data;

public partial class ESPNScores {
    [JsonPropertyName("leagues")]
    public ESPNLeague[] Leagues { get; set; }

    [JsonPropertyName("season")]
    public Season Season { get; set; }

    [JsonPropertyName("week")]
    public Week Week { get; set; }

    [JsonPropertyName("events")]
    public Event[] Events { get; set; }
}

public partial class Event {
    [JsonPropertyName("season")]
    public Season Season { get; set; }
    [JsonPropertyName("week")]
    public Week Week { get; set; }

    [JsonPropertyName("date")]
    public DateTimeOffset Date { get; set; }

    [JsonPropertyName("competitions")]
    public Competition[] Competitions { get; set; }

}

public class Competition {

    [JsonPropertyName("date")]
    //[JsonConverter(typeof(IsoDateTimeOffsetConverter))]
    public DateTimeOffset Date { get; set; }

    [JsonPropertyName("competitors")]
    public Competitor[] Competitors { get; set; }

    [JsonPropertyName("status")]
    public ESPNStatus Status { get; set; }
    public override int GetHashCode() {
        return HashCode.Combine(Date.ToString("yyyyMMddHHmmss"), Competitors[0].Team.Abbreviation, Competitors[1].Team.Abbreviation);
    }

}

public partial class Broadcast {
    [JsonPropertyName("market")]
    public MarketEnum Market { get; set; }

    [JsonPropertyName("names")]
    public string[] Names { get; set; }
}

public partial class Competitor {

    [JsonPropertyName("homeAway")]
    public HomeAway HomeAway { get; set; }

    [JsonPropertyName("team")]
    public Team Team { get; set; }

    [JsonPropertyName("score")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Score { get; set; }
}

public partial class CompetitorLeader {
    [JsonPropertyName("name")]
    public LeaderName Name { get; set; }

    [JsonPropertyName("displayName")]
    public DisplayName DisplayName { get; set; }

    [JsonPropertyName("shortDisplayName")]
    public ShortDisplayName ShortDisplayName { get; set; }

    [JsonPropertyName("abbreviation")]
    public LeaderAbbreviation Abbreviation { get; set; }

    [JsonPropertyName("leaders")]
    public LeaderLeader[] Leaders { get; set; }
}

public partial class LeaderLeader {
    [JsonPropertyName("displayValue")]
    public string DisplayValue { get; set; }

    [JsonPropertyName("value")]
    public double Value { get; set; }

    [JsonPropertyName("athlete")]
    public Athlete Athlete { get; set; }

    [JsonPropertyName("team")]
    public TeamClass Team { get; set; }
}

public partial class Athlete {
    [JsonPropertyName("id")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }

    [JsonPropertyName("fullName")]
    public string FullName { get; set; }

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }

    [JsonPropertyName("shortName")]
    public string ShortName { get; set; }

    [JsonPropertyName("links")]
    public AthleteLink[] Links { get; set; }

    [JsonPropertyName("headshot")]
    public Uri Headshot { get; set; }

    [JsonPropertyName("jersey")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Jersey { get; set; }

    [JsonPropertyName("position")]
    public Position Position { get; set; }

    [JsonPropertyName("team")]
    public TeamClass Team { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }
}

public partial class AthleteLink {
    [JsonPropertyName("rel")]
    public RelEnum[] Rel { get; set; }

    [JsonPropertyName("href")]
    public Uri Href { get; set; }
}

public partial class Position {
    [JsonPropertyName("abbreviation")]
    public string Abbreviation { get; set; }
}

public partial class TeamClass {
    [JsonPropertyName("id")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }
}

public partial class Linescore {
    [JsonPropertyName("value")]
    public double Value { get; set; }
}

public partial class Record {
    [JsonPropertyName("name")]
    public RecordName Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("abbreviation")]
    public RecordAbbreviation? Abbreviation { get; set; }

    [JsonPropertyName("type")]
    public RecordType Type { get; set; }

    [JsonPropertyName("summary")]
    public string Summary { get; set; }
}

public partial class Team {

    [JsonPropertyName("abbreviation")]
    public string Abbreviation { get; set; }

    //[JsonPropertyName("links")]
    //public TeamsOnByeLink[] Links { get; set; }

    [JsonPropertyName("logo")]
    public Uri Logo { get; set; }
}

public partial class TeamsOnByeLink {
    [JsonPropertyName("rel")]
    public RecordTypeDetail[] Rel { get; set; }

    [JsonPropertyName("href")]
    public Uri Href { get; set; }

    [JsonPropertyName("text")]
    public Text Text { get; set; }

    [JsonPropertyName("isExternal")]
    public bool IsExternal { get; set; }

    [JsonPropertyName("isPremium")]
    public bool IsPremium { get; set; }
}

public partial class Format {
    [JsonPropertyName("regulation")]
    public Regulation Regulation { get; set; }
}

public partial class Regulation {
    [JsonPropertyName("periods")]
    public long Periods { get; set; }
}

public partial class GeoBroadcast {
    [JsonPropertyName("type")]
    public GeoBroadcastType Type { get; set; }

    [JsonPropertyName("market")]
    public MarketClass Market { get; set; }

    [JsonPropertyName("media")]
    public Media Media { get; set; }

    [JsonPropertyName("lang")]
    public Lang Lang { get; set; }

    [JsonPropertyName("region")]
    public Region Region { get; set; }
}

public partial class MarketClass {
    [JsonPropertyName("id")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }

    [JsonPropertyName("type")]
    public MarketType Type { get; set; }
}

public partial class Media {
    [JsonPropertyName("shortName")]
    public string ShortName { get; set; }
}

public partial class GeoBroadcastType {
    [JsonPropertyName("id")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }

    [JsonPropertyName("shortName")]
    public string ShortName { get; set; }
}

public partial class Headline {
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("type")]
    public ShortText Type { get; set; }

    [JsonPropertyName("shortLinkText")]
    public string ShortLinkText { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("video")]
    public Video[] Video { get; set; }
}

public partial class Video {
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("headline")]
    public string Headline { get; set; }

    [JsonPropertyName("thumbnail")]
    public Uri Thumbnail { get; set; }

    [JsonPropertyName("duration")]
    public long Duration { get; set; }

    [JsonPropertyName("tracking")]
    public Tracking Tracking { get; set; }

    [JsonPropertyName("deviceRestrictions")]
    public DeviceRestrictions DeviceRestrictions { get; set; }

    [JsonPropertyName("links")]
    public Links Links { get; set; }
}

public partial class DeviceRestrictions {
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("devices")]
    public Device[] Devices { get; set; }
}

public partial class Links {
    [JsonPropertyName("api")]
    public Api Api { get; set; }

    [JsonPropertyName("web")]
    public Web Web { get; set; }

    [JsonPropertyName("source")]
    public ESPNSource Source { get; set; }

    [JsonPropertyName("mobile")]
    public Mobile Mobile { get; set; }
}

public partial class Api {
    [JsonPropertyName("self")]
    public ArtworkElement Self { get; set; }

    [JsonPropertyName("artwork")]
    public ArtworkElement Artwork { get; set; }
}

public partial class ArtworkElement {
    [JsonPropertyName("href")]
    public Uri Href { get; set; }
}

public partial class Mobile {
    [JsonPropertyName("alert")]
    public ArtworkElement Alert { get; set; }

    [JsonPropertyName("source")]
    public ArtworkElement Source { get; set; }

    [JsonPropertyName("href")]
    public Uri Href { get; set; }

    [JsonPropertyName("streaming")]
    public ArtworkElement Streaming { get; set; }

    [JsonPropertyName("progressiveDownload")]
    public ArtworkElement ProgressiveDownload { get; set; }
}

public partial class ESPNSource {
    [JsonPropertyName("mezzanine")]
    public ArtworkElement Mezzanine { get; set; }

    [JsonPropertyName("flash")]
    public ArtworkElement Flash { get; set; }

    [JsonPropertyName("hds")]
    public ArtworkElement Hds { get; set; }

    [JsonPropertyName("HLS")]
    public Hls Hls { get; set; }

    [JsonPropertyName("HD")]
    public ArtworkElement Hd { get; set; }

    [JsonPropertyName("full")]
    public ArtworkElement Full { get; set; }

    [JsonPropertyName("href")]
    public Uri Href { get; set; }
}

public partial class Hls {
    [JsonPropertyName("href")]
    public Uri Href { get; set; }

    [JsonPropertyName("HD")]
    public ArtworkElement Hd { get; set; }
}

public partial class Web {
    [JsonPropertyName("href")]
    public Uri Href { get; set; }

    [JsonPropertyName("short")]
    public ArtworkElement Short { get; set; }

    [JsonPropertyName("self")]
    public ArtworkElement Self { get; set; }
}

public partial class Tracking {
    [JsonPropertyName("sportName")]
    public string SportName { get; set; }

    [JsonPropertyName("leagueName")]
    public string LeagueName { get; set; }

    [JsonPropertyName("coverageType")]
    public string CoverageType { get; set; }

    [JsonPropertyName("trackingName")]
    public string TrackingName { get; set; }

    [JsonPropertyName("trackingId")]
    public string TrackingId { get; set; }
}

public partial class Note {
    [JsonPropertyName("type")]
    public NoteType Type { get; set; }

    [JsonPropertyName("headline")]
    public string Headline { get; set; }
}

public partial class Odd {
    [JsonPropertyName("provider")]
    public Provider Provider { get; set; }

    [JsonPropertyName("details")]
    public string Details { get; set; }

    [JsonPropertyName("overUnder")]
    public double OverUnder { get; set; }
}

public partial class Provider {
    [JsonPropertyName("id")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("priority")]
    public long Priority { get; set; }
}

public partial class Situation {
    [JsonPropertyName("$ref")]
    public Uri Ref { get; set; }

    [JsonPropertyName("lastPlay")]
    public LastPlay LastPlay { get; set; }

    [JsonPropertyName("down")]
    public long Down { get; set; }

    [JsonPropertyName("yardLine")]
    public long YardLine { get; set; }

    [JsonPropertyName("distance")]
    public long Distance { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("downDistanceText")]
    public string DownDistanceText { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("shortDownDistanceText")]
    public string ShortDownDistanceText { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("possessionText")]
    public string PossessionText { get; set; }

    [JsonPropertyName("isRedZone")]
    public bool IsRedZone { get; set; }

    [JsonPropertyName("homeTimeouts")]
    public long HomeTimeouts { get; set; }

    [JsonPropertyName("awayTimeouts")]
    public long AwayTimeouts { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("possession")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long? Possession { get; set; }
}

public partial class LastPlay {
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("type")]
    public LastPlayType Type { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("scoreValue")]
    public long ScoreValue { get; set; }

    [JsonPropertyName("team")]
    public TeamClass Team { get; set; }

    [JsonPropertyName("probability")]
    public Probability Probability { get; set; }

    [JsonPropertyName("drive")]
    public Drive Drive { get; set; }

    [JsonPropertyName("start")]
    public LastPlayEnd Start { get; set; }

    [JsonPropertyName("end")]
    public LastPlayEnd End { get; set; }

    [JsonPropertyName("statYardage")]
    public long StatYardage { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("athletesInvolved")]
    public AthletesInvolved[] AthletesInvolved { get; set; }
}

public partial class AthletesInvolved {
    [JsonPropertyName("id")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }

    [JsonPropertyName("fullName")]
    public string FullName { get; set; }

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }

    [JsonPropertyName("shortName")]
    public string ShortName { get; set; }

    [JsonPropertyName("links")]
    public AthleteLink[] Links { get; set; }

    [JsonPropertyName("headshot")]
    public Uri Headshot { get; set; }

    [JsonPropertyName("jersey")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Jersey { get; set; }

    [JsonPropertyName("position")]
    public string Position { get; set; }

    [JsonPropertyName("team")]
    public TeamClass Team { get; set; }
}

public partial class Drive {
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("start")]
    public DriveEnd Start { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("end")]
    public DriveEnd End { get; set; }

    [JsonPropertyName("timeElapsed")]
    public TimeElapsed TimeElapsed { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("result")]
    public string Result { get; set; }
}

public partial class DriveEnd {
    [JsonPropertyName("yardLine")]
    public long YardLine { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }
}

public partial class TimeElapsed {
    [JsonPropertyName("displayValue")]
    public string DisplayValue { get; set; }
}

public partial class LastPlayEnd {
    [JsonPropertyName("yardLine")]
    public long YardLine { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("team")]
    public TeamClass Team { get; set; }
}

public partial class Probability {
    [JsonPropertyName("tiePercentage")]
    public double TiePercentage { get; set; }

    [JsonPropertyName("homeWinPercentage")]
    public double HomeWinPercentage { get; set; }

    [JsonPropertyName("awayWinPercentage")]
    public double AwayWinPercentage { get; set; }

    [JsonPropertyName("secondsLeft")]
    public double SecondsLeft { get; set; }
}

public partial class LastPlayType {
    [JsonPropertyName("id")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("abbreviation")]
    public string Abbreviation { get; set; }
}

public partial class ESPNStatus {
    [JsonPropertyName("clock")]
    public double Clock { get; set; }

    [JsonPropertyName("displayClock")]
    public string DisplayClock { get; set; }

    [JsonPropertyName("period")]
    public long Period { get; set; }

    [JsonPropertyName("type")]
    public StatusType Type { get; set; }
}

public partial class StatusType {
    [JsonPropertyName("id")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public TypeName Name { get; set; }

    [JsonPropertyName("state")]
    public State State { get; set; }

    [JsonPropertyName("completed")]
    public bool Completed { get; set; }

    [JsonPropertyName("description")]
    public Description Description { get; set; }

    [JsonPropertyName("detail")]
    public string Detail { get; set; }

    [JsonPropertyName("shortDetail")]
    public string ShortDetail { get; set; }
}

public partial class Ticket {
    [JsonPropertyName("summary")]
    public string Summary { get; set; }

    [JsonPropertyName("numberAvailable")]
    public long NumberAvailable { get; set; }

    [JsonPropertyName("links")]
    public ArtworkElement[] Links { get; set; }
}

public partial class CompetitionType {
    [JsonPropertyName("id")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }

    [JsonPropertyName("abbreviation")]
    public TypeAbbreviation Abbreviation { get; set; }
}

public partial class CompetitionVenue {
    [JsonPropertyName("id")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }

    [JsonPropertyName("fullName")]
    public string FullName { get; set; }

    [JsonPropertyName("address")]
    public Address Address { get; set; }

    [JsonPropertyName("capacity")]
    public long Capacity { get; set; }

    [JsonPropertyName("indoor")]
    public bool Indoor { get; set; }
}

public partial class Address {
    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("state")]
    public string State { get; set; }
}

public partial class EventLink {
    [JsonPropertyName("language")]
    public Language Language { get; set; }

    [JsonPropertyName("rel")]
    public RelUnion[] Rel { get; set; }

    [JsonPropertyName("href")]
    public Uri Href { get; set; }

    [JsonPropertyName("text")]
    public ShortText Text { get; set; }

    [JsonPropertyName("shortText")]
    public ShortText ShortText { get; set; }

    [JsonPropertyName("isExternal")]
    public bool IsExternal { get; set; }

    [JsonPropertyName("isPremium")]
    public bool IsPremium { get; set; }
}

public partial class EventSeason {
    [JsonPropertyName("year")]
    public long Year { get; set; }

    [JsonPropertyName("type")]
    public long Type { get; set; }

    [JsonPropertyName("slug")]
    public TypeOfSeason Slug { get; set; }
}

public partial class Weather {
    [JsonPropertyName("displayValue")]
    public string DisplayValue { get; set; }

    [JsonPropertyName("temperature")]
    public long Temperature { get; set; }

    [JsonPropertyName("highTemperature")]
    public long HighTemperature { get; set; }

    [JsonPropertyName("conditionId")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long ConditionId { get; set; }

    [JsonPropertyName("link")]
    public EventLink Link { get; set; }
}

public partial class EventWeek {
    [JsonPropertyName("number")]
    public long Number { get; set; }
}

public partial class ESPNLeague {
    [JsonPropertyName("id")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }

    [JsonPropertyName("uid")]
    public string Uid { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("abbreviation")]
    public string Abbreviation { get; set; }

    [JsonPropertyName("slug")]
    public string Slug { get; set; }

    [JsonPropertyName("season")]
    public LeagueSeason Season { get; set; }

    [JsonPropertyName("logos")]
    public Logo[] Logos { get; set; }

    [JsonPropertyName("calendarType")]
    public string CalendarType { get; set; }

    [JsonPropertyName("calendarIsWhitelist")]
    public bool CalendarIsWhitelist { get; set; }

    [JsonPropertyName("calendarStartDate")]
    public string CalendarStartDate { get; set; }

    [JsonPropertyName("calendarEndDate")]
    public string CalendarEndDate { get; set; }

    [JsonPropertyName("calendar")]
    public Calendar[] Calendar { get; set; }
}

public partial class Calendar {
    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("value")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Value { get; set; }

    [JsonPropertyName("startDate")]
    public string StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public string EndDate { get; set; }

    //[JsonPropertyName("entries")]
    //public Entry[] Entries { get; set; }
}

public partial class Entry {
    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("alternateLabel")]
    public string AlternateLabel { get; set; }

    [JsonPropertyName("detail")]
    public string Detail { get; set; }

    [JsonPropertyName("value")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Value { get; set; }

    [JsonPropertyName("startDate")]
    public string StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public string EndDate { get; set; }
}

public partial class Logo {
    [JsonPropertyName("href")]
    public Uri Href { get; set; }

    [JsonPropertyName("width")]
    public long Width { get; set; }

    [JsonPropertyName("height")]
    public long Height { get; set; }

    [JsonPropertyName("alt")]
    public string Alt { get; set; }

    [JsonPropertyName("rel")]
    public string[] Rel { get; set; }

    [JsonPropertyName("lastUpdated")]
    public string LastUpdated { get; set; }
}

public partial class LeagueSeason {
    [JsonPropertyName("year")]
    public long Year { get; set; }

    [JsonPropertyName("startDate")]
    public string StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public string EndDate { get; set; }

    [JsonPropertyName("displayName")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long DisplayName { get; set; }

    [JsonPropertyName("type")]
    public SeasonType Type { get; set; }
}

public partial class SeasonType {
    [JsonPropertyName("id")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }

    [JsonPropertyName("type")]
    public long Type { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("abbreviation")]
    public string Abbreviation { get; set; }
}

public partial class Season {
    [JsonPropertyName("type")]
    public long Type { get; set; }

    [JsonPropertyName("year")]
    public long Year { get; set; }
}

public partial class Week {
    [JsonPropertyName("number")]
    public long Number { get; set; }

    [JsonPropertyName("teamsOnBye")]
    public Team[] TeamsOnBye { get; set; }
}

public enum MarketEnum { National, Away, Home };

public enum HomeAway { Away, Home };

public enum LeaderAbbreviation { Pyds, Recyds, Ryds };

public enum DisplayName { PassingLeader, ReceivingLeader, RushingLeader };

public enum RelEnum { Athlete, Desktop, Playercard };

//public enum Tion { Qb, Rb, Te, Wr };

public enum LeaderName { PassingLeader, PassingYards, ReceivingLeader, ReceivingYards, RushingLeader, RushingYards };

public enum ShortDisplayName { Pass, Rec, Rush };

public enum RecordAbbreviation { Any, Game };

public enum RecordName { Home, Overall, Road };

public enum RecordType { Home, Road, Total };

public enum RecordTypeDetail { Clubhouse, Desktop, Roster, Schedule, Stats, Team };

public enum Text { Clubhouse, Roster, Schedule, Statistics };

public enum Lang { En };

public enum MarketType { National, Home, Away };

public enum Region { Us };

public enum ShortName { Tv, Web };

public enum ShortText { BoxScore, Gamecast, Highlights, PlayByPlay, Recap, Weather };

public enum Device { Desktop, Handset, Settop, Tablet };

public enum NoteType { Boxscore, Desktop, Event, Highlights, Live, Pbp, Recap, Summary, The07073 };

//public enum DisplayClock { The000, The1033, The1308, The1500, The355 };

public enum Description { Final, Halftime, InProgress, Scheduled, EndOfPeriod };

public enum TypeName { StatusFinal, StatusHalftime, StatusInProgress, StatusScheduled, StatusEndPeriod };

public enum State { In, Post, Pre };

public enum TypeAbbreviation { Std };

public enum Language { EnUs };

public enum TypeOfSeason { PostSeason = 3, RegularSeason = 2, PreSeason = 1 };

public partial struct RelUnion {
    public NoteType? Enum;
    public long? Integer;

    public static implicit operator RelUnion(NoteType Enum) => new() { Enum = Enum };
    public static implicit operator RelUnion(long Integer) => new() { Integer = Integer };
}

internal static class ESPNApiServiceJsonConverter {
    public static readonly JsonSerializerOptions Settings = new(JsonSerializerDefaults.General) {
        Converters =
            {
                MarketEnumConverter.Singleton,
                HomeAwayConverter.Singleton,
                LeaderAbbreviationConverter.Singleton,
                DisplayNameConverter.Singleton,
                RelEnumConverter.Singleton,
                //TionConverter.Singleton,
                LeaderNameConverter.Singleton,
                ShortDisplayNameConverter.Singleton,
                RecordAbbreviationConverter.Singleton,
                RecordNameConverter.Singleton,
                RecordTypeConverter.Singleton,
                RecordTypeDetailConverter.Singleton,
                TextConverter.Singleton,
                LangConverter.Singleton,
                MarketTypeConverter.Singleton,
                RegionConverter.Singleton,
                ShortNameConverter.Singleton,
                ShortTextConverter.Singleton,
                DeviceConverter.Singleton,
                NoteTypeConverter.Singleton,
                //DisplayClockConverter.Singleton,
                DescriptionConverter.Singleton,
                TypeNameConverter.Singleton,
                StateConverter.Singleton,
                TypeAbbreviationConverter.Singleton,
                LanguageConverter.Singleton,
                RelUnionConverter.Singleton,
                TypeOfSeasonConverter.Singleton,
                new DateOnlyConverter(),
                new TimeOnlyConverter(),
                IsoDateTimeOffsetConverter.Singleton,
                new DateTimeConverterUsingDateTimeParse()
            },
    };
}

internal class MarketEnumConverter : JsonConverter<MarketEnum> {
    public override bool CanConvert(Type t) => t == typeof(MarketEnum);

    public override MarketEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        if (value == "national") {
            return MarketEnum.National;
        }
        if (value == "away") {
            return MarketEnum.Away;
        }
        else return MarketEnum.Home;
    }

    public override void Write(Utf8JsonWriter writer, MarketEnum value, JsonSerializerOptions options) {
        if (value == MarketEnum.National) {
            JsonSerializer.Serialize(writer, "national", options);
            return;
        }
        return;
    }

    public static readonly MarketEnumConverter Singleton = new();
}

internal class HomeAwayConverter : JsonConverter<HomeAway> {
    public override bool CanConvert(Type t) => t == typeof(HomeAway);

    public override HomeAway Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "away":
                return HomeAway.Away;
            case "home":
                return HomeAway.Home;
        }
        throw new Exception("Cannot unmarshal type HomeAway");
    }

    public override void Write(Utf8JsonWriter writer, HomeAway value, JsonSerializerOptions options) {
        switch (value) {
            case HomeAway.Away:
                JsonSerializer.Serialize(writer, "away", options);
                return;
            case HomeAway.Home:
                JsonSerializer.Serialize(writer, "home", options);
                return;
        }
        return;
    }

    public static readonly HomeAwayConverter Singleton = new();
}

internal class ParseStringConverter : JsonConverter<long> {
    public override bool CanConvert(Type t) => t == typeof(long);

    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        long l;
        if (Int64.TryParse(value, out l)) {
            return l;
        }
        throw new Exception("Cannot unmarshal type long");
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options) {
        JsonSerializer.Serialize(writer, value.ToString(), options);
        return;
    }

    public static readonly ParseStringConverter Singleton = new();
}

internal class LeaderAbbreviationConverter : JsonConverter<LeaderAbbreviation> {
    public override bool CanConvert(Type t) => t == typeof(LeaderAbbreviation);

    public override LeaderAbbreviation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "PYDS":
                return LeaderAbbreviation.Pyds;
            case "RECYDS":
                return LeaderAbbreviation.Recyds;
            case "RYDS":
                return LeaderAbbreviation.Ryds;
        }
        return LeaderAbbreviation.Pyds;
    }

    public override void Write(Utf8JsonWriter writer, LeaderAbbreviation value, JsonSerializerOptions options) {
        switch (value) {
            case LeaderAbbreviation.Pyds:
                JsonSerializer.Serialize(writer, "PYDS", options);
                return;
            case LeaderAbbreviation.Recyds:
                JsonSerializer.Serialize(writer, "RECYDS", options);
                return;
            case LeaderAbbreviation.Ryds:
                JsonSerializer.Serialize(writer, "RYDS", options);
                return;
        }
        return;
    }

    public static readonly LeaderAbbreviationConverter Singleton = new();
}

internal class DisplayNameConverter : JsonConverter<DisplayName> {
    public override bool CanConvert(Type t) => t == typeof(DisplayName);

    public override DisplayName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "Passing Leader":
                return DisplayName.PassingLeader;
            case "Receiving Leader":
                return DisplayName.ReceivingLeader;
            case "Rushing Leader":
                return DisplayName.RushingLeader;
        }
        throw new Exception("Cannot unmarshal type DisplayName");
    }

    public override void Write(Utf8JsonWriter writer, DisplayName value, JsonSerializerOptions options) {
        switch (value) {
            case DisplayName.PassingLeader:
                JsonSerializer.Serialize(writer, "Passing Leader", options);
                return;
            case DisplayName.ReceivingLeader:
                JsonSerializer.Serialize(writer, "Receiving Leader", options);
                return;
            case DisplayName.RushingLeader:
                JsonSerializer.Serialize(writer, "Rushing Leader", options);
                return;
        }
        return;
    }

    public static readonly DisplayNameConverter Singleton = new();
}

internal class RelEnumConverter : JsonConverter<RelEnum> {
    public override bool CanConvert(Type t) => t == typeof(RelEnum);

    public override RelEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "athlete":
                return RelEnum.Athlete;
            case "desktop":
                return RelEnum.Desktop;
            case "playercard":
                return RelEnum.Playercard;
        }
        throw new Exception("Cannot unmarshal type RelEnum");
    }

    public override void Write(Utf8JsonWriter writer, RelEnum value, JsonSerializerOptions options) {
        switch (value) {
            case RelEnum.Athlete:
                JsonSerializer.Serialize(writer, "athlete", options);
                return;
            case RelEnum.Desktop:
                JsonSerializer.Serialize(writer, "desktop", options);
                return;
            case RelEnum.Playercard:
                JsonSerializer.Serialize(writer, "playercard", options);
                return;
        }
        return;
    }

    public static readonly RelEnumConverter Singleton = new();
}
/*
internal class TionConverter : JsonConverter<Tion>
{
    public override bool CanConvert(Type t) => t == typeof(Tion);

    public override Tion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        switch (value)
        {
            case "QB":
                return Tion.Qb;
            case "RB":
                return Tion.Rb;
            case "TE":
                return Tion.Te;
            case "WR":
                return Tion.Wr;
        }
        throw new Exception("Cannot unmarshal type Tion");
    }

    public override void Write(Utf8JsonWriter writer, Tion value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case Tion.Qb:
                JsonSerializer.Serialize(writer, "QB", options);
                return;
            case Tion.Rb:
                JsonSerializer.Serialize(writer, "RB", options);
                return;
            case Tion.Te:
                JsonSerializer.Serialize(writer, "TE", options);
                return;
            case Tion.Wr:
                JsonSerializer.Serialize(writer, "WR", options);
                return;
        }
        throw new Exception("Cannot marshal type Tion");
    }

    public static readonly TionConverter Singleton = new TionConverter();
}
*/
internal class LeaderNameConverter : JsonConverter<LeaderName> {
    public override bool CanConvert(Type t) => t == typeof(LeaderName);

    public override LeaderName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "passingLeader":
                return LeaderName.PassingLeader;
            case "passingYards":
                return LeaderName.PassingYards;
            case "receivingLeader":
                return LeaderName.ReceivingLeader;
            case "receivingYards":
                return LeaderName.ReceivingYards;
            case "rushingLeader":
                return LeaderName.RushingLeader;
            case "rushingYards":
                return LeaderName.RushingYards;
        }
        throw new Exception("Cannot unmarshal type LeaderName");
    }

    public override void Write(Utf8JsonWriter writer, LeaderName value, JsonSerializerOptions options) {
        switch (value) {
            case LeaderName.PassingLeader:
                JsonSerializer.Serialize(writer, "passingLeader", options);
                return;
            case LeaderName.PassingYards:
                JsonSerializer.Serialize(writer, "passingYards", options);
                return;
            case LeaderName.ReceivingLeader:
                JsonSerializer.Serialize(writer, "receivingLeader", options);
                return;
            case LeaderName.ReceivingYards:
                JsonSerializer.Serialize(writer, "receivingYards", options);
                return;
            case LeaderName.RushingLeader:
                JsonSerializer.Serialize(writer, "rushingLeader", options);
                return;
            case LeaderName.RushingYards:
                JsonSerializer.Serialize(writer, "rushingYards", options);
                return;
        }
        return;
    }

    public static readonly LeaderNameConverter Singleton = new();
}

internal class ShortDisplayNameConverter : JsonConverter<ShortDisplayName> {
    public override bool CanConvert(Type t) => t == typeof(ShortDisplayName);

    public override ShortDisplayName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "PASS":
                return ShortDisplayName.Pass;
            case "REC":
                return ShortDisplayName.Rec;
            case "RUSH":
                return ShortDisplayName.Rush;
        }
        throw new Exception("Cannot unmarshal type ShortDisplayName");
    }

    public override void Write(Utf8JsonWriter writer, ShortDisplayName value, JsonSerializerOptions options) {
        switch (value) {
            case ShortDisplayName.Pass:
                JsonSerializer.Serialize(writer, "PASS", options);
                return;
            case ShortDisplayName.Rec:
                JsonSerializer.Serialize(writer, "REC", options);
                return;
            case ShortDisplayName.Rush:
                JsonSerializer.Serialize(writer, "RUSH", options);
                return;
        }
        return;
    }

    public static readonly ShortDisplayNameConverter Singleton = new();
}

internal class RecordAbbreviationConverter : JsonConverter<RecordAbbreviation> {
    public override bool CanConvert(Type t) => t == typeof(RecordAbbreviation);

    public override RecordAbbreviation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "Any":
                return RecordAbbreviation.Any;
            case "Game":
                return RecordAbbreviation.Game;
        }
        return RecordAbbreviation.Any;
    }

    public override void Write(Utf8JsonWriter writer, RecordAbbreviation value, JsonSerializerOptions options) {
        switch (value) {
            case RecordAbbreviation.Any:
                JsonSerializer.Serialize(writer, "Any", options);
                return;
            case RecordAbbreviation.Game:
                JsonSerializer.Serialize(writer, "Game", options);
                return;
        }
        return;
    }

    public static readonly RecordAbbreviationConverter Singleton = new();
}

internal class RecordNameConverter : JsonConverter<RecordName> {
    public override bool CanConvert(Type t) => t == typeof(RecordName);

    public override RecordName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "Home":
                return RecordName.Home;
            case "Road":
                return RecordName.Road;
            case "overall":
                return RecordName.Overall;
        }
        return RecordName.Home;
    }

    public override void Write(Utf8JsonWriter writer, RecordName value, JsonSerializerOptions options) {
        switch (value) {
            case RecordName.Home:
                JsonSerializer.Serialize(writer, "Home", options);
                return;
            case RecordName.Road:
                JsonSerializer.Serialize(writer, "Road", options);
                return;
            case RecordName.Overall:
                JsonSerializer.Serialize(writer, "overall", options);
                return;
        }
        return;
    }

    public static readonly RecordNameConverter Singleton = new();
}

internal class RecordTypeConverter : JsonConverter<RecordType> {
    public override bool CanConvert(Type t) => t == typeof(RecordType);

    public override RecordType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "home":
                return RecordType.Home;
            case "road":
                return RecordType.Road;
            case "total":
                return RecordType.Total;
        }
        return RecordType.Home;
    }

    public override void Write(Utf8JsonWriter writer, RecordType value, JsonSerializerOptions options) {
        switch (value) {
            case RecordType.Home:
                JsonSerializer.Serialize(writer, "home", options);
                return;
            case RecordType.Road:
                JsonSerializer.Serialize(writer, "road", options);
                return;
            case RecordType.Total:
                JsonSerializer.Serialize(writer, "total", options);
                return;
        }
        return;
    }

    public static readonly RecordTypeConverter Singleton = new();
}

internal class RecordTypeDetailConverter : JsonConverter<RecordTypeDetail> {
    public override bool CanConvert(Type t) => t == typeof(RecordTypeDetail);

    public override RecordTypeDetail Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "clubhouse":
                return RecordTypeDetail.Clubhouse;
            case "desktop":
                return RecordTypeDetail.Desktop;
            case "roster":
                return RecordTypeDetail.Roster;
            case "schedule":
                return RecordTypeDetail.Schedule;
            case "stats":
                return RecordTypeDetail.Stats;
            case "team":
                return RecordTypeDetail.Team;
        }
        throw new Exception("Cannot unmarshal type TypRecordTypeDetaileElement");
    }

    public override void Write(Utf8JsonWriter writer, RecordTypeDetail value, JsonSerializerOptions options) {
        switch (value) {
            case RecordTypeDetail.Clubhouse:
                JsonSerializer.Serialize(writer, "clubhouse", options);
                return;
            case RecordTypeDetail.Desktop:
                JsonSerializer.Serialize(writer, "desktop", options);
                return;
            case RecordTypeDetail.Roster:
                JsonSerializer.Serialize(writer, "roster", options);
                return;
            case RecordTypeDetail.Schedule:
                JsonSerializer.Serialize(writer, "schedule", options);
                return;
            case RecordTypeDetail.Stats:
                JsonSerializer.Serialize(writer, "stats", options);
                return;
            case RecordTypeDetail.Team:
                JsonSerializer.Serialize(writer, "team", options);
                return;
        }
        return;
    }

    public static readonly RecordTypeDetailConverter Singleton = new();
}

internal class TextConverter : JsonConverter<Text> {
    public override bool CanConvert(Type t) => t == typeof(Text);

    public override Text Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "Clubhouse":
                return Text.Clubhouse;
            case "Roster":
                return Text.Roster;
            case "Schedule":
                return Text.Schedule;
            case "Statistics":
                return Text.Statistics;
        }
        return Text.Clubhouse;
    }

    public override void Write(Utf8JsonWriter writer, Text value, JsonSerializerOptions options) {
        switch (value) {
            case Text.Clubhouse:
                JsonSerializer.Serialize(writer, "Clubhouse", options);
                return;
            case Text.Roster:
                JsonSerializer.Serialize(writer, "Roster", options);
                return;
            case Text.Schedule:
                JsonSerializer.Serialize(writer, "Schedule", options);
                return;
            case Text.Statistics:
                JsonSerializer.Serialize(writer, "Statistics", options);
                return;
        }
        return;
    }

    public static readonly TextConverter Singleton = new();
}

internal class LangConverter : JsonConverter<Lang> {
    public override bool CanConvert(Type t) => t == typeof(Lang);

    public override Lang Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        if (value == "en") {
            return Lang.En;
        }
        return Lang.En;
    }

    public override void Write(Utf8JsonWriter writer, Lang value, JsonSerializerOptions options) {
        if (value == Lang.En) {
            JsonSerializer.Serialize(writer, "en", options);
            return;
        }
        return;
    }

    public static readonly LangConverter Singleton = new();
}

internal class MarketTypeConverter : JsonConverter<MarketType> {
    public override bool CanConvert(Type t) => t == typeof(MarketType);

    public override MarketType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        if (value == "National") {
            return MarketType.National;
        }
        if (value == "Away") {
            return MarketType.Away;
        }
        return MarketType.Home;
    }

    public override void Write(Utf8JsonWriter writer, MarketType value, JsonSerializerOptions options) {
        if (value == MarketType.National) {
            JsonSerializer.Serialize(writer, "National", options);
            return;
        }
        return;
    }

    public static readonly MarketTypeConverter Singleton = new();
}

internal class RegionConverter : JsonConverter<Region> {
    public override bool CanConvert(Type t) => t == typeof(Region);

    public override Region Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        if (value == "us") {
            return Region.Us;
        }
        return Region.Us;
    }

    public override void Write(Utf8JsonWriter writer, Region value, JsonSerializerOptions options) {
        if (value == Region.Us) {
            JsonSerializer.Serialize(writer, "us", options);
            return;
        }
        return;
    }

    public static readonly RegionConverter Singleton = new();
}

internal class ShortNameConverter : JsonConverter<ShortName> {
    public override bool CanConvert(Type t) => t == typeof(ShortName);

    public override ShortName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "TV":
                return ShortName.Tv;
            case "Web":
                return ShortName.Web;
        }
        return ShortName.Tv;
    }

    public override void Write(Utf8JsonWriter writer, ShortName value, JsonSerializerOptions options) {
        switch (value) {
            case ShortName.Tv:
                JsonSerializer.Serialize(writer, "TV", options);
                return;
            case ShortName.Web:
                JsonSerializer.Serialize(writer, "Web", options);
                return;
        }
        return;
    }

    public static readonly ShortNameConverter Singleton = new();
}

internal class ShortTextConverter : JsonConverter<ShortText> {
    public override bool CanConvert(Type t) => t == typeof(ShortText);

    public override ShortText Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "Box Score":
                return ShortText.BoxScore;
            case "Gamecast":
                return ShortText.Gamecast;
            case "Highlights":
                return ShortText.Highlights;
            case "Play-by-Play":
                return ShortText.PlayByPlay;
            case "Recap":
                return ShortText.Recap;
            case "Weather":
                return ShortText.Weather;
        }
        throw new Exception("Cannot unmarshal type ShortText");
    }

    public override void Write(Utf8JsonWriter writer, ShortText value, JsonSerializerOptions options) {
        switch (value) {
            case ShortText.BoxScore:
                JsonSerializer.Serialize(writer, "Box Score", options);
                return;
            case ShortText.Gamecast:
                JsonSerializer.Serialize(writer, "Gamecast", options);
                return;
            case ShortText.Highlights:
                JsonSerializer.Serialize(writer, "Highlights", options);
                return;
            case ShortText.PlayByPlay:
                JsonSerializer.Serialize(writer, "Play-by-Play", options);
                return;
            case ShortText.Recap:
                JsonSerializer.Serialize(writer, "Recap", options);
                return;
            case ShortText.Weather:
                JsonSerializer.Serialize(writer, "Weather", options);
                return;
        }
        return;
    }

    public static readonly ShortTextConverter Singleton = new();
}

internal class DeviceConverter : JsonConverter<Device> {
    public override bool CanConvert(Type t) => t == typeof(Device);

    public override Device Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "desktop":
                return Device.Desktop;
            case "handset":
                return Device.Handset;
            case "settop":
                return Device.Settop;
            case "tablet":
                return Device.Tablet;
        }
        return Device.Desktop;
    }

    public override void Write(Utf8JsonWriter writer, Device value, JsonSerializerOptions options) {
        switch (value) {
            case Device.Desktop:
                JsonSerializer.Serialize(writer, "desktop", options);
                return;
            case Device.Handset:
                JsonSerializer.Serialize(writer, "handset", options);
                return;
            case Device.Settop:
                JsonSerializer.Serialize(writer, "settop", options);
                return;
            case Device.Tablet:
                JsonSerializer.Serialize(writer, "tablet", options);
                return;
        }
        return;
    }

    public static readonly DeviceConverter Singleton = new();
}

internal class NoteTypeConverter : JsonConverter<NoteType> {
    public override bool CanConvert(Type t) => t == typeof(NoteType);

    public override NoteType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "07073":
                return NoteType.The07073;
            case "boxscore":
                return NoteType.Boxscore;
            case "desktop":
                return NoteType.Desktop;
            case "event":
                return NoteType.Event;
            case "highlights":
                return NoteType.Highlights;
            case "live":
                return NoteType.Live;
            case "pbp":
                return NoteType.Pbp;
            case "recap":
                return NoteType.Recap;
            case "summary":
                return NoteType.Summary;
        }
        throw new Exception("Cannot unmarshal type NoteType");
    }

    public override void Write(Utf8JsonWriter writer, NoteType value, JsonSerializerOptions options) {
        switch (value) {
            case NoteType.Boxscore:
                JsonSerializer.Serialize(writer, "boxscore", options);
                return;
            case NoteType.Desktop:
                JsonSerializer.Serialize(writer, "desktop", options);
                return;
            case NoteType.Event:
                JsonSerializer.Serialize(writer, "event", options);
                return;
            case NoteType.Highlights:
                JsonSerializer.Serialize(writer, "highlights", options);
                return;
            case NoteType.Live:
                JsonSerializer.Serialize(writer, "live", options);
                return;
            case NoteType.Pbp:
                JsonSerializer.Serialize(writer, "pbp", options);
                return;
            case NoteType.Recap:
                JsonSerializer.Serialize(writer, "recap", options);
                return;
            case NoteType.Summary:
                JsonSerializer.Serialize(writer, "summary", options);
                return;
        }
        return;
    }

    public static readonly NoteTypeConverter Singleton = new();
}
/*
internal class DisplayClockConverter : JsonConverter<DisplayClock>
{
    public override bool CanConvert(Type t) => t == typeof(DisplayClock);

    public override DisplayClock Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        switch (value)
        {
            case "0:00":
                return DisplayClock.The000;
            case "10:33":
                return DisplayClock.The1033;
            case "13:08":
                return DisplayClock.The1308;
            case "15:00":
                return DisplayClock.The1500;
            case "3:55":
                return DisplayClock.The355;
        }
        throw new Exception("Cannot unmarshal type DisplayClock");
    }

    public override void Write(Utf8JsonWriter writer, DisplayClock value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case DisplayClock.The000:
                JsonSerializer.Serialize(writer, "0:00", options);
                return;
            case DisplayClock.The1033:
                JsonSerializer.Serialize(writer, "10:33", options);
                return;
            case DisplayClock.The1308:
                JsonSerializer.Serialize(writer, "13:08", options);
                return;
            case DisplayClock.The1500:
                JsonSerializer.Serialize(writer, "15:00", options);
                return;
            case DisplayClock.The355:
                JsonSerializer.Serialize(writer, "3:55", options);
                return;
        }
        throw new Exception("Cannot marshal type DisplayClock");
    }

    public static readonly DisplayClockConverter Singleton = new DisplayClockConverter();
}
*/
internal class DescriptionConverter : JsonConverter<Description> {
    public override bool CanConvert(Type t) => t == typeof(Description);

    public override Description Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "Final":
                return Description.Final;
            case "Halftime":
                return Description.Halftime;
            case "In Progress":
                return Description.InProgress;
            case "Scheduled":
                return Description.Scheduled;
            case "End of Period":
                return Description.EndOfPeriod;
        }
        throw new Exception("Cannot unmarshal type Description");
    }

    public override void Write(Utf8JsonWriter writer, Description value, JsonSerializerOptions options) {
        switch (value) {
            case Description.Final:
                JsonSerializer.Serialize(writer, "Final", options);
                return;
            case Description.Halftime:
                JsonSerializer.Serialize(writer, "Halftime", options);
                return;
            case Description.InProgress:
                JsonSerializer.Serialize(writer, "In Progress", options);
                return;
            case Description.Scheduled:
                JsonSerializer.Serialize(writer, "Scheduled", options);
                return;
        }
        return;
    }

    public static readonly DescriptionConverter Singleton = new();
}

internal class TypeNameConverter : JsonConverter<TypeName> {
    public override bool CanConvert(Type t) => t == typeof(TypeName);

    public override TypeName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "STATUS_FINAL":
                return TypeName.StatusFinal;
            case "STATUS_HALFTIME":
                return TypeName.StatusHalftime;
            case "STATUS_IN_PROGRESS":
                return TypeName.StatusInProgress;
            case "STATUS_SCHEDULED":
                return TypeName.StatusScheduled;
            case "STATUS_END_PERIOD":
                return TypeName.StatusEndPeriod;
        }
        throw new Exception("Cannot unmarshal type TypeName");
    }

    public override void Write(Utf8JsonWriter writer, TypeName value, JsonSerializerOptions options) {
        switch (value) {
            case TypeName.StatusFinal:
                JsonSerializer.Serialize(writer, "STATUS_FINAL", options);
                return;
            case TypeName.StatusHalftime:
                JsonSerializer.Serialize(writer, "STATUS_HALFTIME", options);
                return;
            case TypeName.StatusInProgress:
                JsonSerializer.Serialize(writer, "STATUS_IN_PROGRESS", options);
                return;
            case TypeName.StatusScheduled:
                JsonSerializer.Serialize(writer, "STATUS_SCHEDULED", options);
                return;
        }
        return;
    }

    public static readonly TypeNameConverter Singleton = new();
}

internal class StateConverter : JsonConverter<State> {
    public override bool CanConvert(Type t) => t == typeof(State);

    public override State Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        switch (value) {
            case "in":
                return State.In;
            case "post":
                return State.Post;
            case "pre":
                return State.Pre;
        }
        throw new Exception("Cannot unmarshal type State");
    }

    public override void Write(Utf8JsonWriter writer, State value, JsonSerializerOptions options) {
        switch (value) {
            case State.In:
                JsonSerializer.Serialize(writer, "in", options);
                return;
            case State.Post:
                JsonSerializer.Serialize(writer, "post", options);
                return;
            case State.Pre:
                JsonSerializer.Serialize(writer, "pre", options);
                return;
        }
        return;
    }

    public static readonly StateConverter Singleton = new();
}

internal class TypeAbbreviationConverter : JsonConverter<TypeAbbreviation> {
    public override bool CanConvert(Type t) => t == typeof(TypeAbbreviation);

    public override TypeAbbreviation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        if (value == "STD") {
            return TypeAbbreviation.Std;
        }
        return TypeAbbreviation.Std;
    }

    public override void Write(Utf8JsonWriter writer, TypeAbbreviation value, JsonSerializerOptions options) {
        if (value == TypeAbbreviation.Std) {
            JsonSerializer.Serialize(writer, "STD", options);
            return;
        }
        return;
    }

    public static readonly TypeAbbreviationConverter Singleton = new();
}

internal class LanguageConverter : JsonConverter<Language> {
    public override bool CanConvert(Type t) => t == typeof(Language);

    public override Language Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        if (value == "en-US") {
            return Language.EnUs;
        }
        return Language.EnUs;
    }

    public override void Write(Utf8JsonWriter writer, Language value, JsonSerializerOptions options) {
        if (value == Language.EnUs) {
            JsonSerializer.Serialize(writer, "en-US", options);
            return;
        }
        return;
    }

    public static readonly LanguageConverter Singleton = new();
}

internal class RelUnionConverter : JsonConverter<RelUnion> {
    public override bool CanConvert(Type t) => t == typeof(RelUnion);

    public override RelUnion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        switch (reader.TokenType) {
            case JsonTokenType.String:
                var stringValue = reader.GetString();
                switch (stringValue) {
                    case "07073":
                        return new RelUnion { Enum = NoteType.The07073 };
                    case "boxscore":
                        return new RelUnion { Enum = NoteType.Boxscore };
                    case "desktop":
                        return new RelUnion { Enum = NoteType.Desktop };
                    case "event":
                        return new RelUnion { Enum = NoteType.Event };
                    case "highlights":
                        return new RelUnion { Enum = NoteType.Highlights };
                    case "live":
                        return new RelUnion { Enum = NoteType.Live };
                    case "pbp":
                        return new RelUnion { Enum = NoteType.Pbp };
                    case "recap":
                        return new RelUnion { Enum = NoteType.Recap };
                    case "summary":
                        return new RelUnion { Enum = NoteType.Summary };
                }
                long l;
                if (Int64.TryParse(stringValue, out l)) {
                    return new RelUnion { Integer = l };
                }
                break;
        }
        throw new Exception("Cannot unmarshal type RelUnion");
    }

    public override void Write(Utf8JsonWriter writer, RelUnion value, JsonSerializerOptions options) {
        if (value.Enum != null) {
            switch (value.Enum) {
                case NoteType.The07073:
                    JsonSerializer.Serialize(writer, "07073", options);
                    return;
                case NoteType.Boxscore:
                    JsonSerializer.Serialize(writer, "boxscore", options);
                    return;
                case NoteType.Desktop:
                    JsonSerializer.Serialize(writer, "desktop", options);
                    return;
                case NoteType.Event:
                    JsonSerializer.Serialize(writer, "event", options);
                    return;
                case NoteType.Highlights:
                    JsonSerializer.Serialize(writer, "highlights", options);
                    return;
                case NoteType.Live:
                    JsonSerializer.Serialize(writer, "live", options);
                    return;
                case NoteType.Pbp:
                    JsonSerializer.Serialize(writer, "pbp", options);
                    return;
                case NoteType.Recap:
                    JsonSerializer.Serialize(writer, "recap", options);
                    return;
                case NoteType.Summary:
                    JsonSerializer.Serialize(writer, "summary", options);
                    return;
            }
        }
        if (value.Integer != null) {
            JsonSerializer.Serialize(writer, value.Integer.Value.ToString(), options);
            return;
        }
        return;
    }

    public static readonly RelUnionConverter Singleton = new();
}

internal class TypeOfSeasonConverter : JsonConverter<TypeOfSeason> {
    public override bool CanConvert(Type t) => t == typeof(TypeOfSeason);

    public override TypeOfSeason Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        if (value == "regular-season") {
            return TypeOfSeason.RegularSeason;
        }
        else if (value == "pre-sesaon")
            return TypeOfSeason.PreSeason;
        return TypeOfSeason.PreSeason;
    }

    public override void Write(Utf8JsonWriter writer, TypeOfSeason value, JsonSerializerOptions options) {
        if (value == TypeOfSeason.RegularSeason) {
            JsonSerializer.Serialize(writer, "regular-season", options);
            return;
        }
        return;
    }

    public static readonly TypeOfSeasonConverter Singleton = new();
}

public class DateOnlyConverter : JsonConverter<DateOnly> {
    private readonly string serializationFormat;
    public DateOnlyConverter() : this(null) { }

    public DateOnlyConverter(string? serializationFormat) {
        this.serializationFormat = serializationFormat ?? "yyyy-MM-dd";
    }

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        return DateOnly.Parse(value!);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(serializationFormat));
}

public class TimeOnlyConverter : JsonConverter<TimeOnly> {
    private readonly string serializationFormat;

    public TimeOnlyConverter() : this(null) { }

    public TimeOnlyConverter(string? serializationFormat) {
        this.serializationFormat = serializationFormat ?? "HH:mm:ss.fff";
    }

    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        return TimeOnly.Parse(value!);
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(serializationFormat));
}
//2023-10-13T00:15Z
internal class IsoDateTimeOffsetConverter : JsonConverter<DateTimeOffset> {
    public override bool CanConvert(Type t) => t == typeof(DateTimeOffset);

    private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

    private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;
    private string? _dateTimeFormat;
    private CultureInfo? _culture;

    public DateTimeStyles DateTimeStyles {
        get => _dateTimeStyles;
        set => _dateTimeStyles = value;
    }

    public string? DateTimeFormat {
        get => _dateTimeFormat ?? string.Empty;
        set => _dateTimeFormat = (string.IsNullOrEmpty(value)) ? null : value;
    }

    public CultureInfo Culture {
        get => _culture ?? CultureInfo.CurrentCulture;
        set => _culture = value;
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) {
        string text;


        if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
            || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal) {
            value = value.ToUniversalTime();
        }

        text = value.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);

        writer.WriteStringValue(text);
    }

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        string? dateText = reader.GetString();

        if (string.IsNullOrEmpty(dateText) == false) {
            if (!string.IsNullOrEmpty(_dateTimeFormat)) {
                return DateTimeOffset.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
            }
            else {
                return DateTimeOffset.Parse(dateText, Culture, _dateTimeStyles);
            }
        }
        else {
            return default(DateTimeOffset);
        }
    }


    public static readonly IsoDateTimeOffsetConverter Singleton = new();
}
internal class DateTimeConverterUsingDateTimeParse : JsonConverter<DateTime> {
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        //Apr 1 2019 12:00
        //Debug.Assert(typeToConvert == typeof(DateTime));
        //return DateTime.SpecifyKind(DateTime.Parse(reader.GetString() ?? string.Empty), DateTimeKind.Utc);
        return DateTime.Parse(reader.GetString() ?? string.Empty);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) {
        writer.WriteStringValue(value.ToString());
    }
}
