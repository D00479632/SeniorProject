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

            // Print the question being sent for debugging
            Console.WriteLine($"Sending question: {Text}");

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
            
                    // Log the request being sent
                    string requestBody = await content.ReadAsStringAsync(); // Get the string content
                    Console.WriteLine($"Sending question: {requestBody}"); // Log the actual JSON being sent

                    HttpResponseMessage response = await client.PostAsync("http://127.0.0.1:8080/ask", content);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var json = System.Text.Json.JsonDocument.Parse(responseBody);

                    if (json.RootElement.TryGetProperty("answer", out var answerElement))
                    {
                        return answerElement.GetString();
                    }
                    else
                    {
                        Console.WriteLine($"The answer was: {json.RootElement.GetRawText()}");
                        return "Error: Server response missing 'answer' key.";
                    }
                }
                catch (HttpRequestException e)
                {
                    return $"Error: {e.Message}";
                }
                catch (System.Text.Json.JsonException)
                {
                    return "Error: Invalid JSON response from server.";
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
