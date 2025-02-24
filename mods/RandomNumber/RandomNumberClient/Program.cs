using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Path to the Python script and Python executable
        // The path where the Python script is located
        string pythonScriptPath = @"/Users/paulalozanogonzalo/spring2025/CS-4600/mod/RandomNumber/PythonServer/app.py"; 
        // The path to the Python executable, which is used to run the script
        string pythonExePath = @"/Users/paulalozanogonzalo/.venv/bin/python"; 

        // Start the Python script as a new process
        Process pythonProcess = new Process();
        pythonProcess.StartInfo.FileName = pythonExePath; // Set the executable to run
        pythonProcess.StartInfo.Arguments = pythonScriptPath; // Set the script to run as an argument
        pythonProcess.StartInfo.UseShellExecute = false; // Do not use the shell to start the process
        pythonProcess.StartInfo.RedirectStandardOutput = true; // Redirect standard output to read it later
        pythonProcess.StartInfo.RedirectStandardError = true; // Redirect standard error to read it later
        pythonProcess.StartInfo.CreateNoWindow = true; // Do not create a window for the process

        try
        {
            pythonProcess.Start(); // Start the Python process
            Console.WriteLine("Python script started."); 

            // Read the output and error streams asynchronously
            var outputTask = pythonProcess.StandardOutput.ReadToEndAsync(); // Task to read output
            var errorTask = pythonProcess.StandardError.ReadToEndAsync(); // Task to read errors

            // Wait for the Python server to start (adjust delay if necessary)
            await Task.Delay(2000); 

            // Send an HTTP request to the Python server
            using (HttpClient client = new HttpClient()) // Create a new HttpClient instance
            {
                try
                {
                    // Send a GET request to the Python server to get a random number
                    HttpResponseMessage response = await client.GetAsync("http://127.0.0.1:8080/random");
                    response.EnsureSuccessStatusCode(); // Ensure the request was successful

                    // Read the response content as a string
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Parse the JSON response to get the random number
                    var json = System.Text.Json.JsonDocument.Parse(responseBody);
                    int randomNumber = json.RootElement.GetProperty("random_number").GetInt32(); // Extract the random number

                    // Display the random number
                    Console.WriteLine($"Random Number: {randomNumber}");
                }
                catch (HttpRequestException e) 
                {
                    Console.WriteLine($"Request error: {e.Message}");
                }
            }

            // Send a shutdown request to the Python server
            using (HttpClient client = new()) // Create another HttpClient instance
            {
                try
                {
                    // Send a GET request to shut down the Python server
                    HttpResponseMessage shutdownResponse = await client.GetAsync("http://127.0.0.1:8080/shutdown");
                    shutdownResponse.EnsureSuccessStatusCode(); // Ensure the shutdown request was successful
                    Console.WriteLine("Shutdown request sent to Python server."); // Notify that the shutdown request was sent
                }
                catch (HttpRequestException e) 
                {
                    Console.WriteLine($"Shutdown request error: {e.Message}");
                }
            }

           
            pythonProcess.WaitForExit(); // Block until the Python process exits
            Console.WriteLine("Python script stopped gracefully."); // Notify that the script has stopped

            // Display the Python process output and error (for debugging)
            string output = await outputTask; // Get the output from the process
            string error = await errorTask; // Get the error from the process

            // Display the output and error if they exist
            if (!string.IsNullOrEmpty(output))
                Console.WriteLine("Python Output: " + output);
            if (!string.IsNullOrEmpty(error))
                Console.WriteLine("Python Error: " + error);
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"Failed to start Python script: {ex.Message}"); 
            return; 
        }
    }
}

/*
 * THE SERVER NEEDS TO BE UP FOR THIS ONE
using System;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Send a GET request to the Python server
                HttpResponseMessage response = await client.GetAsync("http://127.0.0.1:8080/random");
                response.EnsureSuccessStatusCode(); // Throw an exception if the request fails

                // Read the response content as a string
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse the JSON response to get the random number
                var json = System.Text.Json.JsonDocument.Parse(responseBody);
                int randomNumber = json.RootElement.GetProperty("random_number").GetInt32();

                // Display the random number
                Console.WriteLine($"Random Number: {randomNumber}");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
        }
    }
}
*/
