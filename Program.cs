using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using RestSharp;
using static System.Net.Mime.MediaTypeNames;

namespace CityPasswordGenerator
{
    class Program
    {
        // Twój klucz API z RapidAPI
        const string ApiKey = "xxx";

        // URL do API GeoDB Cities
        const string ApiUrl = "https://wft-geo-db.p.rapidapi.com/v1/geo/cities?namePrefixDefaultLangResults=true&hateoasMode=false&limit=1&offset=";

        const int minPassLenght = 12;
        const int maxPassLenght = 16;

        static void Main(string[] args)
        {
            try
            {
                //List<string> cities = GetCities();
                string city = GetCity();

                string password = GeneratePassword(city);
                Console.WriteLine($"Wygenerowane hasło: {password}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
            }
        }

        static string GetCity()
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
                string city = "";
                foreach (var cityData in jsonResponse.Data)
                {
                    city = cityData.City;
                }
                return city;
            }
            else
            {
                // Wyświetlanie szczegółowych informacji o błędzie
                Console.WriteLine($"Failed to fetch data from API: {response.StatusCode} - {response.StatusDescription}");
                Console.WriteLine($"Response content: {response.Content}");
                throw new Exception("Failed to fetch data from API");
            }
        }

        static string GeneratePassword(string city)
        {
            var random = new Random();
            int randomNumber = random.Next(100, 1000);
            string character = "";

            String sc = "!@#$%^&*~";
            int sz = random.Next(sc.Length);
            character = sc.ElementAt(sz).ToString();

            string password = character + city + randomNumber;

            while (password.Any(Char.IsWhiteSpace) || Regex.IsMatch(city, @"abcdefghijklmnopqrstuvwxyz0123456789") || 
                password.Length < minPassLenght || password.Length > maxPassLenght )
            {
                Thread.Sleep(1100);
                city = GetCity();

                password = character + city + randomNumber;
            }

            return password;
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