/* CV Submission Application
 * 
 * This application performs the following tasks:
 * 1. Generates a dictionary of possible passwords based on variations of "password".
 * 2. Authenticates against a REST API using the generated passwords.
 * 3. Upon successful authentication, retrieves a URL for CV submission.
 * 4. Creates a ZIP file containing the user's CV, this program's source code, and the generated password dictionary.
 * 5. Encodes the ZIP file as a Base64 string and submits it to the retrieved URL.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO.Compression;

partial class Program
{
    // API endpoint for authentication
    private const string AuthUrl = "http://recruitment.warpdevelopment.co.za/api/authenticate";
    const string Username = "John";
    const string DictFile = "dict.txt";

    static async Task Main( string[] args)
    {
        try
        {
            //Password dictionary generator
            var passwords = GeneratePasswordDictionary();
            File.WriteAllLines(DictFile, passwords);
            Console.WriteLine($"Dictionary has been generated successfully and has been saved to: {DictFile}");

            // Authenticate and get the uploaded URL
            string uploadedUrl = await AuthenticateAndGetUploadUrl(passwords);
            Console.WriteLine($"Your authentication has been successful. The Upload URL is: {uploadedUrl}");

            //Create and upload the Zip file
            await CreateAndUploadZip(uploadedUrl);
            Console.WriteLine("The Zip file was uploaded successfuly. ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"The following error has occurred: {ex.Message}");
        }
    }

    // Generates all possible password combinations
    private static IEnumerable<string> GeneratePasswordDictionary()
    {
        string baseWord = "password";
        var result = new HashSet<string>();

        // Recursive function to generate permutations
        void GeneratePermutations(string current, int index)
        {
            if (index == baseWord.Length)
            {
                result.Add(current);
                return;
            }

            char c = baseWord[index];
            GeneratePermutations(current + char.ToLower(c), index + 1);
            GeneratePermutations(current + char.ToUpper(c), index + 1);

            // Special character substitutions
            if (c == 'a')
                GeneratePermutations(current + '@', index + 1);
            else if (c == 's')
                GeneratePermutations(current + '5', index + 1);
            else if (c == 'o')
                GeneratePermutations(current + '0', index + 1);
        }

        GeneratePermutations("", 0);
        return result;
    }

    // Attempts authentication with each password until successful
    private static async Task<string> AuthenticateAndGetUploadUrl(IEnumerable<string> passwords)
    {
        using (var client = new HttpClient())
        {
            foreach (var password in passwords)
            {
                // Creates Basic Authentication header
                var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Username}:{password}"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

                var response = await client.GetAsync(AuthUrl);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
        throw new Exception("Authentication failed for all password combinations.");
    }

    // Creates and uploads the ZIP file to the provided URL
    private static async Task CreateAndUploadZip(string uploadUrl)
    {
        string zipPath = "submission.zip";
        CreateZipFile(zipPath);

        // Read ZIP file and convert to Base64
        byte[] zipBytes = File.ReadAllBytes(zipPath);
        string base64Zip = Convert.ToBase64String(zipBytes);

        // Prepare JSON payload
        var payload = new { Data = base64Zip };
        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Upload ZIP file
        using (var client = new HttpClient())
        {
            try
            {
                var response = await client.PostAsync(uploadUrl, content);
                response.EnsureSuccessStatusCode();
                Console.WriteLine($"Upload successful. Status code: {response.StatusCode}");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Upload failed. Error: {e.Message}");
                throw;
            }
        }
    }

    // Creates a ZIP file containing CV, source code, and dictionary file
    private static void CreateZipFile(string zipPath)
    {
        if (File.Exists(zipPath))
            File.Delete(zipPath);

        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string sourceCodePath = Path.Combine(currentDirectory, "..", "..", "..", "Program.cs");
        string cvPath = Path.Combine(currentDirectory, "Muhammed_Aboobaker_Arbee_CV.pdf");

        using (var zipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
        {
            // Adds cv to zip
            if (File.Exists(cvPath))
                zipArchive.CreateEntryFromFile(cvPath, "Muhammed_Aboobaker_Arbee_CV.pdf");
            else
                Console.WriteLine($"Warning: CV file not found at {cvPath}");

            // Adds source code to zip
            if (File.Exists(sourceCodePath))
                zipArchive.CreateEntryFromFile(sourceCodePath, "Program.cs");
            else
                Console.WriteLine($"Warning: Program.cs not found at {sourceCodePath}");

            // Add the dictionary file to zip
            string dictPath = Path.Combine(currentDirectory, DictFile);
            if (File.Exists(dictPath))
                zipArchive.CreateEntryFromFile(dictPath, DictFile);
            else
                Console.WriteLine($"Warning: Dictionary file not found at {dictPath}");
        }

        // checks to see if the zip file is larger than 5MB limit
        var fileInfo = new FileInfo(zipPath);
        if (fileInfo.Length > 5 * 1024 * 1024)
        {
            throw new Exception("ZIP file size exceeds 5MB limit.");
        }
    }
}