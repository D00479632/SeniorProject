using System;
using System.ComponentModel;
using PropertyChanged.SourceGenerator;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

namespace InGameAssistant
{
    public partial class TextInputViewModel : INotifyPropertyChanged
    {
        [Notify] private string text = "";
        [Notify] private string randomNumberText = "Click the button to generate a response";
        public async void AskQuestion()
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                RandomNumberText = "Please enter a valid question.";
                return;
            }

            RandomNumberText = "We are generating the response, please wait.";
            string responseText = await GetResponseFromServer(Text);
            RandomNumberText = responseText ?? "Failed to retrieve a response.";
        }

        private async Task<string?> GetResponseFromServer(string question)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new { query = question }), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("http://127.0.0.1:8080/ask", content);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var json = System.Text.Json.JsonDocument.Parse(responseBody);
                    return json.RootElement.GetProperty("answer").GetString();
                }
                catch (HttpRequestException)
                {
                    return null;
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
