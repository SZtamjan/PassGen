using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace CityPasswordGenerator
{
    class Program
    {
        // Twój klucz API z RapidAPI
        const string ApiKey = "Jakiś losowy klucz api";

        // URL do API GeoDB Cities z limitem 10 wyników
        const string ApiUrl = "https://wft-geo-db.p.rapidapi.com/v1/geo/cities?namePrefixDefaultLangResults=true&hateoasMode=false&limit=1&offset=";

        static void Main(string[] args)
        {
            try
            {
                List<string> cities = GetCities();
                if (cities.Count > 0)
                {
                    string password = GeneratePassword(cities);
                    Console.WriteLine($"Wygenerowane hasło: {password}");
                }
                else
                {
                    Console.WriteLine("Nie udało się wygenerować hasła");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
            }
        }

        static List<string> GetCities()
        {
            var random = new Random();
            int randomNumber = random.Next(9500);
            string randomApi = $"{ApiUrl}{randomNumber}";

            var client = new RestClient(randomApi);
            var request = new RestRequest();





            request.AddHeader("x-rapidapi-host", "wft-geo-db.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", ApiKey);

            var response = client.Execute(request);
            if (response.IsSuccessful)
            {
                var jsonResponse = JsonConvert.DeserializeObject<GeoDbResponse>(response.Content);
                List<string> cities = new List<string>();
                foreach (var cityData in jsonResponse.Data)
                {
                    cities.Add(cityData.City);
                }
                return cities;
            }
            else
            {
                // Wyświetlanie szczegółowych informacji o błędzie
                Console.WriteLine($"Failed to fetch data from API: {response.StatusCode} - {response.StatusDescription}");
                Console.WriteLine($"Response content: {response.Content}");
                throw new Exception("Failed to fetch data from API");
            }
        }

        static string GeneratePassword(List<string> cities)
        {
            var random = new Random();
            int cityIndex = random.Next(cities.Count);
            int randomNumber = random.Next(100, 1000);

            return cities[cityIndex] + randomNumber.ToString();
        }
    }

    public class GeoDbResponse
    {
        [JsonProperty("data")]
        public List<CityData> Data { get; set; }
    }

    public class CityData
    {
        [JsonProperty("city")]
        public string City { get; set; }
    }
}