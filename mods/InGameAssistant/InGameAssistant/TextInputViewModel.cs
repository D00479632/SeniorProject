using System;
using System.ComponentModel;
using PropertyChanged.SourceGenerator;
using System.Net.Http;
using System.Threading.Tasks;

namespace InGameAssistant
{
    public partial class TextInputViewModel : INotifyPropertyChanged
    {
        private readonly Random random = new();
        [Notify] private string text = "";
        [Notify] private string randomNumberText = "Click the button to generate a random number";

        public async void GenerateRandom()
        {
            int randomNumber = await GetRandomNumberFromServer();
            RandomNumberText = randomNumber != -1 
                ? $"Your random number is: {randomNumber}" 
                : "Failed to retrieve random number.";
        }

        private async Task<int> GetRandomNumberFromServer()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("http://127.0.0.1:8080/random");
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var json = System.Text.Json.JsonDocument.Parse(responseBody);
                    return json.RootElement.GetProperty("random_number").GetInt32();
                }
                catch (HttpRequestException e)
                {
                    // Log error (you may want to use a logging mechanism here)
                    return -1;
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}