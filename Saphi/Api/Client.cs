using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;

namespace Saphira.Saphi.Api
{
    public class Client
    {
        private readonly HttpClient _httpClient;
        private readonly Configuration _configuration;

        public Client(HttpClient httpClient, Configuration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            if (!string.IsNullOrWhiteSpace(_configuration.SaphiBaseUrl))
            {
                _httpClient.BaseAddress = new Uri(_configuration.SaphiBaseUrl);
            }

            if (!string.IsNullOrWhiteSpace(_configuration.SaphiApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Saphi-Api-Key", _configuration.SaphiApiKey);
            }

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private string BuildUrlWithQuery(string endpoint, Dictionary<string, string>? queryParams)
        {
            if (queryParams == null || queryParams.Count == 0)
                return endpoint;

            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var param in queryParams)
            {
                query[param.Key] = param.Value;
            }

            return $"{endpoint}?{query}";
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<T?> GetAsync<T>(string endpoint, Dictionary<string, string> queryParams)
        {
            var url = BuildUrlWithQuery(endpoint, queryParams);
            return await GetAsync<T>(url);
        }
    }
}
