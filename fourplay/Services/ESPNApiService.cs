using System.Text.Json;
using fourplay.Helpers;
using fourplay.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
namespace fourplay.Services;
public class ESPNApiService : IESPNApiService {
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memory;

    public ESPNApiService(HttpClient httpClient, IMemoryCache memoryCache) {
        _httpClient = httpClient;
        _memory = memoryCache;
        // Add any additional configuration for the HttpClient here
    }
    public async Task<ESPNScores?> GetWeekScores(int week, int year, bool postSeason = false) {
        return await _memory.GetOrCreateAsync<ESPNScores?>($"scores:{week}:{year}:{postSeason}", async (option) => {
            option.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            try {
                // Replace the endpoint with the actual ESPN API endpoint for NFL spreads
                var response = await _httpClient.GetAsync($"/apis/site/v2/sports/football/nfl/scoreboard?dates={year}&seasontype={(postSeason ? 3 : 2)}&week={week}");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode) {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions {
                        PropertyNameCaseInsensitive = true
                    };
                    ESPNApiServiceJsonConverter.Settings.Converters.ToList().ForEach(x => options.Converters.Add(x));
                    // Deserialize the JSON response into a .NET object
                    foreach (var map in NFLTeamMappingHelpers.NFLTeamAbbrMapping) {
                        if (responseString.Contains($"\"{map.Key}\""))
                            responseString = responseString.Replace($"\"{map.Key}\"", $"\"{map.Value}\"");
                    }
                    var deserializedObject = JsonSerializer.Deserialize<ESPNScores>(responseString, options);

                    // Use the deserialized object as needed
                    return deserializedObject;
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
);
    }
    /*
        public async Task<ESPNApiNFLSeasonDetail?> GetSeasonDetail(int year)
        {
            return await _memory.GetOrCreateAsync<ESPNApiNFLSeasonDetail?>($"seasons:{year}", async (option) =>
      {
          option.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
          try
          {
              // Replace the endpoint with the actual ESPN API endpoint for NFL spreads
              var response = await _httpClient.GetAsync($"/apis/site/v2/sports/football/leagues/nfl/seasons/{year}?lang=en&region=us");
              response.EnsureSuccessStatusCode();
              if (response.IsSuccessStatusCode)
              {
                  var responseString = await response.Content.ReadAsStringAsync();
                  var options = new JsonSerializerOptions
                  {
                      PropertyNameCaseInsensitive = true
                  };
                  // Deserialize the JSON response into a .NET object
                  var deserializedObject = JsonSerializer.Deserialize<ESPNApiNFLSeasonDetail>(responseString, options);

                  // Use the deserialized object as needed
                  Log.Error($"Deserialized object: {deserializedObject}");
                  return deserializedObject;
              }
              else
              {
                  Log.Error($"Error: {response.ReasonPhrase}");
                  return null;
              }
          }
          catch (HttpRequestException e)
          {
              Log.Error($"HTTP Request error: {e.Message}");
              return null;
          }
      }
    );
        }
        public async Task<ESPNApiNFLSeasons?> GetSeasons()
        {
            return await _memory.GetOrCreateAsync<ESPNApiNFLSeasons?>($"seasons", async (option) =>
            {
                option.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                try
                {
                    // Replace the endpoint with the actual ESPN API endpoint for NFL spreads
                    var response = await _httpClient.GetAsync($"/apis/site/v2/sports/football/leagues/nfl/seasons?limit=100");
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        // Deserialize the JSON response into a .NET object
                        var deserializedObject = JsonSerializer.Deserialize<ESPNApiNFLSeasons>(responseString, options);

                        // Use the deserialized object as needed
                        Log.Error($"Deserialized object: {deserializedObject}");
                        return deserializedObject;
                    }
                    else
                    {
                        Log.Error($"Error: {response.ReasonPhrase}");
                        return null;
                    }
                }
                catch (HttpRequestException e)
                {
                    Log.Error($"HTTP Request error: {e.Message}");
                    return null;
                }
            });
        }
    */
    public async Task<ESPNScores?> GetScores() {
        return await _memory.GetOrCreateAsync<ESPNScores?>("scores", async (option) => {
            option.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            try {
                // Replace the endpoint with the actual ESPN API endpoint for NFL spreads
                var response = await _httpClient.GetAsync("/apis/site/v2/sports/football/nfl/scoreboard");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode) {
                    var responseString = await response.Content.ReadAsStringAsync();
                    if (String.IsNullOrEmpty(responseString))
                        return new ESPNScores();
                    var options = new JsonSerializerOptions {
                        PropertyNameCaseInsensitive = true
                    };
                    ESPNApiServiceJsonConverter.Settings.Converters.ToList().ForEach(x => options.Converters.Add(x));
                    foreach (var map in NFLTeamMappingHelpers.NFLTeamAbbrMapping) {
                        if (responseString.Contains($"\"{map.Key}\""))
                            responseString = responseString.Replace($"\"{map.Key}\"", $"\"{map.Value}\"");
                    }
                    // Deserialize the JSON response into a .NET object
                    var deserializedObject = JsonSerializer.Deserialize<ESPNScores>(responseString, options);

                    // Use the deserialized object as needed
                    Log.Error($"Deserialized object: {deserializedObject}");
                    return deserializedObject;
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
    );
    }
}