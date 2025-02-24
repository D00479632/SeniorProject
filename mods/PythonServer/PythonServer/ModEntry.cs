using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.IO;
using System.Text;
using System.Threading;

namespace PythonServer
{
    internal sealed class ModEntry : Mod
    {
        private Process? _pythonProcess; // Declare as nullable
        private ModConfig _config; // Configuration object
        private static readonly HttpClient _heartbeatClient = new HttpClient();
        private CancellationTokenSource? _heartbeatCancellation;

        public ModEntry()
        {
            // Initialize _config with a default value
            _config = new ModConfig();
        }

        public override void Entry(IModHelper helper)
        {
            try
            {
                // Load the configuration
                _config = helper.ReadConfig<ModConfig>();
                
                // Start the Python server when the game is launched
                helper.Events.GameLoop.SaveLoaded += (sender, e) => 
                {
                    this.Monitor.Log("About to start the server.", LogLevel.Info); 
                    StartPythonServer();
                    StartHeartbeat(); // Start heartbeat when game is loaded
                };

                // Adds an 'event handler' when the button-pressed event happens.
                helper.Events.Input.ButtonPressed += this.OnButtonPressed;

                // Stop the Python server when the game ends
                helper.Events.GameLoop.ReturnedToTitle += async (sender, e) => 
                {
                    StopHeartbeat(); // Stop heartbeat when returning to title
                    await StopPythonServer();
                };
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Failed to initialize mod: {ex.Message}", LogLevel.Error);
            }
        }

        private async void StartPythonServer()
        {
            string pythonScriptPath = GetPythonScriptPath();
            string pythonExePath = GetPythonExePath();

            _pythonProcess = new Process();
            _pythonProcess.StartInfo.FileName = pythonExePath;
            _pythonProcess.StartInfo.Arguments = $"\"{pythonScriptPath}\""; 
            _pythonProcess.StartInfo.UseShellExecute = false;
            _pythonProcess.StartInfo.RedirectStandardOutput = true;
            _pythonProcess.StartInfo.RedirectStandardError = true;
            _pythonProcess.StartInfo.CreateNoWindow = true;

            try
            {
                _pythonProcess.Start();
                this.Monitor.Log("Python script started.", LogLevel.Info);

                // Log the output and error streams
                string output = await _pythonProcess.StandardOutput.ReadToEndAsync();
                string error = await _pythonProcess.StandardError.ReadToEndAsync();
                this.Monitor.Log($"Python server output: {output}", LogLevel.Debug);
                this.Monitor.Log($"Python server error: {error}", LogLevel.Debug);

                // Wait for the Python server to start
                // This just blocks the current thread
                Thread.Sleep(5000);
                //await Task.Delay(5000);
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Failed to start Python script: {ex.Message}", LogLevel.Error);
            }
        }

        private async Task StopPythonServer()
        {
            if (_pythonProcess != null && !_pythonProcess.HasExited)
            {
                // Send a shutdown request to the Python server
                using (HttpClient client = new())
                {
                    try
                    {
                        HttpResponseMessage shutdownResponse = await client.GetAsync("http://127.0.0.1:8080/shutdown");
                        shutdownResponse.EnsureSuccessStatusCode();
                        Thread.Sleep(5000);
                        this.Monitor.Log("Shutdown request sent to Python server.", LogLevel.Info);
                    }
                    catch (HttpRequestException e)
                    {
                        this.Monitor.Log($"Shutdown request error: {e.Message}", LogLevel.Error);
                    }
                }

                _pythonProcess.Kill();
                _pythonProcess.Dispose();
                this.Monitor.Log("Python server stopped.", LogLevel.Info);
            }
        }

        private async void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button == SButton.MouseRight)
            {
                if (_pythonProcess == null || _pythonProcess.HasExited)
                {
                    this.Monitor.Log("Python server is not running.", LogLevel.Error);
                    return;
                }

                int randomNumber = await GetRandomNumberFromServer(); // Request random number from Python server
                if (randomNumber != -1) // Check if the number was successfully retrieved
                {
                    this.Monitor.Log($"Received random number: {randomNumber}", LogLevel.Info);
                    Game1.hudMessages.Add(new HUDMessage($"Your random number is {randomNumber}", HUDMessage.achievement_type));
                }
            }

            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
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
                    return json.RootElement.GetProperty("random_number").GetInt32(); // Return the random number
                }
                catch (HttpRequestException e)
                {
                    this.Monitor.Log($"Request error: {e.Message}", LogLevel.Error);
                    return -1; // Return -1 to indicate an error
                }
            }
        }

        private string GetPythonScriptPath()
        {
            // Get the path to the Python script relative to the mod's directory
            string path = Path.Combine(this.Helper.DirectoryPath, "script.py");
            this.Monitor.Log($"Python script path: {path}", LogLevel.Debug);
            return path;
        }

        private string GetPythonExePath()
        {
            // Use the configured path if available, otherwise fall back to "python3" or "python"
            if (!string.IsNullOrEmpty(_config.PythonExePath))
            {
                string path = _config.PythonExePath;
                this.Monitor.Log($"Using Python executable: {path}", LogLevel.Debug);
                return path;
            }

            throw new InvalidOperationException("Python executable path is not configured in config.json.");
        }

        private void StartHeartbeat()
        {
            _heartbeatCancellation = new CancellationTokenSource();
            _ = SendHeartbeats(_heartbeatCancellation.Token);
        }

        private void StopHeartbeat()
        {
            _heartbeatCancellation?.Cancel();
            _heartbeatCancellation?.Dispose();
            _heartbeatCancellation = null;
        }

        private async Task SendHeartbeats(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(15000, cancellationToken); // 30 second delay between heartbeats
                    var response = await _heartbeatClient.PostAsync(
                        "http://127.0.0.1:8080/heartbeat", 
                        new StringContent("", Encoding.UTF8, "application/json"),
                        cancellationToken
                    );
                    this.Monitor.Log($"Heartbeat sent: {response.StatusCode}", LogLevel.Trace);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    this.Monitor.Log($"Failed to send heartbeat: {ex.Message}", LogLevel.Warn);
                }
                
                try
                {
                    await Task.Delay(15000, cancellationToken); // 30 second delay between heartbeats
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    // Configuration class for the mod
    internal class ModConfig
    {
        public string PythonExePath { get; set; } = ""; 
    }
}