using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class GoogleMapsService
{
    private string apiKey = "AIzaSyAP8kakZfEGQoq4Zc0QsFn8qoTP-lrUezM";

    public GoogleMapsService(string apiKey)
    {
        this.apiKey = apiKey;
    }

    public async Task<List<string>> GetAddressSuggestionsAsync(string input, string languageCode)
    {
        using (var client = new HttpClient())
        {
            // Base URL of the Google Maps API
            string baseUrl = "https://maps.googleapis.com/maps/api/place/autocomplete/json";

            // Build the query string with parameters
            var queryString = new Dictionary<string, string>
            {
                { "key", apiKey },
                { "input", input },
                { "language", languageCode } // Set the language code for localization
            };

            string queryStringParams = string.Join("&", queryString.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

            // Construct the full URL with parameters
            string apiUrl = $"{baseUrl}?{queryStringParams}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var suggestions = JsonConvert.DeserializeObject<GoogleMapsSuggestionsResponse>(content);

                    if (suggestions != null && suggestions.Predictions != null)
                    {
                        return suggestions.Predictions.ConvertAll(prediction => prediction.Description);
                    }
                }
                else
                {
                    Console.WriteLine("API request failed. Status Code: " + response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return new List<string>();
        }
    }
}

public class GoogleMapsSuggestionsResponse
{
    public List<Prediction> Predictions { get; set; }
}

public class Prediction
{
    public string Description { get; set; }
    
}
