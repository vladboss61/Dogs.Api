namespace Dogs.Api;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net;
using System.IO;
using System.Linq;

internal sealed class DogResponse
{
    public string Status { get; set; }

    public string Message { get; set; }

    public override string ToString()
    {
        return $"Status: {Status}, Message: {Message}";
    }
}


internal class Program
{
    internal static async Task Main(string[] args)
    {
        Console.WriteLine("Dogs Api HTTP Example.");
        
        await DownloadDogAsync();
        
        Environment.Exit(0);
    }

    public static async Task DownloadDogAsync()
    {
        using var httpClient = new HttpClient();
        HttpResponseMessage dogResponseMessage = await httpClient.GetAsync("https://dog.ceo/api/breeds/image/random");

        if (dogResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            string dogString = await dogResponseMessage.Content.ReadAsStringAsync();
            DogResponse dogResponse  = JsonSerializer.Deserialize<DogResponse>(dogString, new JsonSerializerOptions {  PropertyNameCaseInsensitive = true });

            Console.WriteLine(dogResponse.ToString());

            HttpResponseMessage imageResponseMessage = await httpClient.GetAsync(dogResponse.Message);
            
            if (imageResponseMessage.IsSuccessStatusCode)
            {
                Stream contentStream = await imageResponseMessage.Content.ReadAsStreamAsync();

                var fileName = dogResponse.Message.Split("/").LastOrDefault();

                using var file = File.Create(fileName);
                
                await contentStream.CopyToAsync(file);

                await file.FlushAsync();

                Console.WriteLine("File with dog is downloaded, check its.");
            }
        }
    }
}
