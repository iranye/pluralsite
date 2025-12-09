using Movies.Client.Models;
using System.Text.Json;

namespace Movies.Client.Services;

public class CRUDSamples : IIntegrationService
{
    private const string getMoviesEndpoint = "api/movies";
    private readonly IHttpClientFactory httpClientFactory;

    public CRUDSamples(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task RunAsync()
    {
        await GetResourceAsync();
        await GetResourceThroughHttpRequestMessageAsync();
    }

    public async Task GetResourceAsync()
    {
        var httpClient = httpClientFactory.CreateClient("MoviesAPIClient");

        // UN-COMMENT FOR NON-JSON CONTENT;
        // httpClient.DefaultRequestHeaders.Clear();
        // httpClient.DefaultRequestHeaders.Accept
        //     .Add(
        //     new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
        //     );

        var response = await httpClient.GetAsync(getMoviesEndpoint);
        response.EnsureSuccessStatusCode();

        if (response.Content.Headers.ContentType?.MediaType == "application/json")
        {
            var content = await response.Content.ReadAsStringAsync();
            var movies = JsonSerializer.Deserialize<IEnumerable<Movie>>
                (
                    content,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }
                );
        }
    }

    public async Task GetResourceThroughHttpRequestMessageAsync()
    {
        var httpClient = httpClientFactory.CreateClient("MoviesAPIClient");

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            getMoviesEndpoint);
        request.Headers.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        if (response.Content.Headers.ContentType?.MediaType == "application/json")
        {
            var content = await response.Content.ReadAsStringAsync();
            var movies = JsonSerializer.Deserialize<IEnumerable<Movie>>
                (
                    content,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }
                );
        }
    }
}
