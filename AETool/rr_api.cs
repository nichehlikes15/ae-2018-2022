using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

#pragma warning disable

namespace AETool
{
    internal class Api
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<List<Dictionary<string, object>>> GetPlayerData(List<int> ids)
        {
            string playersApi = "https://api.rec.net/api/players/v2/progression/bulk?id=";

            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(playersApi),
                    Method = HttpMethod.Get,
                };
                request.Headers.Add("Accept", "*/*");

                string idsStr = string.Join(",", ids);
                request.RequestUri = new Uri($"{playersApi}?id={idsStr}");

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    List<Dictionary<string, object>> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseContent);
                    return data;
                }
                else
                {
                    Console.WriteLine($"Error retreiving username: {response.StatusCode}");
                    return null;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request Error: {e.Message}");
                return null;
            }
        }

        public static async Task<Dictionary<string, object>> GetUsername(string playerId)
        {
            string accountsApi = "https://accounts.rec.net/account/bulk?id=";

            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{accountsApi}{playerId}"),
                    Method = HttpMethod.Get,
                };
                request.Headers.Add("Accept", "*/*");

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Dictionary<string, object> data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);
                    return data;
                }
                else
                {
                    Console.WriteLine($"Error retrieving username: {response.StatusCode}");
                    return null;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request Error: {e.Message}");
                return null;
            }
        }
    }
}
