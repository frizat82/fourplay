using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using SportslineOdds;

public class SportslineOddsService : ISportslineOddsService {

    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memory;

    public SportslineOddsService(HttpClient httpClient, IMemoryCache memoryCache) {
        _httpClient = httpClient;
        _memory = memoryCache;
        // Add any additional configuration for the HttpClient here
    }

    public async Task<SportslineOddsData?> GetOdds() {
        return await _memory.GetOrCreateAsync<SportslineOddsData?>("odds", async (option) => {
            SportslineOddsData? results = null;
            option.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            for (int i = 0; i <= 2; i++) {
                try {
                    using var request = new HttpRequestMessage(HttpMethod.Post, "/ui-gateway/v1/graphql");

                    var data = new StringContent(
                        "{\"query\":\"query { odds(league: \\\"nfl\\\", limit: 20, offset: " + i + ") { sportsBooks { name sortOrder } oddsCompetitions { id abbr homeTeamId awayTeamId homeTeam { id league sport location nickName mediumName shortName abbr status color colorPrimary colorSecondary deleted } awayTeam { id league sport location nickName mediumName shortName abbr status color colorPrimary colorSecondary deleted } homeTeamForecast { abbr displayName hasScore name score projectedScore color medName } awayTeamForecast { abbr displayName hasScore name score projectedScore color medName } venue { id abbr name city state country zipCode timeZone weatherCode type } neutral tvInfo { name callLetters countryId teamId countryName typeId typeName } scheduledTime competitionStatus league sport sportsBookOdds deleted matchupBreakdowns { awayStats homeStats label sortOrder } expertPicksCount } } }\"",
                        Encoding.UTF8,
                        "application/json"
                    );

                    request.Content = new StringContent("{\"query\":\"query {\\n        odds(league: \\\"nfl\\\", limit: 40, offset: " + i.ToString() + ") {\\n            sportsBooks {\\n                name\\n                sortOrder\\n            }\\n            oddsCompetitions {\\n                id\\n                abbr\\n                homeTeamId\\n                awayTeamId\\n                homeTeam {\\n                    id\\n                    league\\n                    sport\\n                    location\\n                    nickName\\n                    mediumName\\n                    shortName\\n                    abbr\\n                    status\\n                    color\\n                    colorPrimary\\n                    colorSecondary\\n                    deleted\\n                }\\n                awayTeam {\\n                    id\\n                    league\\n                    sport\\n                    location\\n                    nickName\\n                    mediumName\\n                    shortName\\n                    abbr\\n                    status\\n                    color\\n                    colorPrimary\\n                    colorSecondary\\n                    deleted\\n                }\\n                homeTeamForecast {\\n                    abbr\\n                    displayName\\n                    hasScore\\n                    name\\n                    score\\n                    projectedScore\\n                    color\\n                    medName\\n                }\\n                awayTeamForecast {\\n                    abbr\\n                    displayName\\n                    hasScore\\n                    name\\n                    score\\n                    projectedScore\\n                    color\\n                    medName\\n                }\\n                venue {\\n                    id\\n                    abbr\\n                    name\\n                    city\\n                    state\\n                    country\\n                    zipCode\\n                    timeZone\\n                    weatherCode\\n                    type\\n                }\\n                neutral\\n                tvInfo {\\n                    name\\n                    callLetters\\n                    countryId\\n                    teamId\\n                    countryName\\n                    typeId\\n                    typeName\\n                }\\n                scheduledTime\\n                competitionStatus\\n                league\\n                sport\\n                sportsBookOdds\\n                deleted\\n                matchupBreakdowns {\\n                    awayStats\\n                    homeStats\\n                    label\\n                    sortOrder\\n                }\\n                expertPicksCount\\n            }\\n        }\\n      }\"}");
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await _httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode) {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions {
                            PropertyNameCaseInsensitive = true
                        };
                        SportslineOddsConverters.Settings.Converters.ToList().ForEach(x => options.Converters.Add(x));
                        // Deserialize the JSON response into a .NET object
                        var deserializedObject = JsonSerializer.Deserialize<SportslineOddsData>(responseString, options);
                        if (deserializedObject is not null) {
                            if (results is null)
                                results = deserializedObject;
                            else {
                                results.Data.Odds.OddsCompetitions = results.Data.Odds.OddsCompetitions.Concat(deserializedObject.Data.Odds.OddsCompetitions).ToArray();
                            }
                        }
                    }
                    else {
                        Log.Error($"Error: {response.ReasonPhrase}");
                        return null;
                    }
                }
                catch (HttpRequestException e) {
                    Log.Error($"HTTP Request error: {e.Message}");
                    return null;
                }
            }
            return results;
        });
    }
}