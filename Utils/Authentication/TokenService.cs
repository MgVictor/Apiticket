using apiTicket.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
// using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace apiTicket.Utils.Authentication

{
    public class TokenService
    {
        private static readonly SemaphoreSlim AccessTokenSemaphore;
        private static AccessToken _accessToken;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TokenService> _logger;

        static TokenService()
        {
            _accessToken = null;
            AccessTokenSemaphore = new SemaphoreSlim(1, 1);
        }
        public TokenService(HttpClient httpClient, IConfiguration configuration, ILogger<TokenService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _clientId = config.GetValue<string>("CompanyApi:ClientId");
            _clientSecret = config.GetValue<string>("CompanyApi:ClientSecret");
        }
        public TokenService()
        {
            _accessToken = null;

        }
        //public async Task<AccessToken> GetAccessToken()
        //{
        //    if (_accessToken.Expired is false)
        //    {
        //        return _accessToken;
        //    }

        //    // _accessToken = await FetchToken();
        //    return _accessToken;
        //}
        // public async Task<AccessToken> FetchToken(string type)
        // {
        //     var request = new HttpRequestMessage(HttpMethod.Post, type == "SGC" ? AppSettings.GetTokenAwsSGS + "/oauth2/token" : AppSettings.GetTokenAws360 + "/oauth2/token");
        //     var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
        //     {
        //         { "client_id", _clientId },
        //         { "client_secret", _clientSecret },
        //         { "scope", "https://graph.microsoft.com/.default" },
        //         { "grant_type", "client_credentials" }
        //     });
        //     request.Content = requestContent;
        //     var response = await _httpClient.SendAsync(request);
        //     if (response.IsSuccessStatusCode)
        //     {
        //         var responseContent = await response.Content.ReadAsStringAsync();
        //         var token = JsonSerializer.Deserialize<AccessToken>(responseContent);
        //         _accessToken = token;
        //         return token;
        //     }
        //     else
        //     {
        //         _logger.LogError($"Failed to fetch access token: {response.StatusCode}");
        //         return null;
        //     }
        // }
        public string getTokenAWS(string type = "SGC")
        {
            Token token = null;
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt";
            StreamWriter sw = null;
            sw = new StreamWriter(filepath, true);
            try
            {
                ServicePointManager.ServerCertificateValidationCallback =
delegate (object s, X509Certificate certificate,
X509Chain chain, SslPolicyErrors sslPolicyErrors)
{ return true; };
                var client = new RestClient(type == "SGC" ? AppSettings.GetTokenAwsSGS : AppSettings.GetTokenAws360);
                var request = new RestRequest("oauth2/token", Method.POST);
                //request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Accept", "application/json");

                request.AddParameter("grant_type", "client_credentials");
                //request.AddParameter("client_id", type == "SGC" ? AppSettings.GetUserName_SGC : AppSettings.GetUserName_360, ParameterType.RequestBody);
                //request.AddParameter("client_secret", type == "SGC" ? AppSettings.GetPassword_SGC : AppSettings.GetPassword_360, ParameterType.RequestBody);
                request.AddParameter("scope", AppSettings.GetScope);

                client.Authenticator = new HttpBasicAuthenticator(type == "SGC" ? AppSettings.GetUserName_SGC : AppSettings.GetUserName_360, type == "SGC" ? AppSettings.GetPassword_SGC : AppSettings.GetPassword_360);

                IRestResponse response = client.Execute(request);
                var content = response.Content;
                token = JsonSerializer.Deserialize<Token>(content);
                //return token.access_token;
            }

            catch (Exception ex)
            {
                ex.HelpLink = ex.HelpLink + "error en obtener token " + ex.Message;
                sw.WriteLine(DateTime.Now.ToString() + "-getTokenAWS: " + JsonSerializer.Serialize(ex));
                _logger.LogError($"Failed to fetch access token: {ex.Message}");



            }
            finally
            {

                sw.Flush();
                sw.Close();
            }
            return token.access_token;
        }

        //private async Task<AccessToken> FetchToken()
        //{
        //    try
        //    {
        //        await AccessTokenSemaphore.WaitAsync();

        //        if (_accessToken.Expired is false )
        //        {
        //            return _accessToken;
        //        }

        //        var request = new HttpRequestMessage(HttpMethod.Post, "/connect/token/")
        //        {
        //            Content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
        //            {
        //                new("client_id", _clientId),
        //                new("client_secret", _clientSecret),
        //                new("scope", "company-api"),
        //                new("grant_type", "client_credentials")
        //            })
        //        };

        //         var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        //        response.EnsureSuccessStatusCode();
        //        await   responseContentStream = await response.Content.ReadAsStreamAsync();

        //        var accessToken = await JsonSerializer.DeserializeAsync<AccessToken>(
        //            responseContentStream,
        //            DefaultJsonSerializerOptions.Options);

        //        return accessToken ?? throw new Exception("Failed to deserialize access token");
        //    }
        //    finally
        //    {
        //        AccessTokenSemaphore.Release(1);
        //    }
        //}
        //public async Task<Company> Get(string companyName)
        //{
        //    _logger.LogInformation("Fetching company...");
        //    var accessToken = await GetAccessToken();
        //    var request = new HttpRequestMessage(HttpMethod.Get, $"/api/companies/{companyName}");
        //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);

        //    using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        //    response.EnsureSuccessStatusCode();
        //    await using var responseContentStream = await response.Content.ReadAsStreamAsync();

        //    var company = await JsonSerializer.DeserializeAsync<Company>(
        //        responseContentStream,
        //        DefaultJsonSerializerOptions.Options);

        //    return company ?? throw new Exception("Failed to deserialize company");
        //}
    }
}
