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
using System.ComponentModel;
using StardewUI.Framework;
using StardewUI;

namespace InGameAssistant
{
    internal sealed class ModEntry : Mod
    {
        private Process? _pythonProcess;
        private ModConfig _config;
        private static readonly HttpClient _heartbeatClient = new HttpClient();
        private CancellationTokenSource? _heartbeatCancellation;
        private IViewEngine? viewEngine;
        private TextInputViewModel viewModel;

        public ModEntry()
        {
            _config = new ModConfig();
            viewModel = new TextInputViewModel();
        }

        public override void Entry(IModHelper helper)
        {
            try
            {
                // Load the configuration
                _config = helper.ReadConfig<ModConfig>();
                
                // Set up UI engine
                helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
                
                // Start the Python server when the game is launched
                helper.Events.GameLoop.SaveLoaded += (sender, e) => 
                {
                    this.Monitor.Log("About to start the server.", LogLevel.Info); 
                    StartPythonServer();
                    StartHeartbeat();
                };

                // Handle input events
                helper.Events.Input.ButtonPressed += this.OnButtonPressed;

                // Stop the Python server when the game ends
                helper.Events.GameLoop.ReturnedToTitle += async (sender, e) => 
                {
                    StopHeartbeat();
                    await StopPythonServer();
                };
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Failed to initialize mod: {ex.Message}", LogLevel.Error);
            }
        }

        private void GameLoop_GameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            viewEngine = Helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI");
            viewEngine.RegisterViews("Mods/InGameAssistant/Views", "assets/views");
            viewEngine.EnableHotReloading();
        }

        private async void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button == SButton.F8)
            {
                Game1.activeClickableMenu = viewEngine.CreateMenuFromAsset(
                    "Mods/InGameAssistant/Views/TextInput",
                    viewModel);
            }

            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
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

                string output = await _pythonProcess.StandardOutput.ReadToEndAsync();
                string error = await _pythonProcess.StandardError.ReadToEndAsync();
                this.Monitor.Log($"Python server output: {output}", LogLevel.Debug);
                this.Monitor.Log($"Python server error: {error}", LogLevel.Debug);

                Thread.Sleep(5000);
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

        private string GetPythonScriptPath()
        {
            string path = Path.Combine(this.Helper.DirectoryPath, "script.py");
            this.Monitor.Log($"Python script path: {path}", LogLevel.Debug);
            return path;
        }

        private string GetPythonExePath()
        {
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
                    await Task.Delay(15000, cancellationToken);
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
                    await Task.Delay(15000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    internal class ModConfig
    {
        public string PythonExePath { get; set; } = ""; 
    }
}